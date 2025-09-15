using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class Game : MonoBehaviour
{
    private const string LeaderboardId = "EachOfMatersLB";
    
    [SerializeField] private Player _player;
    [SerializeField] private SpawnerZombie _spawnerZombie;
    [SerializeField] private UnityEngine.UI.Image _buffTimerBar;
    [SerializeField] private float _normalRateOfFire = 0.8f;
    [SerializeField] private float _increasedRateOfFire = 0.4f;
    [SerializeField] private float _xOffsetOcean = 59f;
    [SerializeField] private float _searchRadius = 5f;
    [SerializeField] private int _zombieMultiplier = 3;
    [SerializeField] private int _minCounZombie = 10;
    [SerializeField] private int _countAdRevival= 5;
    [SerializeField] private GameMenuHandler _gameMenuHandler;
    [SerializeField] private ScoreHandler _scoreHandler;
    [SerializeField] private Ocean _ocean;

    private CameraController _cameraController;
    private PlayerInput _playerInput;
    private BridgeGenerator _bridgeGenerator;
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
        _cameraController = GetComponent<CameraController>();
        _connectors = new List<BridgeConnector>();
        _playerInput = GetComponent<PlayerInput>();
        _bridgeGenerator = GetComponent<BridgeGenerator>();
        _cameraController.ConfigureCameraForPlatform();
    }

    private void OnEnable()
    {
        _gameMenuHandler.OnLevelUp += HandleOnLevelUp;
        _gameMenuHandler.OnRewardedAdClicked += RevealAndRemoveKillerObstacle;
        _gameMenuHandler.OnCallHelpPoliceOfficer += HandleCallHelpPolice;
        _gameMenuHandler.OnRewardedAdWatched += SetupRevivalWithAd;
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
        _gameMenuHandler.OnRewardedAdWatched -= SetupRevivalWithAd;
        _gameMenuHandler.OnRewardedAdClicked -= RevealAndRemoveKillerObstacle;
        _gameMenuHandler.OnLevelUp -= HandleOnLevelUp;
        _gameMenuHandler.OnCallHelpPoliceOfficer -= HandleCallHelpPolice;
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
                Transform transform = damagedBridge.transform;
                int number = damagedBridge.NumberPosition;
                Destroy(collider.gameObject);
                _bridgeGenerator.SpawnNormalSegment(transform, number);
            }
        }
    }
    
    private void SetupRevivalWithAd()
    {
        _player.SpawnPoliceOfficer(_countAdRevival);
    }

    private void HandleCallHelpPolice()
    {
        if (_scoreHandler.CurrentScore >= YG2.saves.CallHelpButtonPrice)
        {
            _scoreHandler.DeductPointsForHelp(YG2.saves.CallHelpButtonPrice);
            _gameMenuHandler.LockButtonDuringCooldown();
            _player.SpawnPoliceOfficer(YG2.saves.CountHelpPoliceOfficer);
        }
        else
        {
            _gameMenuHandler.ShowNotEnoughPointsWindow();
        }
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
    
    private void HandleEscapePressed()
    {
        _gameMenuHandler.ShowPauseGameMenu();
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
            _player.SetPlayerFinishPosition(_bridgeGenerator.GetTargetPoint(_countPoint), true, _isHorizontal);
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
        }
    }
    
    private void IncreaseGroupFireRate(FireRateBooster fireRateBooster, ParticleSystem effect)
    {
        fireRateBooster.OnFirstOfficerEntered -= IncreaseGroupFireRate;

        effect.transform.parent = _player.SquadPosition;
        effect.transform.position = _player.SquadPosition.position;
        if (_buffCoroutine != null)
            StopCoroutine(_buffCoroutine);
        _buffCoroutine = StartCoroutine(BuffTimer(effect));
    } 
    
    private IEnumerator BuffTimer(ParticleSystem effect)
    {
        effect.Play();
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

        effect.Stop();
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
    
    private void HandlePoliceOfficerDead()
    {
        _gameMenuHandler.ShowGameOverMenu(_player.IsPoliceOfficerOnBase);
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
            YG2.SetLeaderboard(LeaderboardId, score);
            YG2.SaveProgress();
            _gameMenuHandler.ShowWinGameMenu();
        }
    }
}
