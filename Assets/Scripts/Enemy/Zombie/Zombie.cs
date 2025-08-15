using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private ZombieVision _zombieVision;
    [SerializeField] private ZombieSensor _zombieSensor;
    
    private Health _health = new Health();
    private ZombieWeapon _zombieWeapon;
    private ZombieMovement _zombieMovement;
    private ZombieRotation _zombieRotation;
    private ZombieJump _zombieJump;
    private bool _isTargetVisible;
    private PoliceOfficer _policeOfficer;
    private Transform _target;
    private Transform _finish;
    private Transform _transform;
    private Transform _enterToBase;
    private Transform _startPositionToScanEnemy;
    private bool _isBaseEntryCompleted;
    private bool _isAttacking;
    private bool _isKilledByBullet;

    public bool IsKilledByBullet => _isKilledByBullet;

    public event Action<Zombie> OnZombieDeath;

    private void Awake()
    {
        _transform = transform;
        _zombieWeapon = GetComponent<ZombieWeapon>();
        _zombieJump = GetComponent<ZombieJump>();
        _zombieMovement = GetComponent<ZombieMovement>();
        _zombieRotation = GetComponent<ZombieRotation>();
    }

    private void OnEnable()
    {
        _zombieMovement.OnTargetReached += HandleTargetReached;
        _zombieVision.OnPoliceDetected += ToggleVisionOnPoliceDetected;
        _zombieSensor.OnBaseDetected += ToggleVisionOnBaseDetected;
        _zombieSensor.OnBridgeDestroyedDetected += ReactToBridgeDestruction;
        _zombieSensor.OnPoliceContact += ApplyDamageToPolice;
        _zombieWeapon.OnAttackFinished += ResolveZombieAttack;
    }

    private void OnDisable()
    {
        _zombieMovement.OnTargetReached -= HandleTargetReached;
        _zombieVision.OnPoliceDetected -= ToggleVisionOnPoliceDetected;
        _zombieSensor.OnBaseDetected -= ToggleVisionOnBaseDetected;
        _zombieSensor.OnBridgeDestroyedDetected -= ReactToBridgeDestruction;
        _zombieSensor.OnPoliceContact -= ApplyDamageToPolice;
        _zombieWeapon.OnAttackFinished -= ResolveZombieAttack;
    }

    private void Update()
    {
        EvaluateTargetChange();
        _zombieMovement.Move(_target); 
        _zombieRotation.Rotate(_target.transform.position);
        
    }

    public void SetMovementSpeed()
    {
        _zombieMovement.ResetSpeed();
    }
    
    public void SetFinish(Transform finish)
    {
        _finish = finish;
    }
    
    public void SetHealthPoint(int healthPoint)
    {
        _health.SetHealthPoint(healthPoint);
    }
    
    public void SetScanEnemy()
    {
        _isKilledByBullet = false;
        _enterToBase = null;
        _target = null;
        _isAttacking = false;
        _isTargetVisible = false;
        _isBaseEntryCompleted = false;
    }

    public void TakeDamage(int damage)
    {
        if (_isAttacking == false)
        {
            _health.TakeDamage(damage);
        
            if (_health.HealthPoint <= 0)
            {
                _isKilledByBullet = true;
                _zombieVision.ScanOff();
                OnZombieDeath?.Invoke(this);
            }
        }
    }
    
    private void EvaluateTargetChange()
    {
        if (_isTargetVisible == false)
        {
            _target = _finish;
            
            if (_isBaseEntryCompleted == false && _enterToBase == null && _isAttacking == false)
            {
                _zombieVision.ScanForEnemies();
            }
        }
        else
        {
            if (_policeOfficer != null && _policeOfficer.IsDead == false)
            {
                _target = _policeOfficer.transform;
            }
            else
            {
                if(_isAttacking)
                {
                    _target = _policeOfficer.transform;
                }
                else
                {
                    _target = _finish;
                    _zombieMovement.ResetSpeed();
                    _zombieVision.ScanForEnemies();
                }
            }
        }
    }
    
    private void ResolveZombieAttack()
    {
        _zombieMovement.SetSpeed(_policeOfficer.SpeedOfficer);
        _zombieVision.ScanOff();
        OnZombieDeath?.Invoke(this);
    }
    
    private void HandleTargetReached()
    {
        if (_isBaseEntryCompleted)
        {
            _finish.position = new Vector3(_startPositionToScanEnemy.position.x, _transform.position.y, _startPositionToScanEnemy.position.z);
            _isBaseEntryCompleted = false;
        }
        else
        {
            _enterToBase = null;
        }
    }
    
    private void ToggleVisionOnBaseDetected(Base obj)
    {
        _zombieMovement.ResetSpeed();
        _enterToBase = obj.BaseEntryTransform;
        _startPositionToScanEnemy = obj.StartPositionGeneration;
        _isBaseEntryCompleted = true;
        _finish.position = new Vector3(_enterToBase.position.x, _transform.position.y, _enterToBase.position.z); 
        _isTargetVisible = false;
    }
    
    private void ApplyDamageToPolice(PoliceOfficer policeOfficer)
    {
        if (_isAttacking == false)
        {
            _isAttacking = true;
            _policeOfficer = policeOfficer;
            _target = _policeOfficer.transform;
            _zombieWeapon.Attack(_policeOfficer);
        }
    }
    
    private void ReactToBridgeDestruction()
    {
        _zombieJump.Jump();
    }
    
    private void ToggleVisionOnPoliceDetected(PoliceOfficer policeOfficer)
    {
        _policeOfficer = policeOfficer;
        _target = _policeOfficer.transform;
        _isTargetVisible = true;
        _zombieMovement.Accelerate();
    }
}
