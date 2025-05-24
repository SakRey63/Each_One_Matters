using UnityEngine;

public class FireRateBooster : MonoBehaviour
{
    private bool _isBuffConsumed;
        
    private void OnTriggerEnter(Collider other)
    {
        if (_isBuffConsumed == false && other.TryGetComponent(out PoliceOfficer police))
        {
             _isBuffConsumed = true;
             police.ApplyFireRateBoost();
        }
    }
}
