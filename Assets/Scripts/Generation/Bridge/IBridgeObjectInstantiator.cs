using UnityEngine;

namespace EachOneMatters.Generation.Bridge
{
    public interface IBridgeObjectInstantiator
    {
        public void InstantiateBridgeObstacle(Vector3 position, Quaternion rotation, ObstacleSide side);
    }
}