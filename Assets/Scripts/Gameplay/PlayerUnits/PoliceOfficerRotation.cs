using EachOneMatters.Gameplay.EnemyUnits;
using UnityEngine;

namespace EachOneMatters.Gameplay.PlayerUnits
{
    public class PoliceOfficerRotation : MonoBehaviour
    {
        [SerializeField] private Transform _basePoint;
        [SerializeField] private float _rotationSpeed = 8f;
    
        private Zombie _zombie;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void Rotation()
        {
            if (_zombie != null)
            {
                Vector3 direction = _zombie.transform.position - _transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }

        public void SetTarget(Zombie zombie)
        {
            _zombie = zombie;
        }
    }
}