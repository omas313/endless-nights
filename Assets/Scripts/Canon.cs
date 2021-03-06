using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField] Transform _aimRoot;
    [SerializeField] Transform _debugMarker;
    [SerializeField] Transform _shotPoint;
    [SerializeField] float _maxLineLength = 2f;
    [SerializeField] float _minShotAngle = 0f;
    [SerializeField] float _maxShotAngle = 180f;
    [SerializeField] Projectile _projectilePrefab;

    Transform _junkParent;
    Vector3 _direction;
    float _shotCooldownTimer;
    float _chargeTimer;
    float _lineLength = 0f;
    bool _canShoot;

    void Update()
    {
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
    private void SetShootingDirection(Vector3 lineDirectionTest)
    {
        _direction = lineDirectionTest;
        var lineEndPoint = _direction * _lineLength;
    }

    void ShootProjectile()
    {
        _canShoot = false;

        var projectile = Instantiate(_projectilePrefab, _shotPoint.position, Quaternion.identity, _junkParent);
        var velocityUnitVector = new Vector2(_direction.x, _direction.y).normalized;

        projectile.Finished += OnProjectileFinished;
        projectile.SetDirection(velocityUnitVector);
        projectile.Shoot();
    }

    void OnProjectileFinished(Projectile projectile)
    {
        _canShoot = true;
        projectile.Finished -= OnProjectileFinished;
    }

    void Awake()
    {
        _canShoot = true;
        _junkParent = GameObject.FindWithTag("Junk").transform;
    }
}
