using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class Game : MonoBehaviour
{
    private const string LeaderboardId = "EachOfMatersLB";
    
    [SerializeField] private Player _player;
    [SerializeField] private EnemyGroupController _enemyGroup;
    [SerializeField] private SpawnerZombie _spawnerZombie;
    [SerializeField] private TutorialUI _tutorialUI;
    [SerializeField] private float _xOffsetOcean = 59f;
    [SerializeField] private float _searchRadius = 5f;
    [SerializeField] private int _countAdRevival = 5;
    [SerializeField] private LevelMenuHandler _levelMenuHandler;
    [SerializeField] private ScoreHandler _scoreHandler;
    [SerializeField] private Ocean _ocean;
    
    private CameraController _cameraController;
    private PlayerInput _playerInput;
    private BridgeGenerator _bridgeGenerator;
    private DemoModeController _demoMode;
    private ChunkPool _chunkPool;
    private BulletPool _bulletPool;
    private int _levelPlayer;
    private int _nextCountPoint;
    private int _countPoint;
    private int _currentScore;
    private bool _isHorizontal;
    private bool _isTurnRight;
    private List<BridgeConnector> _connectors;
    private Coroutine _buffCoroutine;
    

    private void Awake()
    {
        _bulletPool = GetComponent<BulletPool>();
        _chunkPool = GetComponent<ChunkPool>();
        _demoMode = GetComponent<DemoModeController>();
        _cameraController = GetComponent<CameraController>();
        _connectors = new List<BridgeConnector>();
        _playerInput = GetComponent<PlayerInput>();
        _bridgeGenerator = GetComponent<BridgeGenerator>();
    }

    private void OnEnable()
    {
        _player.OnUnitDeath += HandleUnitDead;
        _levelMenuHandler.OnLevelUp += HandleOnLevelUp;
        _levelMenuHandler.OnRewardedAdClicked += RevealAndRemoveKillerObstacle;
        _levelMenuHandler.OnCallHelpPoliceOfficer += HandleCallHelpPolice;
        _levelMenuHandler.OnRewardedAdWatched += SetupRevivalWithAd;
        _bridgeGenerator.OnPointSpawnedTrigger += SpawnedTrigger;
        _bridgeGenerator.OnFireRateBoostedCreated += SubscribeToFireRateCreated;
        _bridgeGenerator.OnRecruitPoliceCreated += SubscribeToRecruitPoliced;
        _bridgeGenerator.OnBridgeConnectorCreated += SubscribeToBridgeConnectorCreation;
        _bridgeGenerator.OnBasePoliceOfficerCreated += SubscribeToBasePoliceOfficer;
        _player.OnCheckpointReached += UpdatePlayerTargetPosition;
        _player.OnAllPoliceOfficersDead += HandleAllPoliceOfficerDead;
        _player.OnPlayerReachedBase += ActivateCallHelpButton;
        _enemyGroup.OnZombieKilled += HandleZombieKilled;
    }

    private void OnDisable()
    {
        _player.OnUnitDeath += HandleUnitDead;
        _playerInput.OnEscapePressed -= HandleEscapePressed;
        _levelMenuHandler.OnRewardedAdWatched -= SetupRevivalWithAd;
        _levelMenuHandler.OnRewardedAdClicked -= RevealAndRemoveKillerObstacle;
        _levelMenuHandler.OnLevelUp -= HandleOnLevelUp;
        _levelMenuHandler.OnCallHelpPoliceOfficer -= HandleCallHelpPolice;
        _bridgeGenerator.OnBridgeConnectorCreated -= SubscribeToBridgeConnectorCreation;
        _bridgeGenerator.OnPointSpawnedTrigger -= SpawnedTrigger;
        _bridgeGenerator.OnFireRateBoostedCreated -= SubscribeToFireRateCreated;
        _bridgeGenerator.OnRecruitPoliceCreated -= SubscribeToRecruitPoliced;
        _bridgeGenerator.OnBasePoliceOfficerCreated -= SubscribeToBasePoliceOfficer;
        _player.OnCheckpointReached -= UpdatePlayerTargetPosition;
        _player.OnAllPoliceOfficersDead -= HandleAllPoliceOfficerDead;
        _player.OnPlayerReachedBase -= ActivateCallHelpButton;
        _enemyGroup.OnZombieKilled -= HandleZombieKilled;
        
        foreach (BridgeConnector connector in _connectors)
        {
            connector.OnZombieDetected -= BindNewTargetOnZombieDetected;
        }
    }

    private void Start()
    {
        if (YG2.saves.IsLoadedMainMenu == false)
        {
            _cameraController.ConfigureCameraForPlatform();
            _cameraController.OnUnitCountChanged(YG2.saves.CountPoliceOfficer);
            Cursor.visible = false;
            _playerInput.OnEscapePressed += HandleEscapePressed;
            _playerInput.DirectionChanged += OnDirectionChanged;
            _levelPlayer = YG2.saves.Level;
            _scoreHandler.SetInitialScore(_currentScore);
            _bridgeGenerator.Generate(_levelPlayer);
            _player.SetupObjectsPool(_bulletPool, _chunkPool);

            if (YG2.saves.IsPlayGameGuide)
            {
                StartCoroutine(WaitForFirstInput());
            }
            else
            {
                _player.SpawnPoliceOfficer(YG2.saves.CountPoliceOfficer);
                UpdatePlayerTargetPosition();
            }
        }
    }

    public void StartDemoScene()
    {
        _demoMode.SetupDemoScene(_spawnerZombie, _player.PoliceOfficerSpawner, _bulletPool, _chunkPool);
    }
    
    private void HandleUnitDead(int number)
    {
        _cameraController.OnUnitCountChanged(number);
    }
    
    private IEnumerator WaitForFirstInput()
    {
        _tutorialUI.PlayGameGuide();
        
        yield return new WaitUntil(() => Input.anyKeyDown);
        
        _tutorialUI.CloseGameGuide();
        _player.SpawnPoliceOfficer(YG2.saves.CountPoliceOfficer);
        UpdatePlayerTargetPosition();
        YG2.saves.IsPlayGameGuide = false;
        YG2.SaveProgress();
    }
    
    private void RevealAndRemoveKillerObstacle()
    {
        Collider[] hits = Physics.OverlapSphere(_player.SquadPosition.position, _searchRadius);

        foreach (Collider collider in hits)
        {
            if (collider.TryGetComponent<Hammer>(out _))
            {
                Destroy(collider.gameObject);
            }
            else if (collider.TryGetComponent<RotatingBlade>(out _))
            {
                Destroy(collider.gameObject);
            }
            else if (collider.TryGetComponent<SawBlade>(out _))
            {
                Destroy(collider.gameObject);
            }
            else if (collider.TryGetComponent<SpikedCylinder>(out _))
            {
                Destroy(collider.gameObject);
            }
            else if (collider.TryGetComponent<SpikePress>(out _))
            {
                Destroy(collider.gameObject);
            }
            else if (collider.TryGetComponent<ScannerObstacle>(out _))
            {
                Destroy(collider.gameObject);
            }
            else if (collider.TryGetComponent(out SegmentDamagedBridge damagedBridge))
            {
                foreach (Collider police in hits)
                {
                    if (police.TryGetComponent(out PoliceOfficer policeOfficer))
                    {
                        policeOfficer.OnDeathAnimationComplete();
                    }
                }
                Transform transform = damagedBridge.transform;
                int number = damagedBridge.NumberPosition;
                Destroy(collider.gameObject);
                _bridgeGenerator.SpawnNormalSegment(transform, number);
            }
            else if (collider.TryGetComponent(out Zombie zombie))
            {
                _spawnerZombie.ReturnDemoEnemies(zombie);
            }
        }
    }
    
    private void SubscribeToBasePoliceOfficer(Base basePoliceOfficer)
    {
        basePoliceOfficer.OnPoliceOfficerDetected += HandlePoliceOfficerDetected;
    }

    private void HandlePoliceOfficerDetected(Base basePoliceOfficer)
    {
        basePoliceOfficer.OnPoliceOfficerDetected -= HandlePoliceOfficerDetected;
        _player.HandlePoliceReachedBase(basePoliceOfficer);
    }

    private void SetupRevivalWithAd()
    {
        _playerInput.DirectionChanged += OnDirectionChanged;
        _player.SpawnPoliceOfficer(_countAdRevival);
        _player.KeepMoving();
        _enemyGroup.ResumeAllZombies();
    }

    private void HandleCallHelpPolice()
    {
        if (_scoreHandler.CurrentScore >= YG2.saves.CallHelpButtonPrice)
        {
            _scoreHandler.DeductPointsForHelp(YG2.saves.CallHelpButtonPrice);
            _levelMenuHandler.LockButtonDuringCooldown();
            _player.SpawnPoliceOfficer(YG2.saves.CountHelpPoliceOfficer);
        }
        else
        {
            _levelMenuHandler.ShowNotEnoughPointsWindow();
        }
    }
    
    private void ActivateCallHelpButton()
    {
        _playerInput.DirectionChanged -= OnDirectionChanged;
        _player.ResetPositionUiElements();
        
        Cursor.visible = true;
        
        if (YG2.saves.IsCallHelpUpgradePurchased)
        {
            _levelMenuHandler.ActivateCallHelp();
        }

        CheckForWinnings();
    }

    private void HandleOnLevelUp()
    {
        _levelPlayer++;
        YG2.saves.Level = _levelPlayer;
        YG2.SaveProgress();
    } 
    
    private void HandleEscapePressed()
    {
        _levelMenuHandler.ShowPauseGameMenu();
    }

    private void UpdatePlayerTargetPosition()
    {
        _countPoint = _nextCountPoint;
        _nextCountPoint++;
        
        if (_countPoint == 0)
        {
            _isTurnRight = _bridgeGenerator.IsTurnRight;
        }
        else
        {
            _player.RotateInDirection(_isTurnRight);

            _isTurnRight = !_isTurnRight;
        }
        
        if (_nextCountPoint == _bridgeGenerator.BridgeSpanCount)
        {
            _player.SetPlayerFinishPosition(_bridgeGenerator.EndPositionPlayer, true, _isHorizontal);
        }
        else
        {
            _player.SetNextTargetPosition(_bridgeGenerator.GetTargetPoint(_countPoint), _isHorizontal);
        }

        _player.UpdatePoliceHorizontalStatus();

        UpdateOceanPositionRelativeToPlayer();
        
        _isHorizontal = !_isHorizontal;
    }

    private void UpdateOceanPositionRelativeToPlayer()
    {
        Vector3 position = _player.transform.position;
            
        if (_isHorizontal)
        {
            if (_isTurnRight)
            {
                position = new Vector3(position.x - _xOffsetOcean, _ocean.transform.position.y, position.z);
            }
            else
            {
                position = new Vector3(position.x + _xOffsetOcean, _ocean.transform.position.y, position.z);
            }
        }
        else
        {
            position = new Vector3(position.x, _ocean.transform.position.y, position.z + _xOffsetOcean);
        }
            
        _ocean.gameObject.transform.position = position;
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
    
    private void IncreaseGroupFireRate(FireRateBooster fireRateBooster)
    {
        fireRateBooster.OnFirstOfficerEntered -= IncreaseGroupFireRate;
        _player.ApplyBuffToParty(fireRateBooster.BuffDuration, fireRateBooster.IncreasedRateOfFire);
    } 
    
    private void SubscribeToRecruitPoliced(RecruitPolice recruitPolice)
    {
        recruitPolice.OnRecruitPoliceTriggered += IncreasePoliceOfficerSize;
    }

    private void IncreasePoliceOfficerSize(RecruitPolice recruitPolice)
    {
        recruitPolice.OnRecruitPoliceTriggered -= IncreasePoliceOfficerSize;
        bool isPositiveX = _player.SquadPosition.transform.localPosition.x >= 0;
        int spawnCount = recruitPolice.GetCountSpawnPoliceOfficers(isPositiveX, _player.PoliceCount);

        if (spawnCount > 0)
        {
            _player.SpawnPoliceOfficer(spawnCount);
            _cameraController.OnUnitCountChanged(_player.PoliceCount);
        }
    }
    
    private void SpawnedTrigger(PointSpawnTrigger spawnTrigger)
    {
        spawnTrigger.OnHordeSpawning += SpawnZombie;
        spawnTrigger.AssignSpawnerAndGroup(_spawnerZombie, _enemyGroup);
    }

    private void SpawnZombie(PointSpawnTrigger spawnTrigger)
    {
        spawnTrigger.OnHordeSpawning -= SpawnZombie;
        spawnTrigger.SpawnAdaptiveWave(_player.PoliceCount, _bridgeGenerator.GetTargetPoint(spawnTrigger.Index));
    }

    private void OnDirectionChanged(float direction)
    {
        _player.MoveSideways(direction);
    }
    
    private void HandleAllPoliceOfficerDead()
    {
        _playerInput.DirectionChanged -= OnDirectionChanged;
        _player.StopMoving();
        _player.ResetPositionUiElements();
        _enemyGroup.StopAllZombies();
        _levelMenuHandler.ShowGameOverMenu(_player.IsPoliceOfficerOnBase);
    }
    
    private void HandleZombieKilled(bool isKilledByBullet)
    {
        if (isKilledByBullet)
        {
            _scoreHandler.AddPointsForZombie();
        }

        CheckForWinnings();
    }

    private void CheckForWinnings()
    {
        if (_enemyGroup.CountAllZombies == 0 && _player.IsPoliceOfficerOnBase && _player.PoliceCount > 0)
        {
            int score = YG2.saves.Score;
            score += _scoreHandler.CurrentScore;
            YG2.saves.Score = score;
            YG2.SetLeaderboard(LeaderboardId, score);
            YG2.SaveProgress();
            _levelMenuHandler.ShowWinGameMenu();
            _player.CeaseSquadFire();
        }
    }
}