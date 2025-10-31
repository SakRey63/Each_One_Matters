using System;
using System.Collections;
using UnityEngine;

public class PoliceOfficerVision : MonoBehaviour
{
    [SerializeField] private float _scanRadius = 12f;
    [SerializeField] private float _delay = 0.5f;
    
    private Coroutine _coroutine;

    public event Action<Zombie> OnZombieDetected;

    public void ScanForEnemies()
    {
        if (_coroutine == null && gameObject.activeInHierarchy)
        {
            _coroutine = StartCoroutine(RepeatScanRoutine());
        }
    }
    
    public void ScanOff()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator RepeatScanRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(_delay);

        while (enabled && gameObject.activeInHierarchy)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius);
       
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out Zombie zombie) && !zombie.IsKilledByBullet)
                {
                    OnZombieDetected?.Invoke(zombie);
                    break; 
                }
            }

            yield return delay;
        }

        _coroutine = null;
    }
}
