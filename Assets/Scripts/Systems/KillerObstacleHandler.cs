using System;
using EachOneMatters.Common;
using EachOneMatters.Gameplay.EnemyUnits;
using EachOneMatters.Gameplay.PlayerUnits;
using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Systems
{
    public class KillerObstacleHandler : MonoBehaviour
    {
        [SerializeField] private float _searchRadius = 5f;
        [SerializeField] private int _damageZombie = 100;

        public event Action<Transform, int> OnDestroyDamageSegmentBridge;

        public void ProcessObstacles(Transform squadPosition)
        {
            Collider[] hits = Physics.OverlapSphere(squadPosition.position, _searchRadius);

            foreach (Collider collider in hits)
            {
                if (collider.TryGetComponent<IBridgeObject>(out var bridgeObject))
                {
                    ProcessBridgeObject(bridgeObject, collider, hits);
                }
            }
        }

        private void ProcessBridgeObject(IBridgeObject bridgeObject, Collider collider, Collider[] hits)
        {
            switch (bridgeObject.Type)
            {
                case BridgeObjectType.ScannerObstacle:
                    HandleScannerObstacle(collider);
                    break;

                case BridgeObjectType.Zombie:
                    HandleZombieObstacle(collider);
                    break;

                case BridgeObjectType.DamagedSegment:
                    HandleDamagedSegmentObstacle(collider, hits);
                    break;
            }
        }

        private void HandleScannerObstacle(Collider collider)
        {
            Destroy(collider.gameObject);
        }

        private void HandleZombieObstacle(Collider collider)
        {
            if (collider.TryGetComponent(out Zombie zombie))
            {
                zombie.TakeDamage(_damageZombie, UnitStatus.Dead);
            }
        }

        private void HandleDamagedSegmentObstacle(Collider collider, Collider[] hits)
        {
            foreach (Collider police in hits)
            {
                if (police.TryGetComponent(out PoliceOfficer policeOfficer))
                {
                    policeOfficer.OnDeathAnimationComplete();
                }
            }

            Transform transform = collider.transform;

            if (collider.TryGetComponent(out SegmentDamagedBridge damagedBridge))
            {
                int number = damagedBridge.NumberPosition;
                Destroy(collider.gameObject);

                OnDestroyDamageSegmentBridge?.Invoke(transform, number);
            }
        }
    }
}