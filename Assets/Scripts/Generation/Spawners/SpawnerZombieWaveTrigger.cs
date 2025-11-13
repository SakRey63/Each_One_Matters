using UnityEngine;

public class SpawnerZombieWaveTrigger : MonoBehaviour
{
    [SerializeField] private PointSpawnTrigger _pointSpawnPrefab;
    [SerializeField] private float _verticalPositionZombieWaveTrigger = 1f;
    
    public PointSpawnTrigger GetZombieTrigger(Vector3 position, Quaternion targetRotation)
    {
        Vector3 spawnPosition = new Vector3(position.x, _verticalPositionZombieWaveTrigger, position.z);
        
        PointSpawnTrigger spawnTrigger = Instantiate(_pointSpawnPrefab, spawnPosition, targetRotation);

        return spawnTrigger;
    }
}