using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Generation.Obstacle
{
    public class SpikedCylinder : MonoBehaviour, IBridgeObject
    {
        public BridgeObjectType Type => BridgeObjectType.SpikedCylinder;
    }
}