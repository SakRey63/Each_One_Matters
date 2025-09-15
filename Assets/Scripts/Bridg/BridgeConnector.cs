using System;
using UnityEngine;

public class BridgeConnector : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _bridgeStartPointLeft;
    [SerializeField] private Transform _bridgeStartPointRight;
    [SerializeField] private Transform _placeholderPoint;
    [SerializeField] private Transform _obstacleCratesBlocking;
    [SerializeField] private float _xOffset = 16;

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
    
    public void SetIndex(int index, bool isTurnRight)
    {
        _index = index;
        SetObstaclePosition(isTurnRight);
    }

    private void SetObstaclePosition(bool isTurnRight)
    {
        Vector3 position = _obstacleCratesBlocking.localPosition;

        if (isTurnRight)
        {
            position = new Vector3(-_xOffset, position.y, position.z);
        }
        else
        {
            position = new Vector3(_xOffset, position.y, position.z);
        }

        _obstacleCratesBlocking.localPosition = position;
    }
}