using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float _speed = 7;

    private bool _isHitConfirmed;

    public event Action<Zombie> OnZombieDeath;
    private void Update()
    {
        Move();
    }

    public void TakeDamage()
    {
        if (_isHitConfirmed)
        {
            _isHitConfirmed = false;
            
            OnZombieDeath?.Invoke(this);
        }
        else
        {
            _isHitConfirmed = true;
        }
    }

    private void Move()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime, Space.World);
    }
}
