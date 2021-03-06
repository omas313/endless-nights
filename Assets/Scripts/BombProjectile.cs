using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BombProjectile : Projectile
{
    [SerializeField] float _explosionRadius = 1f;
    [SerializeField] LayerMask _enemiesLayerMask;

    bool _isExploding;

    public override void Shoot()
    {
        rigidbodyComp = GetComponent<Rigidbody2D>();

        var angle = Vector2.SignedAngle(Vector2.right, this.direction);
        var rotation = Quaternion.Euler(0f, 0f, angle);
        transform.localRotation = rotation;

        rigidbodyComp.velocity = this.direction * speed;
    }

    void Explode()
    {
        _isExploding = true;

        rigidbodyComp.velocity = Vector2.zero;
        GetComponentInChildren<SpriteRenderer>().enabled = false;

        if (collisionParticles != null)
            Instantiate(collisionParticles, transform.position, Quaternion.identity, GameObject.FindWithTag("Junk").transform);

        StartCoroutine(ChainExplosion());
    }

    IEnumerator ChainExplosion()
    {
        var toVisitQueue = new Queue<Collider2D>();
        
        var colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _enemiesLayerMask);
        foreach (var collider in colliders)
            toVisitQueue.Enqueue(collider);

        var camera = FindObjectOfType<CinemachineVirtualCamera>();
        var previousFollow = camera.Follow;

        while (toVisitQueue.Count > 0)
        {
            var current = toVisitQueue.Dequeue();
            
            current.GetComponent<Enemy>().TakeHit();
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.0125f, 0.025f));

            colliders = Physics2D.OverlapCircleAll(current.transform.position, _explosionRadius, _enemiesLayerMask);

            if (colliders.Length > 0 && camera.Follow != transform)
            {
                Time.timeScale = 0.2f;
                camera.Follow = transform;
            }

            foreach (var collider in colliders)
                if (collider != null && !toVisitQueue.Contains(collider))
                    toVisitQueue.Enqueue(collider);
        }

        camera.Follow = previousFollow;
        Time.timeScale = 1f;

        InvokeFinishedEvent();
        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Explode();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !_isExploding)
            Explode();    
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
