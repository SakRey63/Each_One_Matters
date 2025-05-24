using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _delay = 5f;

    public event Action<Bullet> OnHit;

    private void OnEnable()
    {
        StartCoroutine(TrackBulletLifecycle());
    }

    private void OnDisable()
    {
        StopCoroutine(TrackBulletLifecycle());
    }

    private void Update()
    {
        Move();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Zombie>())
        {
            other.gameObject.GetComponent<Zombie>().TakeDamage();
        }
        
        OnHit?.Invoke(this);
    }

    private IEnumerator TrackBulletLifecycle()
    {
        WaitForSeconds delay = new WaitForSeconds(_delay);

        yield return delay;

        OnHit.Invoke(this);
    }

    private void Move()
    {
        transform.Translate(Vector3.back * _speed * Time.deltaTime, Space.World);
    }
}
