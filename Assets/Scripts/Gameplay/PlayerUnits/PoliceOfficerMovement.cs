using System;
using EachOneMatters.Common;
using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Gameplay.PlayerUnits
{
    public class PoliceOfficerMovement : MonoBehaviour
    {
        private float _maxBorderPosition;
        private float _minBorderPosition;
        private float _speed;
        private Transform _transform;
        private Vector3 _targetPositionInGroup;
        private bool _isTargetToPoint;
        private float _threshold = 0.001f;
        private BridgeDirection _currentDirection;

        public event Action OnReachedRegroupPoint;

        public float Speed => _speed;

        private void Awake()
        {
            _transform = transform;
        }

        public void Move(ProgressionState state)
        {
            if (_isTargetToPoint)
            {
                switch (_currentDirection)
                {
                    case BridgeDirection.VerticalUp:

                        if (_transform.position.x <= _minBorderPosition && state == ProgressionState.Idle)
                        {
                            _transform.position = new Vector3(_minBorderPosition, _transform.position.y, _transform.position.z);
                        }
                        else if (_transform.position.x >= _maxBorderPosition && state == ProgressionState.Idle)
                        {
                            _transform.position = new Vector3(_maxBorderPosition, _transform.position.y, _transform.position.z);
                        }
                        else
                        {
                            MoveTowardsTarget(_targetPositionInGroup);
                        }

                        break;

                    default:

                        if (_transform.position.z <= _minBorderPosition && state == ProgressionState.Idle)
                        {
                            _transform.position = new Vector3(_transform.position.x, _transform.position.y, _minBorderPosition);
                        }
                        else if (_transform.position.z >= _maxBorderPosition && state == ProgressionState.Idle)
                        {
                            _transform.position = new Vector3(_transform.position.x, _transform.position.y, _maxBorderPosition);
                        }
                        else
                        {
                            MoveTowardsTarget(_targetPositionInGroup);
                        }

                        break;
                }
            }
        }

        public void SetHorizontalAndBorder(BridgeDirection direction, float maxBorder, float minBorder)
        {
            _currentDirection = direction;
            _maxBorderPosition = maxBorder;
            _minBorderPosition = minBorder;
        }

        public void SetSpeed(float backwardMovementSpeed)
        {
            _speed = backwardMovementSpeed;
        }

        public void StopMoving()
        {
            _isTargetToPoint = false;
        }

        public void SetTargetPosition(Vector3 position, bool isTargetPoint)
        {
            _targetPositionInGroup = position;
            _isTargetToPoint = isTargetPoint;
        }

        private void MoveTowardsTarget(Vector3 position)
        {
            _transform.localPosition = Vector3.MoveTowards(_transform.localPosition, position, _speed * Time.deltaTime);

            if ((_transform.localPosition - _targetPositionInGroup).sqrMagnitude < _threshold)
            {
                OnReachedRegroupPoint?.Invoke();
            }
        }
    }
}