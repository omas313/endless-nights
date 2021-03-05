using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : Projectile
{
    public override void Shoot()
    {
        rigidbodyComp = GetComponent<Rigidbody2D>();

        if (rigidbodyComp != null)
            rigidbodyComp.velocity = this.direction * speed;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        var enemy = other.collider.GetComponent<Enemy>();
        if (enemy == null)
            return;

        enemy.TakeHit();            

        if (collisionParticles != null)
            Instantiate(collisionParticles, transform.position, Quaternion.identity);

        GetComponentInChildren<SpriteRenderer>().enabled = false;
        rigidbodyComp.simulated = false;
        InvokeFinishedEvent();
        Destroy(gameObject, 0.5f);
    }
}
