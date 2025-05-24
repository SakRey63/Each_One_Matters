using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private SpawnerZombie _spawnerZombie;
    [SerializeField] private int _levelPlayer = 1;
    [SerializeField] private float _baseLengthBridge = 100f;
    [SerializeField] private float _difficultyMultiplier = 1.2f;
    
    private PlayerInput _playerInput;
    private BridgeGenerator _bridgeGenerator;
    private PointSpawnTrigger _spawnTrigger;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _bridgeGenerator = GetComponent<BridgeGenerator>();
    }

    private void OnEnable()
    {
        _playerInput.DirectionChanged += OnDirectionChanged;
        _bridgeGenerator.OnPointSpawnedTrigger += SpawnedTrigger;
    }

    private void OnDisable()
    {
        _playerInput.DirectionChanged -= OnDirectionChanged;
        _bridgeGenerator.OnPointSpawnedTrigger -= SpawnedTrigger;
    }

    private void Start()
    {
        _bridgeGenerator.Generate(CalculateLengthBridge());
    }

    private float CalculateLengthBridge()
    {
        return _baseLengthBridge * (_levelPlayer * _difficultyMultiplier);
    }
    
    private void SpawnedTrigger(PointSpawnTrigger spawnTrigger)
    {
        _spawnTrigger = spawnTrigger;
        _spawnTrigger.OnHordeSpawning += SpawnZombie;
    }

    private void SpawnZombie(Transform transform)
    {
        _spawnTrigger.OnHordeSpawning -= SpawnZombie;
        _spawnerZombie.SpawnAdaptiveWave(transform, _player.PoliceCount);
    }

    private void OnDirectionChanged(float direction)
    {
        _player.MoveSideways(direction);
    }
}
