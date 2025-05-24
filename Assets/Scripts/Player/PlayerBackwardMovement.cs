using UnityEngine;

public class PlayerBackwardMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    private void Update()
    {
        MoveBackward();
    }

    private void MoveBackward()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime, Space.World);
    }
}
