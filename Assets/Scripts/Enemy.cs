using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static event Action<int> Died;

    const float TAU = 2f * (float)Math.PI;

    public int Damage => _damage;

    [SerializeField] ParticleSystem _explosionParticles;
    [SerializeField] int _damage = 5;
    [SerializeField] int _scoreForKill = 5;

    private float _yOffset;
    float _amplitude = 0f;
    float _period = 0f;
    float _moveSpeed = 0f;

    bool _shouldMove;

    public void RandomiseMovement(float yOffset, float amplitude, float period, float speed)
    {
        _yOffset = yOffset;
        _amplitude = amplitude;
        _period = period;
        _moveSpeed = speed;
    }

    public void TakeHit()
    {
        StartCoroutine(DestroySelf());
    }

    public void Move()
    {
        _shouldMove = true;

        float sineWave = Mathf.Sin(TAU * _period * Time.time);
        var yPosition = _amplitude * sineWave + _yOffset;
        var xPosition = transform.position.x + Time.deltaTime * -_moveSpeed;

        transform.position = new Vector3(xPosition, yPosition, 0f);
    }

    IEnumerator DestroySelf()
    {
        _shouldMove = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        _explosionParticles.Play();
        Died?.Invoke(_scoreForKill);
        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);
    }

    void Update()
    {
        if (_shouldMove)
            Move();
    }

    void Awake()
    {
        if (UnityEngine.Random.value < 0.5f)
            GetComponent<Animator>().SetBool("UseAlternate", true);    
    }
}
