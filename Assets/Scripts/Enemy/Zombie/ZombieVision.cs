using System;
using System.Collections;
using UnityEngine;

public class ZombieVision : MonoBehaviour
{
    [SerializeField] private Transform _sphereCenter;
    [SerializeField] private float _scanRadius = 20f;
    [SerializeField] private float _delay = 0.5f;
    
    private bool _isTargetDetected;
    private Coroutine _coroutine;

    public event Action<PoliceOfficer> OnPoliceDetected;

    public void ScanForEnemies()
    {
        if (_coroutine == null)
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

        bool isEnemyFound = true;

        while (isEnemyFound)
        {
            _isTargetDetected = false;
        
            Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius);
       
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out PoliceOfficer policeOfficer))
                {
                    DetectPoliceOfficer(policeOfficer);
                }

                if (_isTargetDetected)
                {
                    isEnemyFound = false;
                    break;
                }
            }

            yield return delay;
        }

        _coroutine = null;
    }

    private void DetectPoliceOfficer(PoliceOfficer policeOfficer)
    {
        if (policeOfficer.IsDead == false)
        {
            _isTargetDetected = true; 
            OnPoliceDetected?.Invoke(policeOfficer);
        }
    }
}
