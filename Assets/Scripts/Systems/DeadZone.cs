using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private int _damage = 100;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Zombie zombie) && zombie.Status == UnitStatus.Alive)
        {
            zombie.TakeDamage(_damage, UnitStatus.Dead);
        }
    }
}