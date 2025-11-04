using UnityEngine;

public class ScannerObstacle : MonoBehaviour, IBridgeObject
{
    [SerializeField] private int _damage = 100;
    [SerializeField] private AudioSource _sound;

    public BridgeObjectType Type => BridgeObjectType.ScannerObstacle;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PoliceOfficer policeOfficer))
        {
            if (policeOfficer.Status == UnitStatus.Alive)
            {
                _sound.Play();
                policeOfficer.TakeDamage(_damage);
            }
        }
    }
}