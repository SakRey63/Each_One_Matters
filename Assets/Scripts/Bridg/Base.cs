using System;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Transform _baseEntryTransform;
    [SerializeField] private Transform _startPositionGeneration;
    [SerializeField] private Transform _endPositionToPlayer;
    [SerializeField] private Transform _parentToSquad;

    private PositionGenerationBase _positionGenerationBase;

    private void Awake()
    {
        _positionGenerationBase = GetComponent<PositionGenerationBase>();
    }

    public Transform BaseEntryTransform => _baseEntryTransform;
    public Transform StartPositionGeneration => _startPositionGeneration;
    public Transform EndPositionPlayer => _endPositionToPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PoliceOfficer policeOfficer))
        {
            policeOfficer.transform.parent = _parentToSquad;
            policeOfficer.AssignPoliceDestination(_baseEntryTransform, _startPositionGeneration);
            policeOfficer.OnPoliceReachedToGeneratePositionOnBase += AssignNewPositionOnBase;
            
        }
    }
    
    private void AssignNewPositionOnBase(PoliceOfficer policeOfficer)
    {
        policeOfficer.OnPoliceReachedToGeneratePositionOnBase -= AssignNewPositionOnBase;
        Vector3 positionOnBase = _positionGenerationBase.GetPositionOnBase(_startPositionGeneration.localPosition);
        policeOfficer.SetTargetPositionInGroup(positionOnBase);
    }
}
