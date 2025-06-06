using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecruitPolice : MonoBehaviour
{
    [SerializeField] private int _minNumber = 1;
    [SerializeField] private int _maxCumulativeIncrease = 16;
    [SerializeField] private int _maxPopulationMultiplier = 10;
    [SerializeField] private TextMeshProUGUI _populationMultiplierText;
    [SerializeField] private TextMeshProUGUI _cumulativeIncreaseText;
    [SerializeField] private RectTransform _leftPositionText;
    [SerializeField] private RectTransform _rightPositionText;
    
    private int _cumulativeIncrease;
    private int _populationMultiplier;
    private bool _isMultiplication;

    public event Action<int, bool, RecruitPolice> OnRecruitPoliceTriggered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PoliceOfficer policeOfficer))
        {
            if (policeOfficer.transform.position.x >= 0)
            {
                if (_isMultiplication)
                {
                    OnRecruitPoliceTriggered?.Invoke(_populationMultiplier, true, this);
                }
                else
                {
                    OnRecruitPoliceTriggered?.Invoke(_cumulativeIncrease, false, this);
                }
            }
            else
            {
                if (_isMultiplication == false)
                {
                    OnRecruitPoliceTriggered?.Invoke(_populationMultiplier, true, this);
                }
                else
                {
                    OnRecruitPoliceTriggered?.Invoke(_cumulativeIncrease, false, this);
                }
            }
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

    private int GetRandomNumberRecruitPolice(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
