using System;
using System.Collections;
using UnityEngine;

public class FireRateBooster : MonoBehaviour, IBridgeObject
{
    [SerializeField] private ParticleSystem _buffEffect;
    [SerializeField] private ParticleSystem _constEffect;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private AudioSource _soundEffect;
    [SerializeField] private float _delayEffect = 2;
    [SerializeField] private float _increasedRateOfFire = 0.4f;

    private float _buffDuration;
    private Coroutine _handleBuffEffect;

    public BridgeObjectType Type => BridgeObjectType.FireRateBooster;
    public float BuffDuration => _buffDuration;
    public float IncreasedRateOfFire => _increasedRateOfFire;
    
    public event Action<FireRateBooster> OnFirstOfficerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PoliceOfficer>(out _))
        {
            OnFirstOfficerEntered?.Invoke(this);

            if (_handleBuffEffect == null)
            {
                _handleBuffEffect = StartCoroutine(HandleBuffEffect());
            }
        }
    }

    public void SetDurationTimeImprovedRange(float buffDuration)
    {
        _buffDuration = buffDuration;
    }

    private IEnumerator HandleBuffEffect()
    {
        WaitForSeconds delay = new WaitForSeconds(_delayEffect);
        _soundEffect.Play();
        _meshRenderer.enabled = false;
        _constEffect.Stop();
        _buffEffect.Play();
        yield return delay;
        _buffEffect.Stop();
        Destroy(gameObject);
    }
}