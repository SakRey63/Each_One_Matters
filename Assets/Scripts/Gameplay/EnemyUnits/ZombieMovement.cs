using System;
using UnityEngine;

namespace EachOneMatters.Gameplay.EnemyUnits
{
    public class ZombieMovement : MonoBehaviour
    {
        [SerializeField] private float _normalSpeed = 7;
        [SerializeField] private float _boostedSpeed = 14;
        [SerializeField] private float _minBorderY = 1;

        private float _movementSpeed;
        private Transform _transform;
        private float _threshold = 0.01f;

        public event Action OnTargetReached;
    
        private void Awake()
        {
            _transform = transform;
        }

        public void Move(bool isMoving, Transform target)
        {
            if (isMoving)
            {
                float distanceSqr = (target.position - _transform.position).sqrMagnitude;
        
                if (distanceSqr < _threshold)
                {
                    OnTargetReached?.Invoke();
                }
                else
                {
                    _transform.position = Vector3.MoveTowards(_transform.position, target.position, _movementSpeed * Time.deltaTime);
                }

                if (_transform.position.y <= _minBorderY)
                {
                    Vector3 stabilizationOffsetY = new Vector3(_transform.position.x, _minBorderY, _transform.position.z);
                    _transform.position = stabilizationOffsetY;
                }
            }
        }

        public void SetSpeed(float speed)
        {
            _movementSpeed = speed;
        }

        public void ResetSpeed()
        {
            _movementSpeed = _normalSpeed;
        }

        public void Accelerate()
        {
            _movementSpeed = _boostedSpeed;
        }
    }
}