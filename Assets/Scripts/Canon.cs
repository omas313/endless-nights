using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField] Transform _aimRoot;
    [SerializeField] Transform _debugMarker;
    [SerializeField] Transform _shotPoint;
    [SerializeField] float _minShotAngle = 0f;
    [SerializeField] float _maxShotAngle = 180f;
    [SerializeField] Projectile _projectilePrefab;
    [SerializeField] Animator _aimLightAnimator;

    [SerializeField] AudioClip[] _shotAudioClips;

    AudioSource _audioSource;
    Transform _junkParent;
    Vector3 _direction;
    bool _canShoot;
    bool _active = true;

    public void Activate()
    {
        _canShoot = true;
        _active = true;
        _aimLightAnimator.SetBool("InActive", false);
    }

    public void Deactivate()
    {
        _canShoot = false;
        _active = false;
        _aimLightAnimator.SetBool("InActive", true);
    }

    void Update()
    {
        if (!_active)
            return;

        HandleAimLightRotation();

        if (Input.GetButtonDown("Fire1") && _canShoot)
            ShootProjectile();
    }

    void HandleAimLightRotation()
    {
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        _debugMarker.position = mouseWorldPosition;

        var directionTest = (mouseWorldPosition - _aimRoot.transform.position).normalized;
        var testAngle = Vector2.SignedAngle(directionTest, Vector2.right);
        var zRotation = Mathf.Clamp(testAngle, _minShotAngle, _maxShotAngle);
        _aimRoot.localRotation = Quaternion.Euler(0f, 0f, -zRotation);
        
        if (WithinAllowedAngle(testAngle))
            _direction = directionTest;
    }

    bool WithinAllowedAngle(float angle) => angle >= _minShotAngle && angle <= _maxShotAngle;

    void ShootProjectile()
    {
        _canShoot = false;

        _audioSource.PlayOneShot(_shotAudioClips[UnityEngine.Random.Range(0, _shotAudioClips.Length)]);
        _aimLightAnimator.SetBool("InActive", true);

        var projectile = Instantiate(_projectilePrefab, _shotPoint.position, Quaternion.identity, _junkParent);
        var velocityUnitVector = new Vector2(_direction.x, _direction.y).normalized;

        projectile.Finished += OnProjectileFinished;
        projectile.SetDirection(velocityUnitVector);
        projectile.Shoot();
    }

    void OnProjectileFinished(Projectile projectile)
    {
        _canShoot = true;
        _aimLightAnimator.SetBool("InActive", false);
        projectile.Finished -= OnProjectileFinished;
    }

    void Awake()
    {
        _canShoot = true;
        _junkParent = GameObject.FindWithTag("Junk").transform;
        _audioSource = GetComponent<AudioSource>();
    }
}
