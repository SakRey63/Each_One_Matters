using UnityEngine;

public class PlayerSideMovement : MonoBehaviour
{
    [SerializeField] private float _borderX = 5.7f;
    [SerializeField] private float _speed = 5f;

    private float _targetPositionX;

    public void Move(float direction)
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x +  (-direction) * _speed * Time.deltaTime, -_borderX, _borderX);
        transform.position = position;
    }
}
