using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private int _damage = 100;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Zombie zombie) && !zombie.IsKilledByBullet)
        {
            zombie.TakeDamage(_damage, false);
        }
    }
}