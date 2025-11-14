using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Generation.Obstacle
{
    public class RotatingBlade : BridgeObject, IBridgeObjectInstantiator
    {
        private float _verticalPositionRotatingBlade = 0;
        
        public void InstantiateBridgeObstacle(Vector3 position, Quaternion rotation, ObstacleSide side)
        {
            position = new Vector3(position.x, _verticalPositionRotatingBlade, position.z);

            Instantiate(this, position, rotation);
        }
    }
}