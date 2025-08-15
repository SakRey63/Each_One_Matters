using System;
using UnityEngine;

public class PointSpawnTrigger : MonoBehaviour
{
    [SerializeField] private Transform _zombieSpawnArea;

    private bool _isHorizontal;

    public bool IsHorizontal => _isHorizontal;
    
    public event Action<PointSpawnTrigger,Transform> OnHordeSpawning;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PoliceOfficer>(out _))
        {
            OnHordeSpawning?.Invoke(this, _zombieSpawnArea);
        }
    }

    public void SetDirection(bool isHorizontal)
    {
        _isHorizontal = isHorizontal;
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
