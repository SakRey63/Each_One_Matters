using EachOneMatters.Gameplay.EnemyUnits;
using EachOneMatters.Pool;

namespace EachOneMatters.Generation.Spawners
{
    public class SpawnerZombie : ObjectPool<Zombie>
    {
        public Zombie CreateEnemy()
        {
            Zombie zombie = GetObject();
            zombie.OnZombieDeath += ReturnZombie;
        
            return zombie;
        }

        public void ReturnDemoEnemies(Zombie zombie)
        {
            zombie.OnZombieDeath -= ReturnZombie;
            ReturnObject(zombie);
        }

        private void ReturnZombie(Zombie zombie)
        {
            zombie.OnZombieDeath -= ReturnZombie;
            ReturnObject(zombie);
        }
    }
}