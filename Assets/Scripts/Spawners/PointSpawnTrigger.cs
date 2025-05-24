using System;
using UnityEngine;

public class PointSpawnTrigger : MonoBehaviour
{
    [SerializeField] private Transform _zombieSpawnArea;
    
    public event Action<Transform> OnHordeSpawning;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PoliceOfficer>(out _))
        {
            OnHordeSpawning?.Invoke(_zombieSpawnArea);
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if(collider == null)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix; 
        Gizmos.DrawWireCube(collider.center, collider.size);
    }
}
