using UnityEngine;

namespace EachOneMatters.Systems
{
    public class KillerObstacleHandler : MonoBehaviour
    {
        [SerializeField] private float _searchRadius = 5f;
        
        public void ProcessObstacles(Transform squadPosition)
        {
            Collider[] hits = Physics.OverlapSphere(squadPosition.position, _searchRadius);

            foreach (Collider collider in hits)
            {
                if (collider.TryGetComponent<IReaction>(out var obstacleHandler))
                {
                    obstacleHandler.HandleInteraction();
                }
            }
        }
    }
}