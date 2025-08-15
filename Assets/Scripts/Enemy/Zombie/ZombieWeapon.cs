using System;
using System.Collections;
using UnityEngine;

public class ZombieWeapon : MonoBehaviour
{
    [SerializeField] private int _damage = 75;
    [SerializeField] private float _durationAttack = 0.4f;

    private Coroutine _coroutine;

    public event Action OnAttackFinished;
    
    public void Attack(PoliceOfficer policeOfficer)
    {
        if (_coroutine == null && policeOfficer.IsDead == false)
        {
            _coroutine = StartCoroutine(PerformAttack(policeOfficer));
        }
    }

    private IEnumerator PerformAttack(PoliceOfficer policeOfficer)
    {
        float duration = 0;
        policeOfficer.TakeDamage(_damage);
        
        while (duration < _durationAttack)
        {
            duration += Time.deltaTime;
            
            yield return null;
        }

        OnAttackFinished?.Invoke();
        
        _coroutine = null;
    }
}
