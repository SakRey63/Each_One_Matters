using System;
using EachOneMatters.Common;
using EachOneMatters.Gameplay.PlayerUnits;
using EachOneMatters.Systems;
using UnityEngine;

namespace EachOneMatters.Generation.Bridge
{
    public class SegmentDamagedBridge : BridgeObject, IReaction
    {
        [SerializeField] private Transform _centrPoint;

        private int _number;
        
        public event Action<SegmentDamagedBridge, Transform, int> OnDestroyDamageSegmentBridge;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PoliceOfficer policeOfficer))
            {
                if (policeOfficer.Status == UnitStatus.Alive)
                {
                    policeOfficer.SetCenterPoint(_centrPoint);
                }
            }
        }

        public void SetStatus(int numberPosition)
        {
            _number = numberPosition;
        }

        public void HandleInteraction()
        {
            OnDestroyDamageSegmentBridge?.Invoke(this, transform, _number);
            Destroy(gameObject);
        }
    }
}