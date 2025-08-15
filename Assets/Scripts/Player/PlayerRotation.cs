using System.Collections;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private float _duration = 2.5f;
    [SerializeField] private float _currentAngle = 90f;
    
    private Transform _transform;
    private Coroutine _coroutine;

    private void Awake()
    {
        _transform = transform;
    }

    public void Rotate(bool turnRight)
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(PerformRotation(turnRight));
        }
    }

    private IEnumerator PerformRotation(bool turnRight)
    {
        float targetAngle;
        
        float currentYAngle = _transform.eulerAngles.y;
        
        if (turnRight == false)
        {
            targetAngle = currentYAngle -_currentAngle;
        }
        else
        {
            targetAngle = currentYAngle + _currentAngle;
        }
        
        targetAngle = Mathf.Repeat(targetAngle, 360f);
        
        float elapsed = 0f;

        Quaternion startRotation = _transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, targetAngle, 0);

        while (elapsed < _duration)
        {
            float t = elapsed / _duration;
            _transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        _transform.rotation = endRotation;

        _coroutine = null;
    }
}
