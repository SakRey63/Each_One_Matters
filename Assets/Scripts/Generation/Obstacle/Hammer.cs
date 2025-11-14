using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Generation.Obstacle
{
    public class Hammer : BridgeObject, IBridgeObjectInstantiator
    {
        [SerializeField] private float _yOffset = 2f; 
        [SerializeField] private float _turnAngle = 180f;

        public void InstantiateBridgeObstacle(Vector3 position, Quaternion rotation, ObstacleSide side)
        {
            Vector3 adjustedPosition = new Vector3(position.x, _yOffset, position.z);

            Quaternion rotationHammer = rotation;

            if (side == ObstacleSide.Left)
            {
                float currentYAngle = rotation.eulerAngles.y;
                currentYAngle -= _turnAngle;
                rotationHammer = Quaternion.Euler(rotation.eulerAngles.x, currentYAngle, rotation.z);
            }

            Instantiate(this, adjustedPosition, rotationHammer);
        }
    }
}