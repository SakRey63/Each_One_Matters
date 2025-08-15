using System;
using UnityEngine;

public class BridgeConnector : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _bridgeStartPointLeft;
    [SerializeField] private Transform _bridgeStartPointRight;
    [SerializeField] private Transform _placeholderPoint;

    private int _index;
    public Transform RotationTarget => _target;
    public Transform BridgeStartPointRight => _bridgeStartPointRight;
    public Transform BridgeStartPointLeft => _bridgeStartPointLeft;
    public int Index => _index;

    public event Action<Zombie, BridgeConnector> OnZombieDetected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Zombie zombie))
        {
            OnZombieDetected?.Invoke(zombie, this);
        }
    }
    
    public void SetIndex(int index)
    {
        _index = index;
    }
}
