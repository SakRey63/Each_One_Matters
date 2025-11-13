using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private float _borderOffset = 5.7f;
    [SerializeField] private Transform _positionSpawnPoliceOfficer;
    [SerializeField] private float _reorganizationDelay = 0.7f;
    [SerializeField] private int _healthPoliceOfficer = 75;
    [SerializeField] private ParticleSystem _effectPlexus;
    [SerializeField] private UnityEngine.UI.Image _buffTimerBar;
    [SerializeField] private float _defaultRepeatShooting = 0.8f;
    [SerializeField] private PoleceOfficerSpawner _officerSpawner;

    private Base _base;
    private Transform _moveTarget;
    private Transform _startPositionGenerationSquad;
    private PlayerBackwardMovement _backwardMovement;
    private PlayerRotation _playerRotation;
    private Dictionary<int, PoliceOfficer> _policeOfficers;
    private PlayerSideMovement _playerSideMovement;
    
    private PositionGeneratorSquad _positionGeneratorSquad;
    private BulletPool _bulletPool;
    private ChunkPool _chunkPool;
    private PlayerView _playerView;
    private int _numberOfficer;
    private int _policeCount;
    private Coroutine _coroutineReorganize;
    private Coroutine _coroutineBuff;
    private bool _isMoving;
    private bool _isFinished;
    private bool _isPoliceOfficerOnBase;
    private bool _isStoppedWar;
    private float _maxBorderPosition;
    private float _minBorderPosition;
    private float _repeatShooting;
    private ProgressionState _currentState;
    private BridgeDirection _currentDirection = BridgeDirection.VerticalUp;
    
    public int PoliceCount => _policeCount;
    public Transform SquadPosition => _playerSideMovement.SquadPosition;
    public bool IsPoliceOfficerOnBase => _isPoliceOfficerOnBase;

    public event Action OnCheckpointReached;
    public event Action OnAllPoliceOfficersDied;
    public event Action OnPlayerReachedBase;
    public event Action<int> OnUnitDied;

    private void OnEnable()
    {
        _backwardMovement.OnPlayerFinish += HandlePlayerFinish;
    }

    private void OnDisable()
    {
        _playerInput.DirectionChanged -= OnDirectionInput;
        _backwardMovement.OnPlayerFinish -= HandlePlayerFinish;
    }

    private void Awake()
    {
        _repeatShooting = _defaultRepeatShooting;
        _playerView = GetComponent<PlayerView>();
        _playerRotation = GetComponent<PlayerRotation>();
        _policeOfficers = new Dictionary<int, PoliceOfficer>();
        _backwardMovement = GetComponent<PlayerBackwardMovement>();
        _playerSideMovement = GetComponent<PlayerSideMovement>();
        _positionGeneratorSquad = GetComponent<PositionGeneratorSquad>();
    }

    private void Update()
    {
        if (_moveTarget != null && _isMoving)
        {
            _backwardMovement.MoveBackward();
        }
    }
    
    private void OnDirectionInput(float direction)
    {
        _playerSideMovement.Move(direction);
    }
    
    public void SetupObjectsPool(BulletPool bulletPool, ChunkPool chunkPool)
    {
        _playerInput.DirectionChanged += OnDirectionInput;
        _bulletPool = bulletPool;
        _chunkPool = chunkPool;
    }

    public void ApplyBuffToParty(float buffDuration, float repeatShooting)
    {
        _effectPlexus.Play();
        _repeatShooting = repeatShooting;
        
        foreach (PoliceOfficer policeOfficer in _policeOfficers.Values)
        {
            policeOfficer.Shooting(_repeatShooting);
        }

        if (_coroutineBuff != null)
        {
            StopCoroutine(_coroutineBuff);
        }

        _coroutineBuff = StartCoroutine(BuffTimer(buffDuration));
    }
    
    public void SetNextTargetPosition(Transform moveTarget, BridgeDirection direction)
    {
        _currentDirection = direction;
        _isMoving = true;
        _moveTarget = moveTarget;
        _backwardMovement.SetTargetPosition(_moveTarget);
    }
    
    public void SetPlayerFinishPosition(Transform moveTarget, bool isFinished, BridgeDirection direction)
    {
        _currentDirection = direction;
        _isMoving = true;
        _isFinished = isFinished;
        _moveTarget = moveTarget;
        _backwardMovement.SetTargetPosition(_moveTarget);
    }
    
    public void UpdatePoliceHorizontalStatus()
    {
        switch (_currentDirection)
        {
            case BridgeDirection.VerticalUp:
                _maxBorderPosition =_moveTarget.position.x + _borderOffset;
                _minBorderPosition =_moveTarget.position.x - _borderOffset;
                break;
            
            default:
                _maxBorderPosition =_moveTarget.position.z + _borderOffset;
                _minBorderPosition =_moveTarget.position.z - _borderOffset;
                break;
        }
        
        foreach (PoliceOfficer policeOfficer in _policeOfficers.Values)
        {
            policeOfficer.SetHorizontalAndBorderStatus(_currentDirection, _minBorderPosition, _maxBorderPosition);
        }
    }
    
    public void RotateInDirection(BridgeDirection rotation)
    {
        _playerRotation.Rotate(rotation);
    }

    public void CeaseSquadFire()
    {
        _isStoppedWar = true;
        
        foreach (PoliceOfficer policeOfficer in _policeOfficers.Values)
        {
            policeOfficer.StopShooting();
        }
    }

    public void SpawnPoliceOfficer(int spawnUnitCount)
    {
        for (int i = 0; i < spawnUnitCount; i++)
        {
            Vector3 positionToGroup = _positionGeneratorSquad.CreateNextSpawnPosition();

            PoliceOfficer policeOfficer = _officerSpawner.CreatePoliceUnits();
            
            
            if (_isPoliceOfficerOnBase == false)
            {
                _currentState = ProgressionState.Idle;
                policeOfficer.transform.parent = _positionSpawnPoliceOfficer;
                policeOfficer.transform.localPosition = _positionSpawnPoliceOfficer.localPosition;
                
                policeOfficer.transform.rotation = _positionSpawnPoliceOfficer.rotation;
                policeOfficer.SetRotationPositionToDamageBridge(_positionSpawnPoliceOfficer.localRotation);
                policeOfficer.SetTargetPositionInGroup(positionToGroup);
            }
            else
            {
                _currentState = ProgressionState.BaseEntryCompleted;
                _base.SetTargetPoliceHelps(policeOfficer);
            }
            
            policeOfficer.SetPoliceOfficerActive(_bulletPool, _chunkPool, _currentState);
            policeOfficer.SetNumberOfficer(_numberOfficer);
            policeOfficer.SetHorizontalAndBorderStatus(_currentDirection, _minBorderPosition, _maxBorderPosition);
            policeOfficer.SetSpeed(_backwardMovement.Speed);
            policeOfficer.SetHealthPoint(_healthPoliceOfficer);
            policeOfficer.OnPoliceDeath += HandlePoliceDeath;
            
            if (_isStoppedWar == false)
            {
                policeOfficer.Shooting(_repeatShooting);
            }
            
            _policeOfficers.Add(policeOfficer.OfficerId, policeOfficer);
            _numberOfficer++;
            _policeCount = _policeOfficers.Count;
            _playerView.ShowPoliceCount(_policeCount);
        }
    }
    
    public void StopMoving()
    {
        _playerInput.DirectionChanged -= OnDirectionInput;
        _isMoving = false;
    }

    public void KeepMoving()
    {
        _playerInput.DirectionChanged += OnDirectionInput;
        _isMoving = true;
    }
    
    public void ResetPositionUiElements()
    {
        _playerInput.DirectionChanged -= OnDirectionInput;
        _playerSideMovement.ResetPositionSquad();
    }

    public void HandlePoliceReachedBase(Base basePolice)
    {
        if (_isPoliceOfficerOnBase || basePolice == null) return;

        _playerInput.DirectionChanged -= OnDirectionInput;
        _base = basePolice;
        _isPoliceOfficerOnBase = true;
        _currentState = ProgressionState.MovingToBase;
        
        if (_coroutineReorganize != null)
        {
            StopCoroutine(_coroutineReorganize);
        }
        
        foreach (PoliceOfficer policeOfficer in _policeOfficers.Values)
        {
            _base.SetTargetPoliceOfficers(policeOfficer);
            policeOfficer.SetFinishingTargets(_base.BaseEntryTransform.localPosition, _base.StartPositionGeneration, _currentState);
        }
            
        OnPlayerReachedBase?.Invoke();
    }

    private void HandlePlayerFinish()
    {
        if (_isFinished)
        {
            _isMoving = false;
        }
        else
        {
            OnCheckpointReached?.Invoke();
        }
    }
    
    private IEnumerator BuffTimer(float buffDuration)
    {
        _buffTimerBar.gameObject.SetActive(true);
        float elapsed = 0f;
        float maxFillAmount = 1;

        while (elapsed < buffDuration)
        {
            _buffTimerBar.fillAmount = maxFillAmount - (elapsed / buffDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        _buffTimerBar.gameObject.SetActive(false);
        _buffTimerBar.fillAmount = 0;
        _repeatShooting = _defaultRepeatShooting;

        if (_isStoppedWar == false)
        {
            foreach (PoliceOfficer policeOfficer in _policeOfficers.Values)
            {
                policeOfficer.Shooting(_repeatShooting);
            }
        }
        
        _coroutineBuff = null;
    }

    private IEnumerator ReorganizeSquadAfterDeath()
    {
        WaitForSeconds delay = new WaitForSeconds(_reorganizationDelay);
        
        _positionGeneratorSquad.ResetAllPositions();

        yield return delay;
        
        foreach (PoliceOfficer policeOfficer in _policeOfficers.Values)
        {
            Vector3 positionToGroup = _positionGeneratorSquad.CreateNextSpawnPosition();
            policeOfficer.SetTargetPositionInGroup(positionToGroup);
        }

        _coroutineReorganize = null;
    }

    private void HandlePoliceDeath(PoliceOfficer officer)
    {
        officer.OnPoliceDeath -= HandlePoliceDeath;
        officer.transform.parent = null;
        _policeOfficers.Remove(officer.OfficerId);
        _policeCount = _policeOfficers.Count;
        _playerView.ShowPoliceCount(_policeCount);
        OnUnitDied?.Invoke(_policeCount);
        
        if (_policeCount == 0)
        {
            if (_coroutineReorganize != null)
            {
                StopCoroutine(_coroutineReorganize);
                _coroutineReorganize = null;
            }
            
            _positionGeneratorSquad.ResetAllPositions();
            OnAllPoliceOfficersDied?.Invoke();
        }
        
        if(_isPoliceOfficerOnBase == false && _policeCount > 0)
        {
            if (_coroutineReorganize != null)
            {
                StopCoroutine(_coroutineReorganize);
            }
            
            _coroutineReorganize = StartCoroutine(ReorganizeSquadAfterDeath());
        }
    }
}