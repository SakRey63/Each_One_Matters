using EachOneMatters.Common;
using EachOneMatters.Gameplay.PlayerUnits;
using EachOneMatters.Systems;
using UnityEngine;

namespace EachOneMatters.Generation.Obstacle
{
    public class ScannerObstacle : MonoBehaviour, IReaction
    {
        [SerializeField] private int _damage = 100;
        [SerializeField] private AudioSource _sound;
    
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

        public void HandleInteraction()
        {
            Destroy(gameObject);
        }
    }
}