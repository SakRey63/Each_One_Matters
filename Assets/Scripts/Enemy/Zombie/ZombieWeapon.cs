using System;
using UnityEngine;

public class ZombieWeapon : MonoBehaviour
{
    [SerializeField] private int _damage = 75;
    
    public event Action<bool> OnAttackFinished; 
    
    public void Attack(PoliceOfficer policeOfficer)
    {
        bool isKilledUnit = false;
        
        if (policeOfficer.IsDead == false)
        {
            policeOfficer.TakeDamage(_damage);
            isKilledUnit = true;
        }
        
        OnAttackFinished?.Invoke(isKilledUnit);
    }
}