using System;
using System.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private float _delay = 1f;

    public event Action<Chunk> OnFinishedEffect;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        WaitForSeconds delay = new WaitForSeconds(_delay);
        _particleSystem.Play();
        
        yield return delay;
        
        _particleSystem.Stop();
        OnFinishedEffect?.Invoke(this);
    }
}