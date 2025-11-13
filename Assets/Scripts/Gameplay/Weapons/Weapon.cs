using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform _bulletSpawnPosition;

    private BulletPool _bulletPool;
    private Coroutine _coroutineRepeatShooting;
    private PoliceOfficerSound _officerSound;

    public void SetBulletPool(BulletPool bulletPool, PoliceOfficerSound sound)
    {
        _bulletPool = bulletPool;
        _officerSound = sound;
    }

    public void Shooting(float repeat)
    {
        if (_coroutineRepeatShooting != null)
        {
            StopCoroutine(_coroutineRepeatShooting);
        }

        _coroutineRepeatShooting = StartCoroutine(RepeatShooting(repeat));
    }
    
    public void StopShooting()
    {
        if (_coroutineRepeatShooting != null)
        {
            StopCoroutine(_coroutineRepeatShooting);
        }
    }
    
    private IEnumerator RepeatShooting(float repeatShoot)
    {
        WaitForSeconds delay = new WaitForSeconds(repeatShoot);
        
        if (_bulletPool != null && _officerSound != null)
        {
            while (enabled)
            {
                Bullet bullet = _bulletPool.GetBullet();
                bullet.transform.position = _bulletSpawnPosition.position;
                bullet.transform.rotation = _bulletSpawnPosition.rotation;
                bullet.MakeShot();
                _officerSound.PlayShoot();
                
                yield return delay;
            }
        }
    }
}
