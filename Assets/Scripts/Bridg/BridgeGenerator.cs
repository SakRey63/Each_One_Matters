using System;
using UnityEngine;

public class BridgeGenerator : MonoBehaviour
{
    [SerializeField] private int _sectionOffset = 5;
    [SerializeField] private float _bridgeConnectorOffset = 23f;
    [SerializeField] private float _baseOffset = 13f;
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
    private int _indexLevel;
    private int _indexDamageSegment;
    private Transform _endPositionPlayer;
    private Quaternion _targetRotation;

    public int BridgeSpanCount => _lengthCalculator.BridgePieceCount;
    public BridgeDirection FirstTurn => _positionGenerator.FirstTurnDirection;
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
            
            for (int y = 0; y < countLevel; y++)
            {
                _indexLevel++;

                if (_indexZombieWaveTrigger == 0 || _indexZombieWaveTrigger >= _maxIndexZombieWaveTrigget)
                {
                    CreateZombieWaveTrigger(nextSpawnPosition, i);
                }
            
                CreateLevel(nextSpawnPosition);
            
                nextSpawnPosition = _positionGenerator.GetNextPositionAlongLength(nextSpawnPosition);
                
                _indexZombieWaveTrigger++;
            }

            CreateConnectorOrFinish(nextSpawnPosition, i);
        }
    }

    public Transform GetTargetPoint(int index)
    {
        return _checkpointStore.GetCheckpointAtIndex(index);
    }
    
    private void CreateZombieWaveTrigger(Vector3 position, int index)
    {
        _indexZombieWaveTrigger = 0;

        PointSpawnTrigger waveTrigger = _spawnerZombieWaveTrigger.GetZombieTrigger(_positionGenerator.GetNextPositionAlongWidth(position), _targetRotation);
        waveTrigger.SetDirectionAndIndex(index);
        
        OnPointSpawnedTrigger?.Invoke(waveTrigger);
    }
    
    private void CreateConnectorOrFinish(Vector3 nextSpawnPosition, int index)
    {
        int number = index;

        if (++number < _lengthCalculator.BridgePieceCount)
        {
            Vector3 position = _positionGenerator.GetPositionToBaseOfConnector(nextSpawnPosition, _bridgeConnectorOffset);
            BridgeConnector connector = _spawnerSegmentBridge.GetBridgeConnector(position, _targetRotation);
            _checkpointStore.AddCheckpointAtIndex(index, connector.RotationTarget);
            BridgeDirection isTurnedRight = _positionGenerator.ToggleMovementDirection();
            connector.SetIndex(number, isTurnedRight);
            float  currentYAngle = _positionGenerator.GetAngelAndCreateNextStartPositionBridgeSegment(_targetRotation.eulerAngles.y, connector.BridgeStartPointRight, connector.BridgeStartPointLeft);
            _startPositionBridgeSegments = _positionGenerator.StartPositionBridgeSegments;
            _targetRotation = Quaternion.Euler(_targetRotation.x, currentYAngle, _targetRotation.z);
            _indexZombieWaveTrigger = 0;
            OnBridgeConnectorCreated?.Invoke(connector);
        }
        else
        {
            Vector3 position = _positionGenerator.GetPositionToBaseOfConnector(nextSpawnPosition, _baseOffset);
            Base basePoliceOfficer = _spawnerSegmentBridge.GetBase(position, _targetRotation);
            _checkpointStore.AddCheckpointAtIndex(index, basePoliceOfficer.BaseEntryTransform);
            _endPositionPlayer = basePoliceOfficer.EndPositionPlayer;
            OnBasePoliceOfficerCreated?.Invoke(basePoliceOfficer);
        }
    }
    
    private void CreateLevel(Vector3 position)
    {
        bool isDamageSegment = false;
        
        for (int j = 0; j < _laneCount; j++)
        {
            if (_indexLevel == _indexLevelSpawnObject)
            {
                _indexLevel = 0;
                _objectSelector.CreateObjectToBridge();
                isDamageSegment = SpawnSegmentBridge();
                SpawnPowerUp(position);
                SpawnObstacles(position);
            }

            if (isDamageSegment == false)
            {
                _spawnerSegmentBridge.CreateNormalSegment(position, _targetRotation, j);
            }
            else
            {
                _spawnerSegmentBridge.CreateDamageSegmentBridge(position, _targetRotation, j, _indexDamageSegment);
            }
            
            position = _positionGenerator.GetNextPositionAlongWidth(position);
        }
    }

    private bool SpawnSegmentBridge()
    {
        bool isDamageSegmentSpawn = false;

        if (_objectSelector.CurrentType == BridgeObjectType.DamagedSegment)
        {
            isDamageSegmentSpawn = true;
            _indexDamageSegment = _positionGenerator.GetIndexNumberPosition();
        }

        return isDamageSegmentSpawn;
    }
    
    private void SpawnPowerUp(Vector3 basePosition)
    {
        switch (_objectSelector.CurrentType)
        {
            case BridgeObjectType.RecruitPolice:
                RecruitPolice recruitPolice = _buffSpawner.GetRecruitPolice(_positionGenerator.GetNextPositionAlongWidth(basePosition), _targetRotation);
                OnRecruitPoliceCreated?.Invoke(recruitPolice);
                break;

            case BridgeObjectType.FireRateBooster:
                FireRateBooster fireRateBooster = _buffSpawner.GetFireRateBooster(_targetRotation, _positionGenerator.GetRandomPositionToLevel(basePosition));
                OnFireRateBoostedCreated?.Invoke(fireRateBooster);
                break;
        }
    }

    private void SpawnObstacles(Vector3 positionObstacle)
    {
        switch (_objectSelector.CurrentType)
        {
            case BridgeObjectType.Hammer:
                _positionGenerator.CreateRandomSide();
                _spawnerObstacles.CreateHummer(_positionGenerator.GetObstaclePosition(positionObstacle), _positionGenerator.Side, _targetRotation);
                break;

            case BridgeObjectType.RotatingBlade:
                _spawnerObstacles.CreateRotatingBlade(_positionGenerator.GetNextPositionAlongWidth(positionObstacle), _targetRotation);
                break;

            case BridgeObjectType.SawBlade:
                _spawnerObstacles.CreateSawBlade(_positionGenerator.GetNextPositionAlongWidth(positionObstacle), _targetRotation);
                break;

            case BridgeObjectType.SpikedCylinder:
                _spawnerObstacles.CreateSpikedCylinder(_positionGenerator.GetNextPositionAlongWidth(positionObstacle), _targetRotation);
                break;

            case BridgeObjectType.SpikePress:
                _positionGenerator.CreateRandomSide();
                _spawnerObstacles.CreateSpikePress(_positionGenerator.GetNextPositionAlongWidth(positionObstacle), _positionGenerator.Side, _targetRotation);
                break;

            case BridgeObjectType.Spikes:
                _spawnerObstacles.CreateSpikes(_positionGenerator.GetRandomPositionToLevel(positionObstacle), _targetRotation);
                break;
        }
    }
}
