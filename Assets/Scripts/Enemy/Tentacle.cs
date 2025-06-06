using UnityEngine;

public class Tentacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PoliceOfficer policeOfficer))
        {
            if (policeOfficer.IsDead == false)
            {
                policeOfficer.TakeDamage();
            }
        }
    }
}
