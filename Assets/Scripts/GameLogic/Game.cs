using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using YG;
using YG.Utils.LB;

public class Game : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private SpawnerZombie _spawnerZombie;
    [SerializeField] private UnityEngine.UI.Image _buffTimerBar;
    [SerializeField] private float _normalRateOfFire = 0.8f;
    [SerializeField] private float _increasedRateOfFire = 0.4f;
    [SerializeField] private int _zombieMultiplier = 3;
    [SerializeField] private int _minCounZombie = 10;
    [SerializeField] private GameMenuHandler _gameMenuHandler;
    [SerializeField] private ScoreHandler _scoreHandler;
    
    private PlayerInput _playerInput;
    private BridgeGenerator _bridgeGenerator;
    private Monster _monster;
    private int _levelPlayer;
    private int _nextCountPoint;
    private int _countPoint;
    private int _countAllZombie;
    private int _currentScore;
    private float _buffDuration;
    private bool _isHorizontal;
    private bool _isTurnRight;
    private List<BridgeConnector> _connectors;
    private Coroutine _buffCoroutine;
    

    private void Awake()
    {
        _connectors = new List<BridgeConnector>();
        _playerInput = GetComponent<PlayerInput>();
        _bridgeGenerator = GetComponent<BridgeGenerator>();
    }

    private void OnEnable()
    {
        _gameMenuHandler.OnLevelUp += HandleOnLevelUp;
        _playerInput.OnEscapePressed += HandleEscapePressed;
        _playerInput.DirectionChanged += OnDirectionChanged;
        _bridgeGenerator.OnPointSpawnedTrigger += SpawnedTrigger;
        _bridgeGenerator.OnFireRateBoostedCreated += SubscribeToFireRateCreated;
        _bridgeGenerator.OnRecruitPoliceCreated += SubscribeToRecruitPoliced;
        _bridgeGenerator.OnBridgeConnectorCreated += SubscribeToBridgeConnectorCreation;
        _player.OnCheckpointReached += UpdatePlayerTargetPosition;
        _player.OnAllPoliceOfficersDead += HandlePoliceOfficerDead;
        _player.OnPlayerReachedBase += ActivateCallHelpButton;
        _spawnerZombie.OnZombieKilled += HandleZombieKilled;

    }

    private void OnDisable()
    {
        _gameMenuHandler.OnLevelUp -= HandleOnLevelUp;
        _bridgeGenerator.OnBridgeConnectorCreated -= SubscribeToBridgeConnectorCreation;
        _playerInput.OnEscapePressed -= HandleEscapePressed;
        _playerInput.DirectionChanged -= OnDirectionChanged;
        _bridgeGenerator.OnPointSpawnedTrigger -= SpawnedTrigger;
        _bridgeGenerator.OnFireRateBoostedCreated -= SubscribeToFireRateCreated;
        _bridgeGenerator.OnRecruitPoliceCreated -= SubscribeToRecruitPoliced;
        _player.OnCheckpointReached -= UpdatePlayerTargetPosition;
        _player.OnAllPoliceOfficersDead -= HandlePoliceOfficerDead;
        _player.OnPlayerReachedBase -= ActivateCallHelpButton;
        _spawnerZombie.OnZombieKilled -= HandleZombieKilled;
        
        foreach (BridgeConnector connector in _connectors)
        {
            connector.OnZombieDetected -= BindNewTargetOnZombieDetected;
        }
    }

    private void Start()
    {
        _buffDuration = YG2.saves.BuffDuration;
        _levelPlayer = YG2.saves.Level;
        _scoreHandler.SetInitialScore(_currentScore);
        _bridgeGenerator.Generate(_levelPlayer);
        _player.SpawnPoliceOfficer(YG2.saves.CountPoliceOfficer);
        UpdatePlayerTargetPosition();
    }
    
    private void ActivateCallHelpButton()
    {
        if (YG2.saves.IsCallHelpUpgradePurchased)
        {
            _gameMenuHandler.ActivateCallHelp();
        }
    }

    private void HandleOnLevelUp()
    {
        _levelPlayer++;
        YG2.saves.Level = _levelPlayer;
        YG2.SaveProgress();
    } 
    
    private void HandlePoliceOfficerDead()
    {
        _gameMenuHandler.ShowGameOverMenu();
    }
    
    private void HandleEscapePressed()
    {
        _gameMenuHandler.ShowPauseGameMenu();
    }

    private void UpdatePlayerTargetPosition()
    {
        _countPoint = _nextCountPoint;
        _nextCountPoint++;
        
        if (_nextCountPoint == _bridgeGenerator.BridgeSpanCount)
        {
            _player.SetPlayerFinishPosition(_bridgeGenerator.GetTargetPoint(_countPoint), true, _isHorizontal);
        }
        else
        {
            _player.SetNextTargetPosition(_bridgeGenerator.GetTargetPoint(_countPoint), _isHorizontal);
        }

        if (_countPoint == 0)
        {
            _isTurnRight = _bridgeGenerator.IsTurnRight;
        }
        else
        {
            _player.RotateInDirection(_isTurnRight);

            _isTurnRight = !_isTurnRight;
        }

        _player.UpdatePoliceHorizontalStatus();
        
        _isHorizontal = !_isHorizontal;
    }
    
    private void SubscribeToBridgeConnectorCreation(BridgeConnector bridgeConnector)
    {
        bridgeConnector.OnZombieDetected += BindNewTargetOnZombieDetected;
        _connectors.Add(bridgeConnector);
    }

    private void BindNewTargetOnZombieDetected(Zombie zombie, BridgeConnector connector)
    {
        zombie.SetFinish(_bridgeGenerator.GetTargetPoint(connector.Index));
    }

    private void SubscribeToFireRateCreated(FireRateBooster fireRateBooster)
    {
        fireRateBooster.OnFirstOfficerEntered += IncreaseGroupFireRate;
    }
    
    private void SubscribeToRecruitPoliced(RecruitPolice recruitPolice)
    {
        recruitPolice.OnRecruitPoliceTriggered += IncreasePoliceOfficerSize;
    }

    private void IncreasePoliceOfficerSize(RecruitPolice recruitPolice)
    {
        recruitPolice.OnRecruitPoliceTriggered -= IncreasePoliceOfficerSize;
        bool isPositiveX = _player.SquadPosition.transform.localPosition.x >= 0;
        int spawnCount;

        if (recruitPolice.IsMultiplication)
        {
            if (isPositiveX)
            {
                spawnCount = CalculateSpawnCount(recruitPolice);
            }
            else
            {
                spawnCount = recruitPolice.CumulativeIncrease;
            }
        }
        else
        {
            if (isPositiveX)
            {
                spawnCount = recruitPolice.CumulativeIncrease;
            }
            else
            {
                spawnCount = CalculateSpawnCount(recruitPolice);
            }
        }

        if (spawnCount > 0)
        {
            _player.SpawnPoliceOfficer(spawnCount);
        }
    }

    private int CalculateSpawnCount(RecruitPolice recruitPolice)
    {
        return (_player.PoliceCount * recruitPolice.PopulationMultiplier) - _player.PoliceCount;
    }

    private void IncreaseGroupFireRate(FireRateBooster fireRateBooster)
    {
        fireRateBooster.OnFirstOfficerEntered -= IncreaseGroupFireRate;
        
        if (_buffCoroutine != null)
            StopCoroutine(_buffCoroutine);
        _buffCoroutine = StartCoroutine(BuffTimer());
    } 
    
    private IEnumerator BuffTimer()
    {
        _player.ApplyBuffToParty(_increasedRateOfFire);
        _buffTimerBar.gameObject.SetActive(true);
        float elapsed = 0f;
        float maxFillAmount = 1;

        while (elapsed < _buffDuration)
        {
            _buffTimerBar.fillAmount = maxFillAmount - (elapsed / _buffDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _buffTimerBar.fillAmount = 0;
        _buffTimerBar.gameObject.SetActive(false);
        _player.ApplyBuffToParty(_normalRateOfFire);
    }
    
    private void SpawnedTrigger(PointSpawnTrigger spawnTrigger)
    {
        spawnTrigger.OnHordeSpawning += SpawnZombie;
    }

    private void SpawnZombie(PointSpawnTrigger spawnTrigger, Transform spawnPoint)
    {
        spawnTrigger.OnHordeSpawning -= SpawnZombie;

        int count = GetCalculateCountZombies(_player.PoliceCount);
        _countAllZombie += count;
        
        _spawnerZombie.SpawnAdaptiveWave(spawnPoint, count, _bridgeGenerator.GetTargetPoint(_countPoint), spawnTrigger.IsHorizontal);
    }
    
    private int GetCalculateCountZombies(int policeCount)
    {
        int count = policeCount * _zombieMultiplier;

        if (count < _minCounZombie)
        {
            count = _minCounZombie;
        }
        
        return count;
    }

    private void OnDirectionChanged(float direction)
    {
        _player.MoveSideways(direction);
    }
    
    private void HandleZombieKilled(bool isKilledByBullet)
    {
        _countAllZombie--;

        if (isKilledByBullet)
        {
            _scoreHandler.AddPointsForZombie();
        }

        if (_countAllZombie == 0 && _player.IsPoliceOfficerOnBase)
        {
            _player.CeaseSquadFire();
            int score = YG2.saves.Score;
            score += _scoreHandler.CurrentScore;
            YG2.saves.Score = score;
            YG2.SetLeaderboard("EachOfMatersLB", score);
            YG2.SaveProgress();
            _gameMenuHandler.ShowWinGameMenu();
        }
    }
}
