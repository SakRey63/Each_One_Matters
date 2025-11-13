using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Generation.Obstacle
{
    public class Hammer : MonoBehaviour, IBridgeObject
    {
        public BridgeObjectType Type => BridgeObjectType.Hammer;
    }
}