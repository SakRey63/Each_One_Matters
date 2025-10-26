using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Base : MonoBehaviour
{
    [SerializeField] private Transform _baseEntryTransform;
    [SerializeField] private Transform _startPositionGeneration;
    [SerializeField] private Transform _endPositionToPlayer;
    [SerializeField] private Transform _parentToSquad;
    [SerializeField] private Transform _spawnHelpPoliceOfficer;
    [SerializeField] private float _xOffset = 5.5f;

    private PositionGenerationBase _positionGenerationBase;

    public event Action<Base> OnPoliceOfficerDetected;

    private void Awake()
    {
        _positionGenerationBase = GetComponent<PositionGenerationBase>();
    }

    public Transform BaseEntryTransform => _baseEntryTransform;
    public Transform StartPositionGeneration => _startPositionGeneration;
    public Transform EndPositionPlayer => _endPositionToPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PoliceOfficer>(out _))
        {
            OnPoliceOfficerDetected?.Invoke(this);
        }
    }

    public void SetTargetPoliceOfficers(PoliceOfficer policeOfficer)
    {
        policeOfficer.OnPoliceReachedToGeneratePositionOnBase += AssignNewPositionOnBase;
        policeOfficer.transform.parent = _parentToSquad;
    }
    
    public void SetTargetPoliceHelps(PoliceOfficer policeOfficer)
    {
        policeOfficer.transform.parent = _parentToSquad;
        policeOfficer.transform.localPosition = GetRandomPositionToBase();
        policeOfficer.transform.rotation = _spawnHelpPoliceOfficer.rotation;
            
        Vector3 positionOnBase = _positionGenerationBase.GetPositionOnBase(_startPositionGeneration.localPosition);
        policeOfficer.SetTargetPositionInGroup(positionOnBase);
    }

    private Vector3 GetRandomPositionToBase()
    {
        Vector3 randomSpawnPosition = new Vector3(Random.Range(-_xOffset, _xOffset), _spawnHelpPoliceOfficer.localPosition.y, _spawnHelpPoliceOfficer.localPosition.z);
        return randomSpawnPosition;
    }
    
    private void AssignNewPositionOnBase(PoliceOfficer policeOfficer)
    {
        policeOfficer.OnPoliceReachedToGeneratePositionOnBase -= AssignNewPositionOnBase;
        Vector3 positionOnBase = _positionGenerationBase.GetPositionOnBase(_startPositionGeneration.localPosition);
        policeOfficer.SetTargetPositionInGroup(positionOnBase);
    }
}