using System;
using System.Collections;
using UnityEngine;

public class PoliceOfficer : MonoBehaviour
{
    private Weapon _weapon;
    private BulletPool _bulletPool;
    private float _repeatShoot;

    public event Action OnFireRateBoosted;
    
    private void Awake()
    {
        _bulletPool = GetComponent<BulletPool>();
        _weapon = GetComponentInChildren<Weapon>();
    }

    public void Shooting(float repeat)
    {
        _repeatShoot = repeat;
        
        StartCoroutine(RepeatShooting());
    }
    
    public void BoostFireRate(float repeat)
    {
        _repeatShoot = repeat;
    }

    public void ApplyFireRateBoost()
    {
        OnFireRateBoosted?.Invoke();
    }

    private IEnumerator RepeatShooting()
    {
        while (enabled)
        {
            _bulletPool.GetBullet(_weapon.BulletSpawnPosition);
            
            yield return new WaitForSeconds(_repeatShoot);
        }
    }
}
