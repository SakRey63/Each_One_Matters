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

    public void CreateHummer(Vector3 positionHammer, bool isMonsterPositionRight, Quaternion targetRotation)
    {
        positionHammer = new Vector3(positionHammer.x, _yOffsetHammer, positionHammer.z);
            
        Quaternion rotationHammer = targetRotation;
            
        if (isMonsterPositionRight == false)
        {
            float currentYAngle = targetRotation.eulerAngles.y;
            currentYAngle -= _turnAngle;
            rotationHammer = Quaternion.Euler(targetRotation.eulerAngles.x, currentYAngle, targetRotation.z);
        }
            
        Instantiate(_hammer, positionHammer, rotationHammer);
    }
    
    public void CreateSpikePress(Vector3 positionSpikePress, bool isMonsterPositionRight, Quaternion targetRotation)
    {
        Quaternion rotationSpikePress = targetRotation;
        positionSpikePress = new Vector3(positionSpikePress.x, _verticalPositionRotatingBlade, positionSpikePress.z);
        
        if (isMonsterPositionRight == false)
        {
            float currentYAngle = targetRotation.eulerAngles.y;
            currentYAngle -= _turnAngle;
            rotationSpikePress = Quaternion.Euler(targetRotation.eulerAngles.x, currentYAngle, targetRotation.z);
        }
        
        Instantiate(_spikePress, positionSpikePress, rotationSpikePress);
    }

    public void CreateSpikes(Vector3 positionSpikes, Quaternion targetRotation)
    {
        positionSpikes = new Vector3(positionSpikes.x, _verticalPositionRotatingBlade, positionSpikes.z);
        Instantiate(_spikes, positionSpikes, targetRotation);
    }

    public void CreateRotatingBlade(Vector3 positionRotatingBlade, Quaternion targetRotation)
    {
        positionRotatingBlade = new Vector3(positionRotatingBlade.x, _verticalPositionRotatingBlade, positionRotatingBlade.z);
        
        Instantiate(_rotatingBlade, positionRotatingBlade, targetRotation);
    }

    public void CreateSawBlade(Vector3 positionSawBlade, Quaternion targetRotation)
    {
        positionSawBlade = new Vector3(positionSawBlade.x, _verticalPositionRotatingBlade, positionSawBlade.z);
        Instantiate(_sawBlade, positionSawBlade, targetRotation);
    }
    
    public void CreateSpikedCylinder(Vector3 positionSpikedCylinder, Quaternion targetRotation)
    {
        positionSpikedCylinder = new Vector3(positionSpikedCylinder.x, _verticalPositionRotatingBlade, positionSpikedCylinder.z);
        Instantiate(_spikedCylinder, positionSpikedCylinder, targetRotation);
    }
}