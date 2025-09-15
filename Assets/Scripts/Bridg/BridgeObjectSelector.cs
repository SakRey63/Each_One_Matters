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
    private bool _isHammerActive;
    private bool _isFireRateBoosted;
    private bool _isRecruitedPolice;
    private bool _isRotatingBlade;
    private bool _isSawBlade;
    private bool _isSpikedCylinder;
    private bool _isSpikePress;
    private bool _isSpikes;
    private bool _isPlaced;

    public bool IsDamagedSegment => _isDamagedSegment;
    public bool IsFireRateBoosted => _isFireRateBoosted;
    public bool IsRecruitedPolice => _isRecruitedPolice;
    public bool IsHammerActive => _isHammerActive;
    public bool IsRotatingBlade => _isRotatingBlade;
    public bool IsSawBlade => _isSawBlade;
    public bool IsSpikedCylinder => _isSpikedCylinder;
    public bool IsSpikePress => _isSpikePress;
    public bool IsSpikes => _isSpikes;
    public bool IsPlaced => _isPlaced;

    public void ChoseObjectToBridge()
    {
        if (_randomFirstSpawnObject > 0)
        {
            _indexSpawnObject++;
            _objectSpawnOfBridge = GetRandomObjectBridge(_bridgeObstacles);
           
            if (_indexSpawnObject == _countBridgeObstacle)
            {
                _randomFirstSpawnObject--;
                _indexSpawnObject = 0;
            }
        }
        else
        {
            _objectSpawnOfBridge = GetRandomObjectBridge(_playerUpgrades);
            _randomFirstSpawnObject++;
        }
        
        if (_objectSpawnOfBridge.TryGetComponent<SegmentDamagedBridge>(out _))
        {
            _isDamagedSegment = true;
        }
        else if (_objectSpawnOfBridge.TryGetComponent<FireRateBooster>(out _))
        {
            _isFireRateBoosted = true;
        }
        else if(_objectSpawnOfBridge.TryGetComponent<RecruitPolice>(out _))
        {
            _isRecruitedPolice = true;
        }
        else if (_objectSpawnOfBridge.TryGetComponent<Hammer>(out _))
        {
            _isHammerActive = true;
        }
        else if (_objectSpawnOfBridge.TryGetComponent<RotatingBlade>(out _))
        {
            _isRotatingBlade = true;
        }
        else if (_objectSpawnOfBridge.TryGetComponent<SawBlade>(out _))
        {
            _isSawBlade = true;
        }
        else if (_objectSpawnOfBridge.TryGetComponent<SpikedCylinder>(out _))
        {
            _isSpikedCylinder = true;
        }
        else if (_objectSpawnOfBridge.TryGetComponent<SpikePress>(out _))
        {
            _isSpikePress = true;
        }
        else if (_objectSpawnOfBridge.TryGetComponent<Spikes>(out _))
        {
            _isSpikes = true;
        }

        _isPlaced = true;
    }

    public void ResetAllStats()
    {
        _isDamagedSegment = false;
        _isFireRateBoosted = false;
        _isRecruitedPolice = false;
        _isHammerActive = false;
        _isRotatingBlade = false;
        _isSawBlade = false;
        _isSpikedCylinder = false;
        _isSpikePress = false;
        _isSpikes = false;
        _isPlaced = false;
    }
    
    public void CreateFirstSpawnObject()
    {
        _randomFirstSpawnObject = GetRandomIndex(0, _maxRandomValue);
    }
    
    private GameObject GetRandomObjectBridge(GameObject[] objectsBridge)
    {
        return objectsBridge[Random.Range(0, objectsBridge.Length)];
    }
    
    private int GetRandomIndex(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
