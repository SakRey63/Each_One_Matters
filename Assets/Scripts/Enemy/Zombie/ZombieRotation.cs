using UnityEngine;

public class ZombieRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 10f;
    
    private Transform _transform;
    private float _threshold = 0.001f;

    private void Awake()
    {
        _transform = transform;
    }

    public void Rotate(Transform target)
    {
        if (target == null) return;

        Vector3 direction = target.position - _transform.position;

        if (direction.sqrMagnitude < _threshold) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}