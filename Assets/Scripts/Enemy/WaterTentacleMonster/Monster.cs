using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private ScannerZombie _scannerZombie;
    [SerializeField] private Transform _rotationCentr;
    [SerializeField] private float _angelAxis = -45f;

    private bool _isUnblocked;

    private void OnEnable()
    {
        _scannerZombie.OnZombieSighted += UnblockPath;
    }

    private void OnDisable()
    {
        _scannerZombie.OnZombieSighted -= UnblockPath;
    }

    public void SetPositionScannerZombie(float zOffset)
    {
        Vector3 positionScanner = _scannerZombie.transform.localPosition;
        positionScanner.z += zOffset;
        _scannerZombie.transform.localPosition = positionScanner;
    }
    
    private void UnblockPath()
    {
        if (_isUnblocked == false)
        {
            _isUnblocked = true;

            StartCoroutine(RaiseBody());
        }
    }

    private IEnumerator RaiseBody()
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Quaternion startRotation = _rotationCentr.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, _angelAxis);

        while (elapsedTime < duration)
        {
            _rotationCentr.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _rotationCentr.rotation = targetRotation;
    }
}
