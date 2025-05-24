using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerZombie : MonoBehaviour
{
    [SerializeField] private int _zombieMultiplier = 5;
    [SerializeField] private int _minCounZombie = 10;
    [SerializeField] private float _spawnDelay = 0.2f;
    [SerializeField] private float _borderX = 5.7f;

    private Transform _spawnPosition;
    private ZombiePool _zombiePool;

    private void Awake()
    {
        _zombiePool = GetComponent<ZombiePool>();
    }

    public void SpawnAdaptiveWave(Transform spawnPosition, int policeCount)
    {
        _spawnPosition = spawnPosition;

        int count = GetCalculateCountZombies(policeCount);

        StartCoroutine(SpawnWithDelay(count));
    }

    private IEnumerator SpawnWithDelay(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Zombie zombie = _zombiePool.GetZombie();
            zombie.transform.position = GetRandomPositionSpawn();
            zombie.transform.rotation = _spawnPosition.rotation;
            yield return new WaitForSeconds(_spawnDelay);
        }
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
        
    private Vector3 GetRandomPositionSpawn()
    {
        Vector3 position = new Vector3(Random.Range(-_borderX, _borderX), _spawnPosition.position.y,
            _spawnPosition.position.z);
        
        return position;
    }
}
