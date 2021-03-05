using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamProjectile : Projectile
{
    [SerializeField] ParticleSystem _particles;
    [SerializeField] float _explosionRadius = 1f;
    [SerializeField] LayerMask _enemiesLayerMask;

    Animation _animation;

    public override void Shoot()
    {
        StartCoroutine(ShootBeam());
    }

    IEnumerator ShootBeam()
    {
        
        var angle = Vector2.SignedAngle(Vector2.right, this.direction);
        var rotation = Quaternion.Euler(0f, 0f, angle);
        transform.localRotation = rotation;

        _particles.Play();
        _animation.Play();

        yield return new WaitUntil(() => _particles.particleCount == 0 && !_animation.isPlaying);

        InvokeFinishedEvent();
        Destroy(gameObject, 5f);
    }

    IEnumerator ChainExplosion(Vector3 position)
    {
        var toVisitQueue = new Queue<Collider2D>();
        var visitedSet = new HashSet<Collider2D>();
        
        var colliders = Physics2D.OverlapCircleAll(position, _explosionRadius, _enemiesLayerMask);
        foreach (var collider in colliders)
            toVisitQueue.Enqueue(collider);

        while (toVisitQueue.Count > 0)
        {
            var current = toVisitQueue.Dequeue();
            visitedSet.Add(current);

            colliders = Physics2D.OverlapCircleAll(current.transform.position, _explosionRadius, _enemiesLayerMask);
            foreach (var collider in colliders)
                if (!toVisitQueue.Contains(collider) && !visitedSet.Contains(collider))
                    toVisitQueue.Enqueue(collider);
        }

        foreach (var collider in visitedSet)
        {
            if (collider != null)
                collider.GetComponent<Enemy>().TakeHit();
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.025f, 0.075f));
        }
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();

        if (enemy != null)    
            StartCoroutine(ChainExplosion(enemy.transform.position));
    }

    void Awake()
    {
        _animation = GetComponent<Animation>();
    }
}