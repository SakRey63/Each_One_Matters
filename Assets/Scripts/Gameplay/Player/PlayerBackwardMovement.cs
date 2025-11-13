using System;
using UnityEngine;

namespace EachOneMatters.Gameplay.Player
{
    public class PlayerBackwardMovement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;

        private Transform _transform;
        private Vector3 _targetPosition;
        private float _threshold = 0.001f;

        public event Action OnPlayerFinish; 
        
        public float Speed => _speed;
        
        private void Awake()
        {
            _transform = transform;
        }

        public void MoveBackward()
        {
            if ((_targetPosition - _transform.position).sqrMagnitude < _threshold)
            {
                OnPlayerFinish?.Invoke();
            }
            else
            {
                _transform.position = Vector3.MoveTowards(_transform.position, _targetPosition, _speed * Time.deltaTime);
            }
        }

        public void SetTargetPosition(Transform moveTarget)
        {
            _targetPosition = new Vector3(moveTarget.position.x, _transform.position.y, moveTarget.position.z);
        }
    }
}