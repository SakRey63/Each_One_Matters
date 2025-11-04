using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class GameSessionSystem : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private EnemyGroupController _enemyGroup;
    [SerializeField] private ScoreHandler _scoreHandler;
    [SerializeField] private LevelMenuHandler _levelMenuHandler;
    [SerializeField] private TutorialUI _tutorialUI;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private ProgressSystem _progressSystem;
    [SerializeField] private BridgeGenerator _bridgeGenerator;
    [SerializeField] private SpawnerZombie _spawnerZombie;
    [SerializeField] private Ocean _ocean;
    [SerializeField] private KillerObstacleHandler _killerObstacleHandler;
    [SerializeField] private int _countAdRevival = 5;
    [SerializeField] private ChunkPool _chunkPool;
    [SerializeField] private BulletPool _bulletPool;

    private int _levelPlayer;
    private int _nextCountPoint;
    private int _countPoint;
    private List<BridgeConnector> _connectors;
    private BridgeDirection _currentDirection = BridgeDirection.VerticalUp;
    private BridgeDirection _lastDirection;
    private BridgeDirection _lastRotation;

    private void OnEnable()
    {
        _killerObstacleHandler.OnDestroyDamageSegmentBridge += RestoreBridgeSegment;
        _player.OnUnitDied += HandleUnitDead;
        _player.OnAllPoliceOfficersDied += HandleAllPoliceOfficerDied;
        _player.OnCheckpointReached += UpdatePlayerTargetPosition;
        _player.OnPlayerReachedBase += ActivateCallHelpButton;
        _enemyGroup.OnZombieKilled += HandleZombieKilled;
        _levelMenuHandler.OnLevelUp += HandleOnLevelUp;
        _levelMenuHandler.OnCallHelpPoliceOfficer += HandleCallHelpPolice;
        _levelMenuHandler.OnRewardedAdClicked += RevealAndRemoveKillerObstacle;
        _bridgeGenerator.OnPointSpawnedTrigger += SpawnedTrigger;
        _bridgeGenerator.OnFireRateBoostedCreated += SubscribeToFireRateCreated;
        _bridgeGenerator.OnRecruitPoliceCreated += SubscribeToRecruitPoliced;
        _bridgeGenerator.OnBridgeConnectorCreated += SubscribeToBridgeConnectorCreation;
        _bridgeGenerator.OnBasePoliceOfficerCreated += SubscribeToBasePoliceOfficer;
    }

    private void OnDisable()
    {
        _killerObstacleHandler.OnDestroyDamageSegmentBridge -= RestoreBridgeSegment;
        _player.OnCheckpointReached -= UpdatePlayerTargetPosition;
        _player.OnUnitDied -= HandleUnitDead;
        _player.OnAllPoliceOfficersDied -= HandleAllPoliceOfficerDied;
        _player.OnPlayerReachedBase -= ActivateCallHelpButton;
        _enemyGroup.OnZombieKilled -= HandleZombieKilled;
        _levelMenuHandler.OnLevelUp -= HandleOnLevelUp;
        _levelMenuHandler.OnCallHelpPoliceOfficer -= HandleCallHelpPolice;
        _bridgeGenerator.OnPointSpawnedTrigger -= SpawnedTrigger;
        _bridgeGenerator.OnFireRateBoostedCreated -= SubscribeToFireRateCreated;
        _bridgeGenerator.OnRecruitPoliceCreated -= SubscribeToRecruitPoliced;
        _bridgeGenerator.OnBridgeConnectorCreated -= SubscribeToBridgeConnectorCreation;
        _bridgeGenerator.OnBasePoliceOfficerCreated -= SubscribeToBasePoliceOfficer;
    }

    private void Awake()
    {
        _connectors = new List<BridgeConnector>();
    }

    public void StartNewLevel()
    {
        if (_progressSystem.GetPlayerCount() > _progressSystem.GetMaxPlayerCount())
        {
            _progressSystem.SetPlayerCount(_progressSystem.GetMaxPlayerCount());
        }

        SetCursorState(true, false);
        _cameraController.ConfigureCameraForPlatform();
        _cameraController.OnUnitCountChanged(_progressSystem.GetPlayerCount());
        _player.SetupObjectsPool(_bulletPool, _chunkPool);
        _levelPlayer = _progressSystem.GetCurrentLevel();
        _scoreHandler.SetInitialScore(_progressSystem.GetCurrentScore());
        _bridgeGenerator.Generate(_levelPlayer);

        if (YG2.saves.gameplay.IsFirstLaunch)
        {
            StartCoroutine(WaitForFirstInput());
        }
        else
        {
            _levelMenuHandler.InitializeLevelMenu();
            UpdatePlayerTargetPosition();
            _player.SpawnPoliceOfficer(YG2.saves.player.CountPoliceOfficer);
        }
    }
    
    public void ResumeAfterAd()
    {
        SetCursorState(true, false);
        int maxAllowed = YG2.saves.player.MaxPoliceCount - _player.PoliceCount;
        int reviveCount = Mathf.Min(_countAdRevival, maxAllowed);

        if (reviveCount > 0)
        {
            _player.SpawnPoliceOfficer(reviveCount);
            _player.KeepMoving();
            _enemyGroup.ResumeAllZombies();
        }    
    }
    
    private void RestoreBridgeSegment(Transform transform, int number)
    {
        _bridgeGenerator.SpawnNormalSegment(transform.position, transform.rotation, number);
    }
    
    private IEnumerator WaitForFirstInput()
    {
        _tutorialUI.PlayGameGuide();
        
        yield return new WaitUntil(() => Input.anyKeyDown);
        
        _tutorialUI.CloseGameGuide();
        _levelMenuHandler.InitializeLevelMenu();
        UpdatePlayerTargetPosition();
        _player.SpawnPoliceOfficer(YG2.saves.player.CountPoliceOfficer);
        YG2.saves.gameplay.IsFirstLaunch = false;
        YG2.SaveProgress();
    }
    
    private void RevealAndRemoveKillerObstacle()
    {
        _killerObstacleHandler.ProcessObstacles(_player.SquadPosition);
    }
    
    private void HandleCallHelpPolice()
    {
        if (_player.PoliceCount >= _progressSystem.GetMaxPlayerCount())
        {
            _levelMenuHandler.ShowMaxPoliceWarning();
            return;
        }
        
        if (_scoreHandler.CurrentScore >= _progressSystem.GetCallHelpPrice())
        {
            _scoreHandler.DeductPointsForHelp(_progressSystem.GetCallHelpPrice());
            _levelMenuHandler.LockButtonDuringCooldown();
            
            int spawnCount = Mathf.Min(YG2.saves.gameplay.CountHelpPoliceOfficer, 
                YG2.saves.player.MaxPoliceCount - _player.PoliceCount);
            
            _player.SpawnPoliceOfficer(spawnCount);
            
            if (_player.PoliceCount >= _progressSystem.GetMaxPlayerCount())
            {
                _levelMenuHandler.ShowMaxPoliceWarning();
            }
        }
        else
        {
            _levelMenuHandler.ShowNotEnoughPointsWindow();
        }
    }
    
    private void UpdatePlayerTargetPosition()
    {
        _countPoint = _nextCountPoint;
        _nextCountPoint++;
        
        if (_countPoint == 0)
        {
            _lastDirection = _bridgeGenerator.FirstTurn;
            _lastRotation = _bridgeGenerator.FirstTurn;
        }
        else
        {
            _player.RotateInDirection(_lastRotation);

            _lastRotation = _lastRotation == BridgeDirection.HorizontalRight
                ? BridgeDirection.HorizontalLeft
                : BridgeDirection.HorizontalRight;
        }
        
        if (_nextCountPoint == _bridgeGenerator.BridgeSpanCount)
        {
            _player.SetPlayerFinishPosition(_bridgeGenerator.EndPositionPlayer, true, _currentDirection);
        }
        else
        {
            _player.SetNextTargetPosition(_bridgeGenerator.GetTargetPoint(_countPoint), _currentDirection);
        }

        _player.UpdatePoliceHorizontalStatus();
        _ocean.UpdateOceanPositionRelativeToPlayer(_player.transform);

        _currentDirection = _currentDirection == BridgeDirection.VerticalUp
            ? _lastDirection
            : BridgeDirection.VerticalUp;
    }

    private void HandleUnitDead(int number)
    {
        _cameraController.OnUnitCountChanged(number);
    }

    private void HandleAllPoliceOfficerDied()
    {
        _player.StopMoving();
        _player.ResetPositionUiElements();
        _enemyGroup.StopAllZombies();
        _levelMenuHandler.ShowGameOverMenu(_player.IsPoliceOfficerOnBase);
    }

    private void HandleZombieKilled(UnitStatus killedStatus)
    {
        if (killedStatus == UnitStatus.KilledByBullet)
        {
            _scoreHandler.AddPointsForZombie();
        }

        CheckForWinnings();
    }

    private void CheckForWinnings()
    {
        if (_enemyGroup.CountAllZombies == 0 && _player.IsPoliceOfficerOnBase && _player.PoliceCount > 0)
        {
            _progressSystem.AddScore(_scoreHandler.CurrentScore);
            _levelMenuHandler.ShowWinGameMenu();
            _player.CeaseSquadFire();
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

        if (_player.PoliceCount >= YG2.saves.player.MaxPoliceCount)
        {
            _levelMenuHandler.ShowMaxPoliceWarning();
            return;
        }
        
        spawnCount = Mathf.Min(spawnCount, YG2.saves.player.MaxPoliceCount - _player.PoliceCount);
        
        if (spawnCount > 0)
        {
            _player.SpawnPoliceOfficer(spawnCount);
            _cameraController.OnUnitCountChanged(_player.PoliceCount);
            
            if (_player.PoliceCount == YG2.saves.player.MaxPoliceCount)
            {
                _levelMenuHandler.ShowMaxPoliceWarning();
            }
        }
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
    
    private void SubscribeToBasePoliceOfficer(Base basePoliceOfficer)
    {
        basePoliceOfficer.OnPoliceOfficerDetected += HandlePoliceOfficerDetected;
    }

    private void HandlePoliceOfficerDetected(Base basePoliceOfficer)
    {
        basePoliceOfficer.OnPoliceOfficerDetected -= HandlePoliceOfficerDetected;
        _player.HandlePoliceReachedBase(basePoliceOfficer);
    }
    
    private void ActivateCallHelpButton()
    {
       
        _player.ResetPositionUiElements();
        
        SetCursorState(false, true);
        
        if (YG2.saves.gameplay.IsCallHelpUpgradePurchased)
        {
            _levelMenuHandler.ActivateCallHelp();
        }

        CheckForWinnings();
    }
    
    private void HandleOnLevelUp()
    {
        _levelPlayer++;
        _progressSystem.SetCurrentLevel(_levelPlayer);
    }
    
    private void SetCursorState(bool isLocked, bool isVisible)
    {
        if (YG2.envir.isDesktop)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
            Cursor.visible = isVisible;
        }
    }
}