using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField] LineRenderer _line;
    [SerializeField] Transform _debugMarker;
    [SerializeField] Transform _shotPoint;
    [SerializeField] float _maxLineLength = 2f;
    [SerializeField] float _minShotAngle = 0f;
    [SerializeField] float _maxShotAngle = 180f;
    [SerializeField] Projectile _projectilePrefab;

    Transform _junkParent;
    Vector3 _lineDirection;
    float _shotCooldownTimer;
    float _chargeTimer;
    float _lineLength = 0f;
    bool _canShoot;

    void Update()
    {
        HandleLineRendererPositioning();

        if (Input.GetButtonDown("Fire1") && _canShoot)
            ShootProjectile();
    }

    void HandleLineRendererPositioning()
    {
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        _debugMarker.position = mouseWorldPosition;

        var lineDirectionTest = (mouseWorldPosition - _line.transform.position).normalized;
        lineDirectionTest.x *= transform.localScale.x;
        
        var testAngle = Vector2.SignedAngle(lineDirectionTest, Vector2.up);
        // Debug.Log(testAngle);
        SetShootingDirection(WithinAllowedAngle(testAngle) ? lineDirectionTest : _lineDirection);
    }

    private void SetShootingDirection(Vector3 lineDirectionTest)
    {
        _lineDirection = lineDirectionTest;
        var lineEndPoint = _lineDirection * _lineLength;
        _line.SetPosition(1, lineDirectionTest * _maxLineLength);
    }

    bool WithinAllowedAngle(float angle) => angle >= _minShotAngle && angle <= _maxShotAngle;
    
    void ShootProjectile()
    {
        _canShoot = false;

        var projectile = Instantiate(_projectilePrefab, _shotPoint.position, Quaternion.identity, _junkParent);
        var velocityUnitVector = new Vector2(_lineDirection.x, _lineDirection.y).normalized;

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
