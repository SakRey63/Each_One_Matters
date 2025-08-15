using System;
using UnityEngine;

public class BridgeGenerator : MonoBehaviour
{
    [SerializeField] private int _sectionOffset = 4;
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
    private MonsterSpawner _monsterSpawner;
    private BuffSpawner _buffSpawner;
    private int _indexZombieWaveTrigger;
    private Vector3 _randomPositionSection;
    private Quaternion _targetRotation;

    public int BridgeSpanCount => _lengthCalculator.SpanCount;
    public bool IsTurnRight => _positionGenerator.IsTurnRight;

    public event Action<PointSpawnTrigger> OnPointSpawnedTrigger;
    public event Action<FireRateBooster> OnFireRateBoostedCreated;
    public event Action<RecruitPolice> OnRecruitPoliceCreated;
    public event Action<BridgeConnector> OnBridgeConnectorCreated;

    private void Awake()
    {
        _monsterSpawner = GetComponent<MonsterSpawner>();
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
    }

    private void OnDisable()
    {
        _buffSpawner.OnPoliceRecruitSpawned -= HandlePoliceRecruitSpawned;
        _buffSpawner.OnFireRateBoostSpawned -= HandleFireRateBoostSpawned;
        _spawnerSegmentBridge.OnBridgeConnectorSpawned -= HandleBridgeConnectorSpawned;
        
    }

    public void Generate(int level)
    {
        _objectSelector.CreateFirstSpawnObject();
        _lengthCalculator.CalculateLengthBridge(level);
        _checkpointStore.CreateCheckpointArray(_lengthCalculator.SpanCount);

        for (int i = 0; i < _lengthCalculator.SpanCount; i++)
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
                    _objectSelector.ChoseObjectToBridge();
                    _randomPositionSection = _positionGenerator.GetRandomPositionToLevel(nextSpawnPosition);
                }

                if (_indexZombieWaveTrigger == 0 || _indexZombieWaveTrigger >= _maxIndexZombieWaveTrigget)
                {
                    CreateZombieWaveTrigger(nextSpawnPosition);
                }
            
                CreateLevel(nextSpawnPosition);
            
                nextSpawnPosition = _positionGenerator.GetNextPositionAlongLength(nextSpawnPosition);
            
                _indexZombieWaveTrigger++;
            }

            _spawnerSegmentBridge.SetBridgeConnectorOrFinish(i, nextSpawnPosition, _positionGenerator, _targetRotation, _checkpointStore, _lengthCalculator.SpanCount);
        }
    }

    public Transform GetTargetPoint(int index)
    {
        Transform point = _checkpointStore.GetCheckpointAtIndex(index);

        return point;
    }
    
    private void HandleBridgeConnectorSpawned(BridgeConnector connector, Quaternion targetRotation, Transform startPositionBridgeSegments)
    {
        OnBridgeConnectorCreated?.Invoke(connector);
        _startPositionBridgeSegments = startPositionBridgeSegments;
        _targetRotation = targetRotation;
        _indexZombieWaveTrigger = 0;
    }
    
    private void CreateZombieWaveTrigger(Vector3 position)
    {
        _indexZombieWaveTrigger = 0;

        PointSpawnTrigger waveTrigger = _spawnerZombieWaveTrigger.GetZombieTrigger(position, _targetRotation, _positionGenerator);
        
        OnPointSpawnedTrigger?.Invoke(waveTrigger);
    }

    private void CreateLevel(Vector3 position)
    {
        for (int j = 0; j < _laneCount; j++)
        {
            _spawnerSegmentBridge.CreateSegmentBridge(position, _objectSelector, _positionGenerator.IsHorizontatl, _targetRotation, _randomPositionSection);
            SpawnMonster(position);
            SpawnPowerUp(position);
            
            position = _positionGenerator.GetNextPositionAlongWidth(position);
        }
    }
    
    private void SpawnPowerUp(Vector3 basePosition)
    {
        _buffSpawner.ConditionalSpawnBuffs(_positionGenerator, _objectSelector, basePosition, _targetRotation, _randomPositionSection);
    }

    private void HandleFireRateBoostSpawned(FireRateBooster fireRateBooster)
    {
        OnFireRateBoostedCreated?.Invoke(fireRateBooster);
    }
    
    private void HandlePoliceRecruitSpawned(RecruitPolice recruitPolice)
    {
        OnRecruitPoliceCreated?.Invoke(recruitPolice);
    }

    private void SpawnMonster(Vector3 positionMonster)
    {
        _monsterSpawner.CreateAndSetupMonster(positionMonster, _objectSelector, _positionGenerator, _targetRotation);
    }
}
