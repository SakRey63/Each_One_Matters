using System;
using UnityEngine;

public class PlayerBackwardMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    private Transform _transform;
    private Vector3 _targetPosition;
    private float _threshold = 0.01f;

    public float Speed => _speed;
    
    public event Action OnPlayerFinish; 

    private void Awake()
    {
        _transform = transform;
    }

    public void MoveBackward()
    {
        if ((_targetPosition - _transform.position).sqrMagnitude < _threshold * _threshold)
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
