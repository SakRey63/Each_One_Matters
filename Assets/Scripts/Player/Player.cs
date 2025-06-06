using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour
{
    [SerializeField] private int _policeCount = 1;
    [SerializeField] private float _repeatShoting = 1;
    [SerializeField] private float _fireRateDivider = 0.5f;
    [SerializeField] private Transform _positionSpawnPoliceOfficer;
    [SerializeField] private float _delay = 0.7f;

    private Dictionary<int, PoliceOfficer> _policeOfficers;
    private PlayerSideMovement _playerSideMovement;
    private PoleceOfficerSpawner _officerSpawner;
    private PositionGenerator _positionGenerator;
    private int _numberOfficer;
    private Coroutine _coroutine;
    
    public int PoliceCount => _policeCount;

    private void Awake()
    {
        _policeOfficers = new Dictionary<int, PoliceOfficer>();
        _playerSideMovement = GetComponent<PlayerSideMovement>();
        _officerSpawner = GetComponent<PoleceOfficerSpawner>();
        _positionGenerator = GetComponent<PositionGenerator>();
    }

    public void ApplyBuffToParty()
    {
        _repeatShoting *= _fireRateDivider;
        
        Debug.Log("OnFireRateBooster " + _repeatShoting);

        foreach (var policeOfficer in _policeOfficers)
        {
            policeOfficer.Value.BoostFireRate(_repeatShoting);
        }
    }

    public void MoveSideways(float direction)
    {
        _playerSideMovement.Move(direction);
    }

    public void SpawnPoliceOfficer(int spawnUnitCount)
    {
        for (int i = 0; i < spawnUnitCount; i++)
        {
            Vector3 positionToGroup = _positionGenerator.CreateNextSpawnPosition(_positionSpawnPoliceOfficer.localPosition);
            
            PoliceOfficer policeOfficer = _officerSpawner.CreatePoliceUnits(_positionSpawnPoliceOfficer);
            policeOfficer.SetTargetPositionInGroup(positionToGroup);
            policeOfficer.Shooting(_repeatShoting);
            policeOfficer.SetNumberOfficer(_numberOfficer);
            policeOfficer.OnPoliceDeath += HandlePoliceDeath;
            _policeOfficers.Add(policeOfficer.OfficerId, policeOfficer);

            _numberOfficer++;
            _policeCount = _policeOfficers.Count;
        }
    }

    private IEnumerator ReorganizeSquadAfterDeath()
    {
        WaitForSeconds delay = new WaitForSeconds(_delay);
        
        _positionGenerator.ResetAllPositions();

        yield return delay;
        
        foreach (var policeOfficer in _policeOfficers)
        {
            Vector3 positionToGroup = _positionGenerator.CreateNextSpawnPosition(_positionSpawnPoliceOfficer.localPosition);
            policeOfficer.Value.SetTargetPositionInGroup(positionToGroup);
        }

        _coroutine = null;
    }

    private void HandlePoliceDeath(PoliceOfficer officer)
    {
        officer.OnPoliceDeath -= HandlePoliceDeath;
        officer.transform.parent = null;
        _policeOfficers.Remove(officer.OfficerId);
        _policeCount = _policeOfficers.Count;

        if(_coroutine == null)
        {
            _coroutine = StartCoroutine(ReorganizeSquadAfterDeath());
        }
    }
}
