using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Generation.Obstacle
{
    public class SpikePress : BridgeObject, IBridgeObjectInstantiator
    {
        [SerializeField] private float _turnAngle = 180f;
        [SerializeField] private float _verticalPositionRotatingBlade;
        
        public void InstantiateBridgeObstacle(Vector3 positionSpikePress, Quaternion rotation, ObstacleSide side)
        {
            Quaternion rotationSpikePress = rotation;
            positionSpikePress = new Vector3(positionSpikePress.x, _verticalPositionRotatingBlade, positionSpikePress.z);

            if (side == ObstacleSide.Left)
            {
                float currentYAngle = rotation.eulerAngles.y;
                currentYAngle -= _turnAngle;
                rotationSpikePress = Quaternion.Euler(rotation.eulerAngles.x, currentYAngle, rotation.z);
            }
            
            Instantiate(this, positionSpikePress, rotationSpikePress);
        }
    }
}