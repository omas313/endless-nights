using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BombProjectile : Projectile
{
    public static event Action<int> Chain;

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
        var toDestroy = new Queue<Collider2D>();
        
        var colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _enemiesLayerMask);
        foreach (var collider in colliders)
            toDestroy.Enqueue(collider);

        var camera = FindObjectOfType<CinemachineVirtualCamera>();
        var previousFollow = camera.Follow;

        int totalCount = colliders.Length;
        while (toDestroy.Count > 0)
        {
            var current = toDestroy.Dequeue();
            
            current.GetComponent<Enemy>().TakeHit();
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.0125f, 0.025f));

            colliders = Physics2D.OverlapCircleAll(current.transform.position, _explosionRadius, _enemiesLayerMask);

            foreach (var collider in colliders)
                if (collider != null && !toDestroy.Contains(collider))
                {
                    toDestroy.Enqueue(collider);
                    totalCount++;
                }

            if (totalCount > 2)
            {
                Time.timeScale = 0.2f;
                camera.Follow = transform;
            }
        }

        if (totalCount > 2)
        {
            // Debug.Log($"chain: {totalCount}");
            Chain?.Invoke(totalCount * 5);
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
