using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour
{
    [SerializeField] private int _policeCount = 1;
    [SerializeField] private float _repeatShooting = 0.8f;
    [SerializeField] private float _border = 5.7f;
    [SerializeField] private Transform _positionSpawnPoliceOfficer;
    [SerializeField] private float _delay = 0.7f;
    [SerializeField] private int _healthPoliceOfficer = 75;

    private Transform _moveTarget;
    private Transform _startPositionGenerationSquad;
    private PlayerBackwardMovement _backwardMovement;
    private PlayerRotation _playerRotation;
    private Dictionary<int, PoliceOfficer> _policeOfficers;
    private PlayerSideMovement _playerSideMovement;
    private PoleceOfficerSpawner _officerSpawner;
    private PositionGeneratorSquad _positionGeneratorSquad;
    private BulletPool _bulletPool;
    private PlayerView _playerView;
    private int _numberOfficer;
    private Coroutine _coroutine;
    private bool _isMoving;
    private bool _isFinished;
    private bool _isHorizontal;
    private bool _isPoliceOfficerOnBase;
    private float _maxBorderPosition;
    private float _minBorderPosition;
    
    public int PoliceCount => _policeCount;
    public Transform SquadPosition => _playerSideMovement.SquadPosition;
    public bool IsPoliceOfficerOnBase => _isPoliceOfficerOnBase;

    public event Action OnCheckpointReached;
    public event Action OnAllPoliceOfficersDead;
    public event Action OnPlayerReachedBase;

    private void OnEnable()
    {
        _backwardMovement.OnPlayerFinish += HandlePlayerFinish;
    }

    private void OnDisable()
    {
        _backwardMovement.OnPlayerFinish -= HandlePlayerFinish;
    }

    private void Awake()
    {
        _playerView = GetComponent<PlayerView>();
        _bulletPool = GetComponent<BulletPool>();
        _playerRotation = GetComponent<PlayerRotation>();
        _policeOfficers = new Dictionary<int, PoliceOfficer>();
        _backwardMovement = GetComponent<PlayerBackwardMovement>();
        _playerSideMovement = GetComponent<PlayerSideMovement>();
        _officerSpawner = GetComponent<PoleceOfficerSpawner>();
        _positionGeneratorSquad = GetComponent<PositionGeneratorSquad>();
    }

    private void Update()
    {
        if (_moveTarget != null && _isMoving)
        {
            _backwardMovement.MoveBackward();
        }
    }

    public void ApplyBuffToParty(float repeatShooting)
    {
        _repeatShooting = repeatShooting;

        foreach (PoliceOfficer policeOfficer in _policeOfficers.Values)
        {
            policeOfficer.ChangeFireRate(_repeatShooting);
        }
    }

    public void SetNextTargetPosition(Transform moveTarget, bool isHorizontal)
    {
        _isHorizontal = isHorizontal;
        _isMoving = true;
        _moveTarget = moveTarget;
        _backwardMovement.SetTargetPosition(_moveTarget);
    }
    
    public void SetPlayerFinishPosition(Transform moveTarget, bool isFinished, bool isHorizontal)
    {
        _isHorizontal = isHorizontal;
        _isFinished = isFinished;
        _isMoving = true;
        _moveTarget = moveTarget;
        _backwardMovement.SetTargetPosition(_moveTarget);
    }
    
    public void UpdatePoliceHorizontalStatus()
    {
        if (_isHorizontal)
        {
            _maxBorderPosition =_moveTarget.position.z + _border;
            _minBorderPosition =_moveTarget.position.z - _border;
        }
        else
        {
            _maxBorderPosition =_moveTarget.position.x + _border;
            _minBorderPosition =_moveTarget.position.x - _border;
        }
        
        foreach (PoliceOfficer policeOfficer in _policeOfficers.Values)
        {
            policeOfficer.SetHorizontalAndBorderStatus(_isHorizontal, _minBorderPosition, _maxBorderPosition);
        }
    }

    public void MoveSideways(float direction)
    {
        _playerSideMovement.Move(direction);
    }
    
    public void RotateInDirection(bool isTurnRight)
    {
        _playerRotation.Rotate(isTurnRight);
    }

    public void CeaseSquadFire()
    {
        foreach (PoliceOfficer policeOfficer in _policeOfficers.Values)
        {
            policeOfficer.StopShooting();
        }
    }

    public void SpawnPoliceOfficer(int spawnUnitCount)
    {
        for (int i = 0; i < spawnUnitCount; i++)
        {
            Vector3 positionToGroup = _positionGeneratorSquad.CreateNextSpawnPosition(_positionSpawnPoliceOfficer.localPosition);

            PoliceOfficer policeOfficer = _officerSpawner.CreatePoliceUnits(_positionSpawnPoliceOfficer, _bulletPool);
            policeOfficer.SetRotationPositionToDamageBridge(_positionSpawnPoliceOfficer.localRotation);
            policeOfficer.SetTargetPositionInGroup(positionToGroup);
            policeOfficer.Shooting(_repeatShooting);
            policeOfficer.SetNumberOfficer(_numberOfficer);
            policeOfficer.SetHealthPoint(_healthPoliceOfficer);
            policeOfficer.SetSpeed(_backwardMovement.Speed);
            policeOfficer.SetHorizontalAndBorderStatus(_isHorizontal, _minBorderPosition, _maxBorderPosition);
            policeOfficer.OnPoliceDeath += HandlePoliceDeath;
            policeOfficer.OnPoliceReachedBase += HandlePoliceReachedBase;
            _policeOfficers.Add(policeOfficer.OfficerId, policeOfficer);

            _numberOfficer++;
            _policeCount = _policeOfficers.Count;
            _playerView.ShowPoliceCount(_policeCount);
        }
    }

    private void HandlePoliceReachedBase(PoliceOfficer policeOfficer)
    {
        policeOfficer.OnPoliceReachedBase -= HandlePoliceReachedBase;

        if (_isPoliceOfficerOnBase == false)
        {
            OnPlayerReachedBase?.Invoke();
            _isPoliceOfficerOnBase = true;
        }
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

    private IEnumerator ReorganizeSquadAfterDeath()
    {
        WaitForSeconds delay = new WaitForSeconds(_delay);
        
        _positionGeneratorSquad.ResetAllPositions();

        yield return delay;
        
        foreach (PoliceOfficer policeOfficer in _policeOfficers.Values)
        {
            Vector3 positionToGroup = _positionGeneratorSquad.CreateNextSpawnPosition(_positionSpawnPoliceOfficer.localPosition);
            policeOfficer.SetTargetPositionInGroup(positionToGroup);
        }

        _coroutine = null;
    }

    private void HandlePoliceDeath(PoliceOfficer officer)
    {
        officer.OnPoliceDeath -= HandlePoliceDeath;
        officer.transform.parent = null;
        _policeOfficers.Remove(officer.OfficerId);
        _policeCount = _policeOfficers.Count;
        _playerView.ShowPoliceCount(_policeCount);
        
        if(_coroutine == null && officer.IsFoundBase == false)
        {
            _coroutine = StartCoroutine(ReorganizeSquadAfterDeath());
        }

        if (_policeCount == 0)
        {
            OnAllPoliceOfficersDead?.Invoke();
        }
    }
}
