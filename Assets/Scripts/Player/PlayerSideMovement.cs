using UnityEngine;

public class PlayerSideMovement : MonoBehaviour
{
    [SerializeField] private float _borderX = 5.7f;
    [SerializeField] private float _speed = 12f;
    [SerializeField] private Transform _transformSquad;

    private float _targetPositionX;

    public Transform SquadPosition => _transformSquad;

    public void Move(float direction)
    {
        Vector3 position = _transformSquad.localPosition;
        position.x = Mathf.Clamp(position.x +  (-direction) * _speed * Time.deltaTime, -_borderX, _borderX);
        _transformSquad.localPosition = position;
    }
}
