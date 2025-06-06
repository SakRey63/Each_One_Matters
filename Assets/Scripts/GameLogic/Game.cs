using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private int _startingUnitCount = 1;
    [SerializeField] private SpawnerZombie _spawnerZombie;
    [SerializeField] private int _levelPlayer = 1;
    [SerializeField] private float _baseLengthBridge = 100f;
    [SerializeField] private float _difficultyMultiplier = 1.2f;
    
    private PlayerInput _playerInput;
    private BridgeGenerator _bridgeGenerator;
    private Monster _monster;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _bridgeGenerator = GetComponent<BridgeGenerator>();
    }

    private void OnEnable()
    {
        _playerInput.DirectionChanged += OnDirectionChanged;
        _bridgeGenerator.OnPointSpawnedTrigger += SpawnedTrigger;
        _bridgeGenerator.OnFireRateBoostedCreated += SubscribeToFireRateCreated;
        _bridgeGenerator.OnRecruitPoliceCreated += SubscribeToRecruitPoliced;

    }

    private void OnDisable()
    {
        _playerInput.DirectionChanged -= OnDirectionChanged;
        _bridgeGenerator.OnPointSpawnedTrigger -= SpawnedTrigger;
        _bridgeGenerator.OnFireRateBoostedCreated -= SubscribeToFireRateCreated;
        _bridgeGenerator.OnRecruitPoliceCreated -= SubscribeToRecruitPoliced;
    }
    
    private void Start()
    {
        _player.SpawnPoliceOfficer(_startingUnitCount);
        _bridgeGenerator.Generate(CalculateLengthBridge());
    }

    private void SubscribeToFireRateCreated(FireRateBooster fireRateBooster)
    {
        fireRateBooster.OnFirstOfficerEntered += IncreaseGroupFireRate;
    }
    
    private void SubscribeToRecruitPoliced(RecruitPolice recruitPolice)
    {
        recruitPolice.OnRecruitPoliceTriggered += IncreasePoliceOfficerSize;
    }

    private void IncreasePoliceOfficerSize(int number, bool isMultiplication, RecruitPolice recruitPolice)
    {
        recruitPolice.OnRecruitPoliceTriggered -= IncreasePoliceOfficerSize;

        if (isMultiplication)
        {
            int spawnUnitCount = (_player.PoliceCount * number) - _player.PoliceCount;
            
            Debug.Log($"Умножить отряд в: {number} раз. Надо заспавнить {spawnUnitCount} полицейских" );

            if (spawnUnitCount > 0)
            {
                _player.SpawnPoliceOfficer(spawnUnitCount);
            }
        }
        else
        {
            Debug.Log($"Увеличить отряд на: {number} человек");
            _player.SpawnPoliceOfficer(number);
        }
        
    }

    private void IncreaseGroupFireRate(FireRateBooster fireRateBooster)
    {
        fireRateBooster.OnFirstOfficerEntered -= IncreaseGroupFireRate;
        _player.ApplyBuffToParty();
    }

    private float CalculateLengthBridge()
    {
        return _baseLengthBridge * (_levelPlayer * _difficultyMultiplier);
    }
    
    private void SpawnedTrigger(PointSpawnTrigger spawnTrigger)
    {
        spawnTrigger.OnHordeSpawning += SpawnZombie;
    }

    private void SpawnZombie(PointSpawnTrigger spawnTrigger, Transform spawnPoint)
    {
        spawnTrigger.OnHordeSpawning -= SpawnZombie;
        _spawnerZombie.SpawnAdaptiveWave(spawnPoint, _player.PoliceCount);
    }

    private void OnDirectionChanged(float direction)
    {
        _player.MoveSideways(direction);
    }
}
