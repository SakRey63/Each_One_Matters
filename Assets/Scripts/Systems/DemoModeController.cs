using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DemoModeController : MonoBehaviour
{
    [SerializeField] private Transform _policeSpawnPosition;
    [SerializeField] private Transform _zombieSpawnPosition;
    [SerializeField] private float _delay = 2;
    [SerializeField] private float _border = 5.5f;
    [SerializeField] private int _minCountUnits = 1;
    [SerializeField] private int _maxContSpawnZombie = 11;
    [SerializeField] private int _maxContSpawnPolice = 6;
    [SerializeField] private float _stepOffset = 1f;
    [SerializeField] private float _repeatShooting = 1f;
    [SerializeField] private int _healthPoint = 75;
    [SerializeField] private BulletPool _bulletPool;
    [SerializeField] private ChunkPool _chunkPool;
    [SerializeField] private SpawnerZombie _spawnerZombie;
    [SerializeField] private PoleceOfficerSpawner _officerSpawner;
    
    private Vector3 _nexPositionSpawn;
    private Vector3 _lastPositionToBaseRight;
    private Vector3 _lastPositionToBaseLeft;
    private bool _isStart;
    private bool _isRight;
    private bool _isLeft;
    private int _officerId;
    private int _zombieId;
    private int _countSpawnPolice;
    private int _countSpawnZombie;
    private Dictionary<int, PoliceOfficer> _activePoliceOfficers;
    private Dictionary<int, Zombie> _activeZombies;
    private Coroutine _coroutine;
    private SquadFormationPhase _currentPhase;
    
    private void Awake()
    {
        _activePoliceOfficers = new Dictionary<int, PoliceOfficer>();
        _activeZombies = new Dictionary<int, Zombie>();
    }

    public void SetupDemoScene()
    {
        StartDemoScene();
    }
    
    private void ReturnAllZombies()
    {
        foreach (Zombie zombie in _activeZombies.Values)
        {
            if (zombie.gameObject.activeSelf)
            {
                _spawnerZombie.ReturnDemoEnemies(zombie);
            }
        }
    }

    private void HandleAllZombieKilled(Zombie zombie)
    {
        zombie.OnZombieDeath -= HandleAllZombieKilled;
        _activeZombies.Remove(zombie.Id);
        
        if (_activeZombies.Count == 0 && _coroutine == null)
        {
            _coroutine = StartCoroutine(FindRemainingOnes());
        }
    }
    
    private void HandlePoliceDeath(PoliceOfficer officer)
    {
        officer.OnPoliceDeath -= HandlePoliceDeath;
        _activePoliceOfficers.Remove(officer.OfficerId);

        if (_activePoliceOfficers.Count == 0 && _coroutine == null)
        {
            _coroutine = StartCoroutine(FindRemainingOnes());
        }
    }

    private IEnumerator FindRemainingOnes()
    {
        WaitForSeconds delay = new WaitForSeconds(_delay);
        
        ReturnAllZombies();
        
        foreach (PoliceOfficer policeOfficer in _activePoliceOfficers.Values)
        {
            if (policeOfficer.gameObject.activeSelf)
            {
                policeOfficer.StopShooting(); 
            }
        }
        
        yield return delay;

        foreach (PoliceOfficer policeOfficer in _activePoliceOfficers.Values)
        {
            if (policeOfficer.gameObject.activeSelf)
            {
                policeOfficer.OnDeathAnimationComplete(); 
            }
        }
        
        _activePoliceOfficers.Clear();
        _activeZombies.Clear();
        StartDemoScene();
        _coroutine = null;
    }

    private void StartDemoScene()
    {
        _currentPhase = SquadFormationPhase.Centered;
        
        _countSpawnPolice = GetRandomCountUnits(_minCountUnits, _maxContSpawnPolice);
        _countSpawnZombie = GetRandomCountUnits(_minCountUnits, _maxContSpawnZombie);
        
        for (int i = 0; i < _countSpawnPolice; i++)
        {
            PoliceOfficer officer = _officerSpawner.CreatePoliceUnits();
            officer.transform.position = GetPositionOnBase(_policeSpawnPosition.position);
            officer.transform.rotation = _policeSpawnPosition.rotation;
            officer.SetPoliceOfficerActive(_bulletPool, _chunkPool, ProgressionState.BaseEntryCompleted);
            officer.Shooting(_repeatShooting);
            officer.SetNumberOfficer(_officerId);
            officer.ScanningEnemiesActive();
            officer.OnPoliceDeath += HandlePoliceDeath;
            _officerId++;
            _activePoliceOfficers.Add(officer.OfficerId, officer);
            
        }

        for (int j = 0; j < _countSpawnZombie; j++)
        {
            Zombie zombie = _spawnerZombie.CreateEnemy();
            zombie.OnZombieDeath += HandleAllZombieKilled;
            zombie.transform.position = new Vector3(_zombieSpawnPosition.position.x, _zombieSpawnPosition.position.y, Random.Range(_zombieSpawnPosition.position.z -_border, _zombieSpawnPosition.position.z + _border));
            zombie.transform.rotation = _zombieSpawnPosition.rotation;
            zombie.SetFinish(_policeSpawnPosition);
            zombie.SetHealthPoint(_healthPoint);
            zombie.SetSettingsToActivateUnit();
            zombie.SetNumberEnemy(_zombieId);
            _zombieId++;
            _activeZombies.Add(zombie.Id, zombie);
        }
    }

    private int GetRandomCountUnits(int minNumber, int maxNumber)
    {
        return Random.Range(minNumber, maxNumber);
    }
    
    private Vector3 GetPositionOnBase(Vector3 startPositionToBase)
    {
        switch (_currentPhase)
        {
            case SquadFormationPhase.Centered:
                _lastPositionToBaseRight = startPositionToBase;
                _lastPositionToBaseLeft= startPositionToBase;
                _nexPositionSpawn = startPositionToBase;
                _currentPhase = SquadFormationPhase.MovingRight;
                break;
            case SquadFormationPhase.MovingRight:
                _lastPositionToBaseRight = new Vector3(_lastPositionToBaseRight.x, _lastPositionToBaseRight.y,
                    _lastPositionToBaseRight.z + _stepOffset);
            
                _nexPositionSpawn = _lastPositionToBaseRight;
                _currentPhase = SquadFormationPhase.MovingLeft;
                
                break;
            case SquadFormationPhase.MovingLeft:
                _lastPositionToBaseLeft = new Vector3(_lastPositionToBaseLeft.x, _lastPositionToBaseLeft.y,
                    _lastPositionToBaseLeft.z - _stepOffset);
            
                _nexPositionSpawn = _lastPositionToBaseLeft;
                _currentPhase = SquadFormationPhase.MovingRight;
                break;
        }

        return _nexPositionSpawn;
    }
}