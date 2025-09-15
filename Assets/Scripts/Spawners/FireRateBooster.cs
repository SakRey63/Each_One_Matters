using System;
using System.Collections;
using UnityEngine;

public class FireRateBooster : MonoBehaviour
{
    [SerializeField] private ParticleSystem _buffEffect;
    [SerializeField] private ParticleSystem _constEffect;
    [SerializeField] private ParticleSystem _squadEffect;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private AudioSource _soundEffect;
    [SerializeField] private float _delay = 2;

    private bool _isBuffConsumed;

    public event Action<FireRateBooster, ParticleSystem> OnFirstOfficerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (_isBuffConsumed == false && other.TryGetComponent<PoliceOfficer>(out _))
        {
            OnFirstOfficerEntered?.Invoke(this, _squadEffect);
            _soundEffect.Play();
            _isBuffConsumed = true;
            StartCoroutine(HandleBuffEffect());
        }
    }

    private IEnumerator HandleBuffEffect()
    {
        WaitForSeconds delay = new WaitForSeconds(_delay);
        _meshRenderer.enabled = false;
        _constEffect.Stop();
        _buffEffect.Play();
        yield return delay;
        _buffEffect.Stop();
        Destroy(gameObject);
    }
}