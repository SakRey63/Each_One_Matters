using UnityEngine;

public class Tentacle : MonoBehaviour
{
    [SerializeField] private int _damage = 100;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PoliceOfficer policeOfficer))
        {
            if (policeOfficer.IsDead == false)
            {
                policeOfficer.TakeDamage(_damage);
            }
        }
    }
}
