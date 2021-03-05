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

    public void SpawnEnemies(int count)
    {
        StartCoroutine(SpawnEnemiesWithDelay(count));
    }

    IEnumerator SpawnEnemiesWithDelay(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.5f));
            
            var position = GetRandomPositionOnY();
            var enemy = Instantiate(_enemyPrefab, position, Quaternion.identity, _enemiesParent);
            enemy.RandomiseMovement(
                position.y,
                UnityEngine.Random.Range(_minAmplitude, _maxAmplitude) * Mathf.Abs(position.y / yOffset),
                UnityEngine.Random.Range(_minPeriod, _maxPeriod),
                UnityEngine.Random.Range(_minMoveSpeed, _maxMoveSpeed)
            ); 
            enemy.Move();
        }
    }

    Vector3 GetRandomPositionOnY() => new Vector3(
        _spawnPoint.position.x, 
        UnityEngine.Random.Range(-yOffset, yOffset),
        0f);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SpawnEnemies(10);        
    }
}
