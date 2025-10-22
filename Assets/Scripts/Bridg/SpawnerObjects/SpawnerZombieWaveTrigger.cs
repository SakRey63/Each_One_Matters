using UnityEngine;

public class SpawnerZombieWaveTrigger : MonoBehaviour
{
    [SerializeField] private PointSpawnTrigger _pointSpawnPrefab;
    [SerializeField] private float _verticalPositionZombieWaveTrigger = 1f;
    
    public PointSpawnTrigger GetZombieTrigger(Vector3 position, Quaternion targetRotation, SegmentPositionGenerator segmentPositionGenerator)
    {
        Vector3 newPosition = segmentPositionGenerator.GetPositionCenterLevel(position, _verticalPositionZombieWaveTrigger);
        
        PointSpawnTrigger spawnTrigger = Instantiate(_pointSpawnPrefab, newPosition, targetRotation);

        return spawnTrigger;
    }
}