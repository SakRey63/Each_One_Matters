using UnityEngine;

public class Hammer : MonoBehaviour, IBridgeObject
{
    public BridgeObjectType Type => BridgeObjectType.Hammer;
}