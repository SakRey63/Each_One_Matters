using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _policeCount = 1;
    [SerializeField] private float _repeatShoting = 1;
    [SerializeField] private float _fireRateDivider = 0.5f;

    private List<PoliceOfficer> _policeOfficers;
    private PlayerSideMovement _playerSideMovement;
    
    public int PoliceCount => _policeCount;

    private void Awake()
    {
        _policeOfficers = new List<PoliceOfficer>();
        _playerSideMovement = GetComponent<PlayerSideMovement>();
        _policeOfficers.Add(GetComponentInChildren<PoliceOfficer>());
    }

    private void OnEnable()
    {
        foreach (PoliceOfficer police in _policeOfficers)
        {
            police.OnFireRateBoosted += ApplyBuffToParty;
        }
    }

    private void OnDisable()
    {
        foreach (PoliceOfficer police in _policeOfficers)
        {
            police.OnFireRateBoosted -= ApplyBuffToParty;
        }
    }

    private void Start()
    {
        foreach (PoliceOfficer police in _policeOfficers)
        {
            police.Shooting(_repeatShoting);
        }
    }

    private void ApplyBuffToParty()
    {
        _repeatShoting *= _fireRateDivider;
        
        foreach (PoliceOfficer police in _policeOfficers)
        {
            police.BoostFireRate(_repeatShoting);
        }
    }

    public void MoveSideways(float direction)
    {
        _playerSideMovement.Move(direction);
    }
}
