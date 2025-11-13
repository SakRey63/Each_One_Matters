using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Generation.Obstacle
{
    public class SpikePress : MonoBehaviour, IBridgeObject
    {
        public BridgeObjectType Type => BridgeObjectType.SpikePress;
    }
}