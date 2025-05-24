using UnityEngine;

public class BulletPool : ObjectPool<Bullet>
{
    public void GetBullet(Transform weaponBulletSpawnPosition)
    {
        Bullet bullet = GetObject();
        bullet.transform.position = weaponBulletSpawnPosition.position;
        bullet.OnHit += ReturnBullet;
    }

    private void ReturnBullet(Bullet bullet)
    {
        bullet.OnHit -= ReturnBullet;
        ReturnObject(bullet);
    }
}
