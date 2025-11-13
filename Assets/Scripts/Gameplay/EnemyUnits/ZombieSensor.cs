using System;
using UnityEngine;
using EachOneMatters.Common;
using EachOneMatters.Gameplay.PlayerUnits;
using EachOneMatters.Generation.Bridge;

namespace EachOneMatters.Gameplay.EnemyUnits
{
    public class ZombieSensor : MonoBehaviour
    {
        public event Action OnBridgeDestroyedDetected;
        public event Action<PoliceOfficer> OnPoliceContact;
        public event Action<Base> OnBaseDetected;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<SegmentDamagedBridge>(out _))
            {
                OnBridgeDestroyedDetected?.Invoke();
            }
            else if (other.TryGetComponent(out PoliceOfficer policeOfficer))
            {
                if (policeOfficer.Status == UnitStatus.Alive)
                {
                    OnPoliceContact?.Invoke(policeOfficer);
                }
            }
            else if (other.TryGetComponent(out Base policeBase))
            {
                OnBaseDetected?.Invoke(policeBase);
            }
        }
    }
}