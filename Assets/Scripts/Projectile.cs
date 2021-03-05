using System;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public event Action<Projectile> Finished;

    [SerializeField] protected float speed = 1f;
    [SerializeField] protected ParticleSystem collisionParticles;
    
    protected Rigidbody2D rigidbodyComp;
    protected Vector2 direction;

    public abstract void Shoot();

    public void SetDirection(Vector2 direction) => this.direction = direction;

    protected void InvokeFinishedEvent() => Finished?.Invoke(this);
}
