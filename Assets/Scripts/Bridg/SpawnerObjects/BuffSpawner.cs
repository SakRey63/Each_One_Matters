using System;
using UnityEngine;
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
    
    public void ConditionalSpawnBuffs(SegmentPositionGenerator positionGenerator, BridgeObjectSelector objectSelector, Vector3 basePosition, Quaternion targetRotation, Vector3 randomPositionSection)
    {
        if (objectSelector.IsFireRateBoosted)
        { 
            if (positionGenerator.IsHorizontatl == false && randomPositionSection.x == basePosition.x || positionGenerator.IsHorizontatl && randomPositionSection.z == basePosition.z)
            {
                basePosition = new Vector3(basePosition.x, _verticalPositionFireRateBoosted, basePosition.z);
                FireRateBooster fireRateBooster = Instantiate(_fireRateBooster, basePosition, targetRotation);
                OnFireRateBoostSpawned?.Invoke(fireRateBooster);
                
                objectSelector.ResetAllStats();
            }
        }
            
        if (objectSelector.IsRecruitedPolice)
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
            
            objectSelector.ResetAllStats();
        }
    }
    
    private int GetRandomIndex(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
