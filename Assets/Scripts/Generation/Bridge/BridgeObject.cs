using UnityEngine;

namespace EachOneMatters.Generation.Bridge
{
    public abstract class BridgeObject: MonoBehaviour
    {
        [SerializeField] private BridgeObjectType _type;

        public BridgeObjectType Type => _type;
    }
}