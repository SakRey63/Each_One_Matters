using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 6f;
    [SerializeField] private float _delay = 5f;
    [SerializeField] private int _damage = 50;

    private Transform _transform;
    private Coroutine _coroutineShot;
    
    public event Action<Bullet> OnHit;

    private void Awake()
    {
        _transform = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Zombie zombie))
        {
            zombie.TakeDamage(_damage);
            StopCoroutine(_coroutineShot);
            OnHit?.Invoke(this);
        }
    }

    public void MakeShot()
    {
        _coroutineShot = StartCoroutine(TrackBulletLifecycle());
    }

    private IEnumerator TrackBulletLifecycle()
    {
        float elapsed = 0f;

        while (elapsed < _delay)
        {
            elapsed += Time.deltaTime;
            _transform.Translate(Vector3.forward * _speed * Time.deltaTime, Space.Self);
            
            yield return null;
        }

        OnHit.Invoke(this);
    }
}