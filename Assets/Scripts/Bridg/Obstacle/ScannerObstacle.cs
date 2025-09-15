using UnityEngine;

public class ScannerObstacle : MonoBehaviour
{
    [SerializeField] private int _damage = 100;
    [SerializeField] private AudioSource _sound;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PoliceOfficer policeOfficer))
        {
            if (policeOfficer.IsDead == false)
            {
                _sound.Play();
                policeOfficer.TakeDamage(_damage);
            }
        }
    }
}