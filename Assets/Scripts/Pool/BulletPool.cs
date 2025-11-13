using EachOneMatters.Gameplay.Weapons;

namespace EachOneMatters.Pool
{
    public class BulletPool : ObjectPool<Bullet>
    {
        public Bullet GetBullet()
        {
            Bullet bullet = GetObject();
            bullet.OnHit += ReturnBullet;
            return bullet;
        }

        private void ReturnBullet(Bullet bullet)
        {
            bullet.OnHit -= ReturnBullet;
            ReturnObject(bullet);
        }
    }
}