using UnityEngine;

public class ZombieRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 7f;
    
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void Rotate(Vector3 target)
    {
        Vector3 direction = target - _transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}
