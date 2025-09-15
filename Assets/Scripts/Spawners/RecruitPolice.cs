using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecruitPolice : MonoBehaviour
{
    [SerializeField] private int _minNumber = 1;
    [SerializeField] private int _maxCumulativeIncrease = 6;
    [SerializeField] private int _maxPopulationMultiplier = 4;
    [SerializeField] private float _delay = 2;
    [SerializeField] private TextMeshProUGUI _populationMultiplierText;
    [SerializeField] private TextMeshProUGUI _cumulativeIncreaseText;
    [SerializeField] private RectTransform _leftPositionText;
    [SerializeField] private RectTransform _rightPositionText;
    [SerializeField] private ParticleSystem _leftEffect;
    [SerializeField] private ParticleSystem _rightEffect;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private AudioSource _soundEffect;
    
    private int _cumulativeIncrease;
    private int _populationMultiplier;
    private bool _isMultiplication;

    public event Action<RecruitPolice> OnRecruitPoliceTriggered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PoliceOfficer>(out _))
        {
            OnRecruitPoliceTriggered?.Invoke(this);
        }
    }

    public void SetBonusCount(bool isMultiplication)
    {
        _isMultiplication = isMultiplication;
        _cumulativeIncrease = GetRandomNumberRecruitPolice(_minNumber, _maxCumulativeIncrease);
        _populationMultiplier = GetRandomNumberRecruitPolice(_minNumber, _maxPopulationMultiplier);
        
        if (_isMultiplication)
        {
            var populationMultiplier = Instantiate(_populationMultiplierText, _leftPositionText.position, _leftPositionText.rotation);
            populationMultiplier.transform.SetParent(_leftPositionText);
            var cumulativeIncrease = Instantiate(_cumulativeIncreaseText, _rightPositionText.position, _rightPositionText.rotation);
            cumulativeIncrease.transform.SetParent(_rightPositionText);
            
            populationMultiplier.SetText("X " + Convert.ToString(_populationMultiplier));
            cumulativeIncrease.SetText("+ " + Convert.ToString(_cumulativeIncrease));
        }
        else
        {
            var populationMultiplier = Instantiate(_populationMultiplierText, _rightPositionText.position, _rightPositionText.rotation);
            populationMultiplier.transform.SetParent(_rightPositionText);
            var cumulativeIncrease = Instantiate(_cumulativeIncreaseText, _leftPositionText.position, _leftPositionText.rotation);
            cumulativeIncrease.transform.SetParent(_leftPositionText);
            
            cumulativeIncrease.SetText("+ " + Convert.ToString(_cumulativeIncrease));
            populationMultiplier.SetText("X " + Convert.ToString(_populationMultiplier));
        }
    }
    
    public int GetCountSpawnPoliceOfficers(bool isPositiveX, int policeCount)
    {
        int countSpawn;
        
        _meshRenderer.gameObject.SetActive(false);
        
        if (_isMultiplication)
        {
            if (isPositiveX)
            {
                countSpawn = CalculateSpawnCount(policeCount);
                StartCoroutine(DeactivateAndCleanupEffect(_leftEffect, countSpawn));
            }
            else
            {
                countSpawn = _cumulativeIncrease;
                StartCoroutine(DeactivateAndCleanupEffect(_rightEffect, countSpawn));
            }
        }
        else
        {
            if (isPositiveX)
            {
                countSpawn = _cumulativeIncrease;
                StartCoroutine(DeactivateAndCleanupEffect(_leftEffect, countSpawn));
            }
            else
            {
                countSpawn = CalculateSpawnCount(policeCount);
                StartCoroutine(DeactivateAndCleanupEffect(_rightEffect, countSpawn));
            }
        }

        return countSpawn;
    }

    private IEnumerator DeactivateAndCleanupEffect(ParticleSystem effect, int count)
    {
        WaitForSeconds delay = new WaitForSeconds(_delay);

        effect.Play();

        if (count > 0)
        {
            _soundEffect.Play();
        }

        yield return delay;
        
        effect.Stop();
        
        Destroy(gameObject);
    }
    
    private int CalculateSpawnCount(int policeCount)
    {
        return (policeCount * _populationMultiplier) - policeCount;
    }

    private int GetRandomNumberRecruitPolice(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
