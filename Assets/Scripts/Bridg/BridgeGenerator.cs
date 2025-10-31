using System;
using UnityEngine;

public class BridgeGenerator : MonoBehaviour
{
    [SerializeField] private int _sectionOffset = 5;
    [SerializeField] private int _indexLevelSpawnObject = 3;
    [SerializeField] private int _maxIndexZombieWaveTrigget = 7;
    [SerializeField] private  int _laneCount = 3;
    [SerializeField] private Transform _startPositionBridgeSegments;
    [SerializeField] private Transform _positionZombieWaveTrigger;

    private BridgeObjectSelector _objectSelector;
    private BridgeLengthCalculator _lengthCalculator;
    private SegmentPositionGenerator _positionGenerator;
    private BridgeCheckpointStore _checkpointStore;
    private SpawnerSegmentBridge _spawnerSegmentBridge;
    private SpawnerZombieWaveTrigger _spawnerZombieWaveTrigger;
    private BuffSpawner _buffSpawner;
    private SpawnerObstacles _spawnerObstacles;
    private int _indexZombieWaveTrigger;
    private Transform _endPositionPlayer;
    private Vector3 _randomPositionSection;
    private Quaternion _targetRotation;
    private GameObject _selectedObject;

    public int BridgeSpanCount => _lengthCalculator.BridgePieceCount;
    public bool IsTurnRight => _positionGenerator.IsFirstTurnedRight;
    public Transform EndPositionPlayer => _endPositionPlayer;

    public event Action<PointSpawnTrigger> OnPointSpawnedTrigger;
    public event Action<FireRateBooster> OnFireRateBoostedCreated;
    public event Action<RecruitPolice> OnRecruitPoliceCreated;
    public event Action<BridgeConnector> OnBridgeConnectorCreated;
    public event Action<Base> OnBasePoliceOfficerCreated; 

    private void Awake()
    {
        _spawnerObstacles = GetComponent<SpawnerObstacles>();
        _buffSpawner = GetComponent<BuffSpawner>();
        _spawnerZombieWaveTrigger = GetComponent<SpawnerZombieWaveTrigger>();
        _spawnerSegmentBridge = GetComponent<SpawnerSegmentBridge>();
        _checkpointStore = GetComponent<BridgeCheckpointStore>();
        _objectSelector = GetComponent<BridgeObjectSelector>();
        _positionGenerator = GetComponent<SegmentPositionGenerator>();
        _lengthCalculator = GetComponent<BridgeLengthCalculator>();
        _targetRotation = _startPositionBridgeSegments.rotation;
    }

    private void OnEnable()
    {
        _buffSpawner.OnPoliceRecruitSpawned += HandlePoliceRecruitSpawned;
        _buffSpawner.OnFireRateBoostSpawned += HandleFireRateBoostSpawned;
        _spawnerSegmentBridge.OnBridgeConnectorSpawned += HandleBridgeConnectorSpawned;
        _spawnerSegmentBridge.OnBaseSpawned += HandleBaseSpawned;
    }

    private void OnDisable()
    {
        _buffSpawner.OnPoliceRecruitSpawned -= HandlePoliceRecruitSpawned;
        _buffSpawner.OnFireRateBoostSpawned -= HandleFireRateBoostSpawned;
        _spawnerSegmentBridge.OnBridgeConnectorSpawned -= HandleBridgeConnectorSpawned;
        _spawnerSegmentBridge.OnBaseSpawned -= HandleBaseSpawned;
    }

    public void SpawnNormalSegment(Vector3 position, Quaternion rotation, int number)
    {
        _spawnerSegmentBridge.CreateNormalSegment(position, rotation, number);
    }

    public void Generate(int level)
    {
        _objectSelector.CreateFirstSpawnObject();
        _lengthCalculator.CalculateLengthBridge(level);
        _checkpointStore.CreateCheckpointArray(_lengthCalculator.BridgePieceCount);

        for (int i = 0; i < _lengthCalculator.BridgePieceCount; i++)
        {
            float countLevel = _lengthCalculator.LenghtBridge / _sectionOffset;

            Vector3 nextSpawnPosition = _startPositionBridgeSegments.position;

            int indexLevel = 0;
            
            for (int y = 0; y < countLevel; y++)
            {
                indexLevel++;

                if (indexLevel == _indexLevelSpawnObject)
                {
                    indexLevel = 0;
                    _selectedObject = _objectSelector.GetObjectToBridge();
                    _randomPositionSection = _positionGenerator.GetRandomPositionToLevel(nextSpawnPosition);
                }

                if (_indexZombieWaveTrigger == 0 || _indexZombieWaveTrigger >= _maxIndexZombieWaveTrigget)
                {
                    CreateZombieWaveTrigger(nextSpawnPosition, i);
                }
            
                CreateLevel(nextSpawnPosition);
            
                nextSpawnPosition = _positionGenerator.GetNextPositionAlongLength(nextSpawnPosition);
                
                _indexZombieWaveTrigger++;
            }

            _spawnerSegmentBridge.SetBridgeConnectorOrFinish(i, nextSpawnPosition, _positionGenerator, _targetRotation, _checkpointStore, _lengthCalculator.BridgePieceCount);
        }
    }

    public Transform GetTargetPoint(int index)
    {
        return _checkpointStore.GetCheckpointAtIndex(index);
    }
    
    private void HandleBaseSpawned(Base basePoliceOfficer)
    {
        _endPositionPlayer = basePoliceOfficer.EndPositionPlayer;
        OnBasePoliceOfficerCreated?.Invoke(basePoliceOfficer);
    }
    
    private void HandleBridgeConnectorSpawned(BridgeConnector connector, Quaternion targetRotation, Transform startPositionBridgeSegments)
    {
        OnBridgeConnectorCreated?.Invoke(connector);
        _startPositionBridgeSegments = startPositionBridgeSegments;
        _targetRotation = targetRotation;
        _indexZombieWaveTrigger = 0;
    }
    
    private void CreateZombieWaveTrigger(Vector3 position, int index)
    {
        _indexZombieWaveTrigger = 0;

        PointSpawnTrigger waveTrigger = _spawnerZombieWaveTrigger.GetZombieTrigger(position, _targetRotation, _positionGenerator);
        waveTrigger.SetDirectionAndIndex(_positionGenerator.IsHorizontal, index);
        
        OnPointSpawnedTrigger?.Invoke(waveTrigger);
    }

    private void HandleFireRateBoostSpawned(FireRateBooster fireRateBooster)
    {
        OnFireRateBoostedCreated?.Invoke(fireRateBooster);
    }
    
    private void HandlePoliceRecruitSpawned(RecruitPolice recruitPolice)
    {
        OnRecruitPoliceCreated?.Invoke(recruitPolice);
    }
    
    private void CreateLevel(Vector3 position)
    {
        for (int j = 0; j < _laneCount; j++)
        {
            SpawnSegmentBridge(position, _selectedObject, j);
            SpawnPowerUp(position, _selectedObject);
            SpawnObstacles(position, _selectedObject);
            
            position = _positionGenerator.GetNextPositionAlongWidth(position);
        }
    }

    private void SpawnSegmentBridge(Vector3 basePosition, GameObject segmentObject, int index)
    {
        if (segmentObject != null &&
            segmentObject.TryGetComponent<IBridgeObject>(out var bridgeObject) &&
            bridgeObject != null &&
            bridgeObject.Type == BridgeObjectType.DamagedSegment)
        {
            if (ShouldPlaceDamagedSegment(basePosition))
            {
                _spawnerSegmentBridge.CreateDemagSegmentBridge(basePosition, _targetRotation, index);
                _selectedObject = null;
                return; 
            }
        }
        
        _spawnerSegmentBridge.CreateNormalSegment(basePosition, _targetRotation, index);
    }
    
    private bool ShouldPlaceDamagedSegment(Vector3 position)
    {
        return (_positionGenerator.IsHorizontal && Mathf.Approximately(_randomPositionSection.z, position.z)) ||
               (!_positionGenerator.IsHorizontal && Mathf.Approximately(_randomPositionSection.x, position.x));
    }
    
    private void SpawnPowerUp(Vector3 basePosition, GameObject segmentObject)
    {
        if (segmentObject == null)
            return;
        
        if (segmentObject.TryGetComponent<IBridgeObject>(out var bridgeObject) && bridgeObject != null)
        {
            switch (bridgeObject.Type)
            {
                case BridgeObjectType.RecruitPolice:
                    _buffSpawner.CreateRecruitPolice(_positionGenerator, basePosition, _targetRotation);
                    _selectedObject = null;
                    break;

                case BridgeObjectType.FireRateBooster:
                    _buffSpawner.CreateFireRateBooster(basePosition, _targetRotation, _randomPositionSection);
                    _selectedObject = null;
                    break;
            }
        }
    }

    private void SpawnObstacles(Vector3 positionObstacle, GameObject segmentObject)
    {
        if (segmentObject == null)
            return;
        
        if (segmentObject.TryGetComponent<IBridgeObject>(out var bridgeObject)  && bridgeObject != null)
        {
            switch (bridgeObject.Type)
            {
                case BridgeObjectType.Hammer:
                    _spawnerObstacles.CreateHummer(positionObstacle, _positionGenerator, _targetRotation);
                    _selectedObject = null;
                    break;

                case BridgeObjectType.RotatingBlade:
                    _spawnerObstacles.CreateRotatingBlade(positionObstacle, _positionGenerator, _targetRotation);
                    _selectedObject = null;
                    break;

                case BridgeObjectType.SawBlade:
                    _spawnerObstacles.CreateSawBlade(positionObstacle, _positionGenerator, _targetRotation);
                    _selectedObject = null;
                    break;

                case BridgeObjectType.SpikedCylinder:
                    _spawnerObstacles.CreateSpikedCylinder(positionObstacle, _positionGenerator, _targetRotation);
                    _selectedObject = null;
                    break;

                case BridgeObjectType.SpikePress:
                    _spawnerObstacles.CreateSpikePress(positionObstacle, _positionGenerator, _targetRotation);
                    _selectedObject = null;
                    break;

                case BridgeObjectType.Spikes:
                    _spawnerObstacles.CreateSpikes(positionObstacle, _positionGenerator, _targetRotation);
                    _selectedObject = null;
                    break;
            }
        }
    }
}
