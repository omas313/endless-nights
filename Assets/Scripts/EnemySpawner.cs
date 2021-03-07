using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy _enemyPrefab;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] Transform _enemiesParent;

    [Header("Enemy Stats")]
    [SerializeField] float _minAmplitude = 0.2f;
    [SerializeField] float _maxAmplitude = 3f;
    [SerializeField] float _minPeriod = 0.2f;
    [SerializeField] float _maxPeriod = 0.8f;
    [SerializeField] float _minMoveSpeed = 0.2f;
    [SerializeField] float _maxMoveSpeed = 1f;
    [SerializeField] float yOffset = 2.5f;
    [SerializeField] AudioClip[] _spawnAudioClips;

    AudioSource _audioSource;

    public void SpawnEnemies(int level, int count, float speed = 0f)
    {
        StartCoroutine(SpawnEnemiesWithDelay(level, count, speed));
    }

    IEnumerator SpawnEnemiesWithDelay(int level, int count, float speed)
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        _audioSource.PlayOneShot(_spawnAudioClips[UnityEngine.Random.Range(0, _spawnAudioClips.Length)]);

        var maxAmplitude = Mathf.Clamp(UnityEngine.Random.Range(_minAmplitude, _minAmplitude + level / 5f), _minAmplitude, _maxAmplitude);
        var maxPeriod = Mathf.Clamp(UnityEngine.Random.Range(_minPeriod, _minPeriod + level / 10f), _minPeriod, _maxPeriod);
        var maxMoveSpeed = Mathf.Clamp(UnityEngine.Random.Range(_minMoveSpeed, _minMoveSpeed + level / 2f), _minMoveSpeed, _maxMoveSpeed);

        for (var i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.4f));
            
            var position = GetRandomPositionOnY();
            var enemy = Instantiate(_enemyPrefab, position, Quaternion.identity, _enemiesParent);
            enemy.RandomiseMovement(
                position.y,
                UnityEngine.Random.Range(_minAmplitude, maxAmplitude) * Mathf.Abs(position.y / yOffset),
                UnityEngine.Random.Range(_minPeriod, maxPeriod),
                speed == 0f ? UnityEngine.Random.Range(_minMoveSpeed, maxMoveSpeed) : speed
            ); 
            enemy.Move();
        }
    }

    Vector3 GetRandomPositionOnY() => new Vector3(
        _spawnPoint.position.x, 
        UnityEngine.Random.Range(-yOffset, yOffset),
        0f);
}
