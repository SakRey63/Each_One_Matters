using System;
using UnityEngine;
using YG;
using Random = UnityEngine.Random;

public class BuffSpawner : MonoBehaviour
{
    [SerializeField] private float _verticalPositionRecuruitPolicePoint = 0f;
    [SerializeField] private float _verticalPositionFireRateBoosted = 0.75f;
    [SerializeField] private FireRateBooster _fireRateBooster;
    [SerializeField] private RecruitPolice _recruitPolice;
    
    private int _maxRandomValue = 2;

    public event Action<RecruitPolice> OnPoliceRecruitSpawned;
    public event Action<FireRateBooster> OnFireRateBoostSpawned;

    public void CreateFireRateBooster(Vector3 basePosition, Quaternion targetRotation, Vector3 randomPositionSection)
    {
        basePosition = new Vector3(randomPositionSection.x, _verticalPositionFireRateBoosted, randomPositionSection.z);
        FireRateBooster fireRateBooster = Instantiate(_fireRateBooster, basePosition, targetRotation);
        fireRateBooster.SetDurationTimeImprovedRange(YG2.saves.BuffDuration);
        OnFireRateBoostSpawned?.Invoke(fireRateBooster);
    }

    public void CreateRecruitPolice(SegmentPositionGenerator positionGenerator, Vector3 basePosition, Quaternion targetRotation)
    {
        int randomPositionSquad = GetRandomIndex(0, _maxRandomValue);
                
        basePosition =  positionGenerator.GetPositionCenterLevel(basePosition, _verticalPositionRecuruitPolicePoint);
                
        RecruitPolice recruitPolice = Instantiate(_recruitPolice, basePosition, targetRotation);

        if (randomPositionSquad == 0)
        { 
            recruitPolice.SetBonusCount(false);
        }
        else
        {
            recruitPolice.SetBonusCount(true);
        }
                
        OnPoliceRecruitSpawned?.Invoke(recruitPolice);
    }
    
    private int GetRandomIndex(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}