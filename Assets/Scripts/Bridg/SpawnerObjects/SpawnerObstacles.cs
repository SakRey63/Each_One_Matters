using UnityEngine;

public class SpawnerObstacles : MonoBehaviour
{
    [SerializeField] private Hammer _hammer;
    [SerializeField] private RotatingBlade _rotatingBlade;
    [SerializeField] private SawBlade _sawBlade;
    [SerializeField] private SpikedCylinder _spikedCylinder;
    [SerializeField] private SpikePress _spikePress;
    [SerializeField] private Spikes _spikes;
    [SerializeField] private float _turnAngle = 180f;
    [SerializeField] private float _yOffsetHammer = 2;
    [SerializeField] private float _verticalPositionRotatingBlade;

    public void CreateHummer(Vector3 positionHammer, SegmentPositionGenerator positionGenerator, Quaternion targetRotation)
    {
        positionHammer = positionGenerator.GetObstaclePosition(positionHammer);

        positionHammer = new Vector3(positionHammer.x, _yOffsetHammer, positionHammer.z);
            
        Quaternion rotationHammer = targetRotation;
            
        if (positionGenerator.IsMonsterPositionRight == false)
        {
            float currentYAngle = targetRotation.eulerAngles.y;
            currentYAngle -= _turnAngle;
            rotationHammer = Quaternion.Euler(targetRotation.eulerAngles.x, currentYAngle, targetRotation.z);
        }
            
        Instantiate(_hammer, positionHammer, rotationHammer);
    }
    
    public void CreateSpikePress(Vector3 positionSpikePress, SegmentPositionGenerator positionGenerator, Quaternion targetRotation)
    {
        positionGenerator.GetObstaclePosition(positionSpikePress);
        Quaternion rotationSpikePress = targetRotation;
        positionSpikePress = positionGenerator.GetPositionCenterLevel(positionSpikePress, _verticalPositionRotatingBlade);
        
        if (positionGenerator.IsMonsterPositionRight == false)
        {
            float currentYAngle = targetRotation.eulerAngles.y;
            currentYAngle -= _turnAngle;
            rotationSpikePress = Quaternion.Euler(targetRotation.eulerAngles.x, currentYAngle, targetRotation.z);
        }
        
        Instantiate(_spikePress, positionSpikePress, rotationSpikePress);
    }

    public void CreateSpikes(Vector3 positionSpikes, SegmentPositionGenerator segmentPositionGenerator, Quaternion targetRotation)
    {
        positionSpikes = segmentPositionGenerator.GetRandomPositionToLevel(positionSpikes);
        positionSpikes = new Vector3(positionSpikes.x, _verticalPositionRotatingBlade, positionSpikes.z);
        Instantiate(_spikes, positionSpikes, targetRotation);
    }

    public void CreateRotatingBlade(Vector3 positionRotatingBlade, SegmentPositionGenerator positionGenerator, Quaternion targetRotation)
    {
        positionRotatingBlade = positionGenerator.GetPositionCenterLevel(positionRotatingBlade, _verticalPositionRotatingBlade);
        Instantiate(_rotatingBlade, positionRotatingBlade, targetRotation);
    }

    public void CreateSawBlade(Vector3 positionSawBlade, SegmentPositionGenerator positionGenerator, Quaternion targetRotation)
    {
        positionSawBlade = positionGenerator.GetPositionCenterLevel(positionSawBlade, _verticalPositionRotatingBlade);
        Instantiate(_sawBlade, positionSawBlade, targetRotation);
    }
    
    public void CreateSpikedCylinder(Vector3 positionSpikedCylinder, SegmentPositionGenerator positionGenerator, Quaternion targetRotation)
    {
        positionSpikedCylinder = positionGenerator.GetPositionCenterLevel(positionSpikedCylinder, _verticalPositionRotatingBlade);
        Instantiate(_spikedCylinder, positionSpikedCylinder, targetRotation);
    }
}