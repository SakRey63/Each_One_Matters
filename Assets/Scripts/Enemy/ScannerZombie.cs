using System;
using UnityEngine;

public class ScannerZombie : MonoBehaviour
{
    public event Action OnZombieSighted;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Zombie>(out _))
        {
            OnZombieSighted?.Invoke();
        }
    }
}
