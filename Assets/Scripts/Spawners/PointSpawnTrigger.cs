using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PointSpawnTrigger : MonoBehaviour
{
    [SerializeField] private Transform _zombieSpawnArea;
    [SerializeField] private int _zombieMultiplier = 3;
    [SerializeField] private int _minCounZombie = 10;
    [SerializeField] private float _spawnDelay = 0.1f;
    [SerializeField] private float _border = 5.5f;

    private SpawnerZombie _spawnerZombie;
    private EnemyGroupController _enemyGroup;
    private bool _isHorizontal;
    private int _index;

    public int Index => _index;
    
    public event Action<PointSpawnTrigger> OnHordeSpawning;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PoliceOfficer>(out _))
        {
            OnHordeSpawning?.Invoke(this);
        }
    }

    public void SetDirectionAndIndex(bool isHorizontal, int index)
    {
        _index = index;
        _isHorizontal = isHorizontal;
    }
    
    public void AssignSpawnerAndGroup(SpawnerZombie spawnerZombie, EnemyGroupController enemyGroup)
    {
        _spawnerZombie = spawnerZombie;
        _enemyGroup = enemyGroup;
    }
    
    public void SpawnAdaptiveWave(int countOfficer, Transform targetPosition)
    {
        int countZombie = GetCalculateCountZombies(countOfficer);
        
        if (_spawnerZombie != null)
        {
            StartCoroutine(SpawnWithDelay(countZombie, targetPosition));
        }
    }
    
    private IEnumerator SpawnWithDelay(int count, Transform targetPosition)
    {
        for (int i = 0; i < count; i++)
        {
            Zombie zombie = _spawnerZombie.CreateEnemy();
            zombie.SetFinish(targetPosition);
            _enemyGroup.AddEnemy(zombie);
            zombie.transform.position = GetRandomPositionSpawn();
            zombie.transform.rotation = _zombieSpawnArea.rotation;
            yield return new WaitForSeconds(_spawnDelay);
        }
    }
    
    private Vector3 GetRandomPositionSpawn()
    {
        Vector3 position;

        if (_isHorizontal)
        {
            position = new Vector3(_zombieSpawnArea.position.x, _zombieSpawnArea.position.y,
                Random.Range(_zombieSpawnArea.position.z - _border, _zombieSpawnArea.position.z + _border));
        }
        else
        {
            position = new Vector3(
                Random.Range(_zombieSpawnArea.position.x - _border, _zombieSpawnArea.position.x + _border),
                _zombieSpawnArea.position.y, _zombieSpawnArea.position.z);
        }

        return position;
    }
    
    private int GetCalculateCountZombies(int policeCount)
    {
        int count = policeCount * _zombieMultiplier;

        if (count < _minCounZombie)
        {
            count = _minCounZombie;
        }
        
        return count;
    }
    
    private void OnDrawGizmos()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if(collider == null)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix; 
        Gizmos.DrawWireCube(collider.center, collider.size);
    }
}
