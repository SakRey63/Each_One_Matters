using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Generation.Obstacle
{
    public class SawBlade : MonoBehaviour, IBridgeObject
    {
        public BridgeObjectType Type => BridgeObjectType.SawBlade;
    }
}