using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerZombie : ObjectPool<Zombie>
{
    [SerializeField] private float _spawnDelay = 0.1f;
    [SerializeField] private float _border = 5.5f;
    [SerializeField] private int _healthPoint = 75;
    
    private Transform _spawnPosition;
    private Transform _targetPosition;
    private bool _isHorizontal;

    public event Action<bool> OnZombieKilled;

    public void SpawnAdaptiveWave(Transform spawnPosition, int count, Transform targetPosition, bool isHorizontal)
    {
        _isHorizontal = isHorizontal;
        _spawnPosition = spawnPosition;
        _targetPosition = targetPosition;

        StartCoroutine(SpawnWithDelay(count));
    }

    private IEnumerator SpawnWithDelay(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Zombie zombie = GetObject();
            zombie.transform.position = GetRandomPositionSpawn();
            zombie.transform.rotation = _spawnPosition.rotation;
            zombie.SetFinish(_targetPosition);
            zombie.SetHealthPoint(_healthPoint);
            zombie.SetMovementSpeed();
            zombie.SetScanEnemy();
            zombie.OnZombieDeath += ReturnZombie;
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private void ReturnZombie(Zombie zombie)
    {
        zombie.OnZombieDeath -= ReturnZombie;
        OnZombieKilled?.Invoke(zombie.IsKilledByBullet);
        ReturnObject(zombie);
    }

    private Vector3 GetRandomPositionSpawn()
    {
        Vector3 position;
        
        if (_isHorizontal)
        {
            position = new Vector3(_spawnPosition.position.x, _spawnPosition.position.y, Random.Range(_spawnPosition.position.z -_border, _spawnPosition.position.z + _border));
        }
        else
        {
            position = new Vector3(Random.Range(_spawnPosition.position.x -_border, _spawnPosition.position.x + _border), _spawnPosition.position.y, _spawnPosition.position.z);
        }
        
        return position;
    }
}
