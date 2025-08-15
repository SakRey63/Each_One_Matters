using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeObjectSelector : MonoBehaviour
{
    [SerializeField] private int _maxRandomValue = 2;
    [SerializeField] private int _countBridgeObstacle = 2;
    [SerializeField] private GameObject[] _bridgeObstacles;
    [SerializeField] private GameObject[] _playerUpgrades;

    private GameObject _objectSpawnOfBridge;
    private int _randomFirstSpawnObject;
    private int _indexSpawnObject;
    private bool _isDamagedSegment;
    private bool _isMonsterActive;
    private bool _isFireRateBoosted;
    private bool _isRecruitedPolice;

    public bool IsDamagedSegment => _isDamagedSegment;
    public bool IsMonsterActive => _isMonsterActive;
    public bool IsFireRateBoosted => _isFireRateBoosted;
    public bool IsRecruitedPolice => _isRecruitedPolice;

    public void ChoseObjectToBridge()
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

    public void ResetAllStats()
    {
        _isDamagedSegment = false;
        _isMonsterActive = false;
        _isFireRateBoosted = false;
        _isRecruitedPolice = false;
    }
    
    public void CreateFirstSpawnObject()
    {
        _randomFirstSpawnObject = GetRandomIndex(0, _maxRandomValue);
    }
    
    private GameObject GetObjectBridge(GameObject[] objectsBridge)
    {
        return objectsBridge[Random.Range(0, objectsBridge.Length)];
    }
    
    private int GetRandomIndex(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
