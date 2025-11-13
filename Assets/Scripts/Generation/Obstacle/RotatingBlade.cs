using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Generation.Obstacle
{
    public class RotatingBlade : MonoBehaviour, IBridgeObject
    {
        public BridgeObjectType Type => BridgeObjectType.RotatingBlade;
    }
}