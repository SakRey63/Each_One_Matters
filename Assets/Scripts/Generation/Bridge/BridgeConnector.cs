using System;
using EachOneMatters.Gameplay.EnemyUnits;
using UnityEngine;

namespace EachOneMatters.Generation.Bridge
{
    public class BridgeConnector : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _bridgeStartPointLeft;
        [SerializeField] private Transform _bridgeStartPointRight;
        [SerializeField] private Transform _obstacleCratesBlocking;
        [SerializeField] private float _xOffset = 16;

        private int _index;
    
        public event Action<Zombie, BridgeConnector> OnZombieDetected;
    
        public Transform RotationTarget => _target;
        public Transform BridgeStartPointRight => _bridgeStartPointRight;
        public Transform BridgeStartPointLeft => _bridgeStartPointLeft;
        public int Index => _index;

        private void OnDestroy()
        {
            OnZombieDetected = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Zombie zombie))
            {
                OnZombieDetected?.Invoke(zombie, this);
            }
        }
    
        public void SetIndex(int index, BridgeDirection isTurnRight)
        {
            _index = index;
            SetObstaclePosition(isTurnRight);
        }

        private void SetObstaclePosition(BridgeDirection isTurnRight)
        {
            Vector3 position = _obstacleCratesBlocking.localPosition;

            if (isTurnRight == BridgeDirection.HorizontalRight)
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
}