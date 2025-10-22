using System;
using System.Collections;
using UnityEngine;

public class ZombieVision : MonoBehaviour
{
    [SerializeField] private Transform _sphereCenter;
    [SerializeField] private float _scanRadius = 12f;
    [SerializeField] private float _delay = 0.5f;
    
    private Coroutine _coroutine;

    public event Action<PoliceOfficer> OnPoliceDetected;

    public void ScanForEnemies()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        
        _coroutine = StartCoroutine(RepeatScanRoutine());
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

        while (true)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<PoliceOfficer>(out var police) && !police.IsDead)
                {
                    OnPoliceDetected?.Invoke(police);
                    yield break;
                }
            }

            yield return delay;
        }
    }
}
