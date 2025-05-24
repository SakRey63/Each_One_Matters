public class ZombiePool : ObjectPool<Zombie>
{
    public Zombie GetZombie()
    {
        Zombie zombie = GetObject();
        zombie.OnZombieDeath += ReturnZombie;
        
        return zombie;
    }
    
    private void ReturnZombie(Zombie zombie)
    {
        zombie.OnZombieDeath -= ReturnZombie;
        ReturnObject(zombie);
    }
}