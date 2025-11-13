using System;
using EachOneMatters.Common;
using EachOneMatters.Gameplay.PlayerUnits;
using UnityEngine;

namespace EachOneMatters.Gameplay.EnemyUnits
{
    public class ZombieWeapon : MonoBehaviour
    {
        [SerializeField] private int _damage = 75;
    
        public event Action<bool> OnAttackFinished; 
    
        public void Attack(PoliceOfficer policeOfficer)
        {
            bool isKilledUnit = false;
        
            if (policeOfficer.Status == UnitStatus.Alive)
            {
                policeOfficer.TakeDamage(_damage);
                isKilledUnit = true;
            }
        
            OnAttackFinished?.Invoke(isKilledUnit);
        }
    }
}