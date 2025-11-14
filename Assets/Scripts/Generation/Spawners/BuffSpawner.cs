using EachOneMatters.Generation.Boosters;
using UnityEngine;
using YG;
using Random = UnityEngine.Random;

namespace EachOneMatters.Generation.Spawners
{
    public class BuffSpawner : MonoBehaviour
    {
        [SerializeField] private float _verticalPositionRecuruitPolicePoint = 0f;
        [SerializeField] private float _verticalPositionFireRateBoosted = 0.75f;
        [SerializeField] private FireRateBooster _fireRateBooster;
        [SerializeField] private RecruitPolice _recruitPolice;
    
        private int _maxRandomValue = 2;

        public FireRateBooster GetFireRateBooster(Quaternion targetRotation, Vector3 randomPositionSection)
        {
            Vector3 position = new Vector3(randomPositionSection.x, _verticalPositionFireRateBoosted, randomPositionSection.z);
            FireRateBooster fireRateBooster = Instantiate(_fireRateBooster, position, targetRotation);
            fireRateBooster.SetDurationTimeImprovedRange(YG2.saves.upgrades.BuffDuration);
            return fireRateBooster;
        }

        public RecruitPolice GetRecruitPolice(Vector3 basePosition, Quaternion targetRotation)
        {
            int randomPositionSquad = GetRandomIndex(0, _maxRandomValue);

            Vector3 position = new Vector3(basePosition.x, _verticalPositionRecuruitPolicePoint, basePosition.z);
                
            RecruitPolice recruitPolice = Instantiate(_recruitPolice, position, targetRotation);

            recruitPolice.SetBonusCount(randomPositionSquad == 0 ? false : true);

            return recruitPolice;
        }
    
        private int GetRandomIndex(int minValue, int maxValue)
        {
            return Random.Range(minValue, maxValue);
        }
    }
}