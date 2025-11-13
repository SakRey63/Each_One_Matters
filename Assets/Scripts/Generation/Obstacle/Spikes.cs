using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Generation.Obstacle
{
    public class Spikes : MonoBehaviour, IBridgeObject
    {
        public BridgeObjectType Type => BridgeObjectType.Spikes;
    }
}