using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BridgeGenerator : MonoBehaviour
{
    [SerializeField] private int _zSectionOffset = 4;
    [SerializeField] private int _xSectionOffset = 4;
    [SerializeField] private int _indexLevelSpawnObject = 3;
    [SerializeField] private int _maxIndexZombieWaveTrigget = 7;
    [SerializeField] private int _countBridgeObstacle = 2;
    [SerializeField] private  int _laneCount = 3;
    [SerializeField] private float _monsterOffset = 3.5f;
    [SerializeField] private float _heateFireBoosterTrigger = 0.75f;
    [SerializeField] private float _zBaseOffset = 13;
    [SerializeField] private Transform _startPositionBridgeSegments;
    [SerializeField] private Transform _startPositionAllTriggers;
    [SerializeField] private Transform _righePositionMonster;
    [SerializeField] private Transform _positionZombieWaveTrigger;
    [SerializeField] private SegmentNormalBrige _normalBrige;
    [SerializeField] private PointSpawnTrigger _pointSpawnPrefab;
    [SerializeField] private FireRateBooster _fireRateBooster;
    [SerializeField] private Monster _monster;
    [SerializeField] private Base _base;
    [SerializeField] private RecruitPolice _recruitPolice;
    [SerializeField] private GameObject[] _bridgeObstacles;
    [SerializeField] private GameObject[] _playerUpgrades;
    
    private float _countLevel;
    private int _indexZombieWaveTrigger;
    private int _indexLevel;
    private int _indexSpawnObject;
    private bool _isDamagedSegment;
    private bool _isMonsterActive;
    private bool _isFireRateBoosted;
    private bool _isRecruitedPolice;
    private float _randomXSectionOffset;
    private int _minRandomValue = -1;
    private int _maxRandomValue = 2;
    private int _randomFirstSpawnObject;
    private GameObject _objectSpawnOfBridge;

    public event Action<PointSpawnTrigger> OnPointSpawnedTrigger;
    public event Action<FireRateBooster> OnFireRateBoostedCreated;
    public event Action<RecruitPolice> OnRecruitPoliceCreated;

    public void Generate(float lengthBridge)
    {
        _randomFirstSpawnObject = GetRandomIndex(0, _maxRandomValue);
        
        _countLevel = lengthBridge / _zSectionOffset;

        Vector3 nextSpawnPositionZ = _startPositionBridgeSegments.position;
        
        CreateZombieWaveTrigger(nextSpawnPositionZ);
        
        for (int i = 0; i < _countLevel; i++)
        {
            _indexLevel++;

            if (_indexLevel == _indexLevelSpawnObject)
            {
                _indexLevel = 0;
                ChoseObjectToBridge();
            }

            if (_indexZombieWaveTrigger >= _maxIndexZombieWaveTrigget)
            {
                CreateZombieWaveTrigger(nextSpawnPositionZ);
            }
            
            CreateLevel(nextSpawnPositionZ);
            
            nextSpawnPositionZ = new Vector3(nextSpawnPositionZ.x, nextSpawnPositionZ.y,
                nextSpawnPositionZ.z + _zSectionOffset);
            
            _indexZombieWaveTrigger++;
        }

        nextSpawnPositionZ = new Vector3(_startPositionAllTriggers.position.x, _startPositionAllTriggers.position.y, nextSpawnPositionZ.z + _zBaseOffset);

        Instantiate(_base, nextSpawnPositionZ, quaternion.identity);
    }

    private void ChoseObjectToBridge()
    {
        if (_randomFirstSpawnObject > 0)
        {
            _indexSpawnObject++;
            _objectSpawnOfBridge = GetObjectBridge(_bridgeObstacles);
           
            if (_indexSpawnObject == _countBridgeObstacle)
            {
                _randomFirstSpawnObject--;
                _indexSpawnObject = 0;
            }
        }
        else
        {
            _objectSpawnOfBridge = GetObjectBridge(_playerUpgrades);
            _randomFirstSpawnObject++;
        }
        
        _randomXSectionOffset = _xSectionOffset * GetRandomIndex(_minRandomValue, _maxRandomValue);

        if (_objectSpawnOfBridge.TryGetComponent<SegmentDamagedBridge>(out _))
        {
            _isDamagedSegment = true;
        }
        else if(_objectSpawnOfBridge.TryGetComponent<Monster>(out _))
        {
            _isMonsterActive = true;
        }
        else if (_objectSpawnOfBridge.TryGetComponent<FireRateBooster>(out _))
        {
            _isFireRateBoosted = true;
        }
        else if(_objectSpawnOfBridge.TryGetComponent<RecruitPolice>(out _))
        {
            _isRecruitedPolice = true;
        }
    }

    private GameObject GetObjectBridge(GameObject[] objectsBridge)
    {
        return objectsBridge[Random.Range(0, objectsBridge.Length)];
    }

    private void CreateZombieWaveTrigger(Vector3 position)
    {
        _indexZombieWaveTrigger = 0;
        
        position = new Vector3(_positionZombieWaveTrigger.position.x, _positionZombieWaveTrigger.position.y, position.z);
        var waveTrigger = Instantiate(_pointSpawnPrefab, position, quaternion.identity);
        
        OnPointSpawnedTrigger?.Invoke(waveTrigger);
    }

    private void CreateLevel(Vector3 position)
    {
        for (int j = 0; j < _laneCount; j++)
        {
            CreateSegmentBridge(position);
            
            if (_randomXSectionOffset == position.x)
            { 
                SpawnMonster(position);
                SpawnPowerUp(position);
            }
            
            position = new Vector3(position.x + _xSectionOffset, position.y, position.z);
        }
    }

    private void CreateSegmentBridge(Vector3 positionSegment)
    {
        if (_isDamagedSegment && positionSegment.x == _randomXSectionOffset)
        {
            _isDamagedSegment = false;
            Instantiate(_objectSpawnOfBridge, positionSegment, quaternion.identity);
        }
        else
        {
            Instantiate(_normalBrige, positionSegment, quaternion.identity);
        }
    }
    
    private void SpawnPowerUp(Vector3 basePosition)
    {
        if (_isFireRateBoosted || _isRecruitedPolice)
        {
            if (_isFireRateBoosted)
            {
                basePosition = new Vector3(basePosition.x, _heateFireBoosterTrigger, basePosition.z);
                _isFireRateBoosted = false;
                
                var fireRateBooster = Instantiate(_fireRateBooster, basePosition, quaternion.identity);
                
                OnFireRateBoostedCreated?.Invoke(fireRateBooster);
            }
            
            if (_isRecruitedPolice)
            {
                int randomPositionSquad = GetRandomIndex(0, _maxRandomValue);
                
                basePosition = new Vector3(_startPositionAllTriggers.position.x, _startPositionAllTriggers.position.y, basePosition.z);
                _isRecruitedPolice = false;
                
                var recruitPolice = Instantiate(_recruitPolice, basePosition, quaternion.identity);

                if (randomPositionSquad == 0)
                {
                    recruitPolice.SetBonusCount(false);
                }
                else
                {
                    recruitPolice.SetBonusCount(true);
                }
                
                OnRecruitPoliceCreated?.Invoke(recruitPolice);
            }
        }
    }

    private void SpawnMonster(Vector3 positionMonster)
    {
        if (_isMonsterActive)
        {
            float zOffset = 2.5f;
            
            _isMonsterActive = false;

            Monster monster = Instantiate(_monster);

            if (_randomXSectionOffset < 0)
            {
                monster.transform.position = new Vector3(_righePositionMonster.position.x, _righePositionMonster.position.y,
                    positionMonster.z);
                monster.transform.rotation = _righePositionMonster.rotation;
            }
            else
            {
                monster.transform.position = new Vector3(_xSectionOffset + _monsterOffset, positionMonster.y, positionMonster.z);
                zOffset = -zOffset;
            } 
            
            monster.SetPositionScannerZombie(zOffset);
        }
    }

    private int GetRandomIndex(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
