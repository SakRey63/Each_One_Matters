using System;
using UnityEngine;

public class PoliceOfficerMovement : MonoBehaviour
{
    private float _maxBorderPosition;
    private float _minBorderPosition;
    private float _speed;
    private Transform _transform;
    private Vector3 _targetPositionInGroup;
    private bool _isTargetToPoint;
    private bool _isHorizontal;
    private float _threshold = 0.01f;

    public float Speed => _speed;

    public event Action OnReachedRegroupPoint;
    
    private void Awake()
    {
        _transform = transform;
    }

    public void Move()
    {
        if (_isTargetToPoint)
        {
            if (_isHorizontal)
            {
                if (_transform.position.z < _minBorderPosition)
                { 
                    _transform.position = new Vector3(_transform.position.x, _transform.position.y, _minBorderPosition);
                }
                else if (_transform.position.z > _maxBorderPosition)
                {
                    _transform.position = new Vector3(_transform.position.x, _transform.position.y, _maxBorderPosition);
                }
                else
                {
                    if ((_transform.localPosition - _targetPositionInGroup).sqrMagnitude < _threshold * _threshold)
                    {
                        OnReachedRegroupPoint?.Invoke();
                    }                                                                                                                                                                                                                                                                                                                                                               
                    else
                    {
                        MoveTowardsTarget(_targetPositionInGroup);
                    }
                }
            }
            else
            {
                if (_transform.position.x < _minBorderPosition)
                {
                    _transform.position = new Vector3(_minBorderPosition, _transform.position.y, _transform.position.z);
                }
                else if (_transform.position.x > _maxBorderPosition)
                {
                    _transform.position = new Vector3(_maxBorderPosition, _transform.position.y, _transform.position.z);
                }
                else
                {
                    if ((_transform.localPosition - _targetPositionInGroup).sqrMagnitude < _threshold * _threshold)
                    {
                        OnReachedRegroupPoint?.Invoke();
                    }                                                                                                                                                                                                                                                                                                                                                               
                    else
                    {
                        MoveTowardsTarget(_targetPositionInGroup);
                    }
                }
            }
        }
    }

    public void SetHorizontalAndBorder(bool isHorizontal, float maxBorder, float minBorder)
    {
        _isHorizontal = isHorizontal;
        _maxBorderPosition = maxBorder;
        _minBorderPosition = minBorder;
    }
    
    public void SetSpeed(float backwardMovementSpeed)
    {
        _speed = backwardMovementSpeed;
    }

    public void StopMove(bool isTargetToPoint)
    {
        _isTargetToPoint = isTargetToPoint;
    }
    
    public void SetTargetPosition(Vector3 position, bool isTargetPoint)
    {
        _targetPositionInGroup = position;
        _isTargetToPoint = isTargetPoint;
    }
    
    private void MoveTowardsTarget(Vector3 position)
    {
        _transform.localPosition = Vector3.MoveTowards(_transform.localPosition, 
            position, _speed * Time.deltaTime);
    }
}
