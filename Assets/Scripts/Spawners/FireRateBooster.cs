using System;
using UnityEngine;

public class FireRateBooster : MonoBehaviour
{
    private bool _isBuffConsumed;

    public event Action<FireRateBooster> OnFirstOfficerEntered; 
        
    private void OnTriggerEnter(Collider other)
    {
        if (_isBuffConsumed == false && other.TryGetComponent<PoliceOfficer>(out _))
        {
            OnFirstOfficerEntered?.Invoke(this);
            _isBuffConsumed = true;
        }
    }
}
