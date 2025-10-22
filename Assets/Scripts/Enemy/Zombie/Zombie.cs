using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private ZombieVision _zombieVision;
    [SerializeField] private ZombieSensor _zombieSensor;
    
    private Health _health = new Health();
    private EnemyWave _enemyWave;
    private ZombieWeapon _zombieWeapon;
    private ZombieMovement _zombieMovement;
    private ZombieRotation _zombieRotation;
    private ZombieJump _zombieJump;
    private PoliceOfficer _policeOfficer;
    private Transform _target;
    private Transform _finish;
    private Transform _transform;
    private Transform _enterToBase;
    private Transform _startPositionToScanEnemy;
    private bool _isMoving;
    private bool _isBaseEntryCompleted;
    private bool _isKilledByBullet;
    private bool _isFoundBase;
    private bool _isAttacked;
    private int _id;
    private int _stopSpeed;

    public int Id => _id;
    public bool IsKilledByBullet => _isKilledByBullet;

    public event Action<Zombie> OnZombieDeath;

    private void Awake()
    {
        _transform = transform;
        _enemyWave = GetComponent<EnemyWave>();
        _zombieWeapon = GetComponent<ZombieWeapon>();
        _zombieJump = GetComponent<ZombieJump>();
        _zombieMovement = GetComponent<ZombieMovement>();
        _zombieRotation = GetComponent<ZombieRotation>();
    }

    private void OnEnable()
    {
        _zombieWeapon.OnAttackFinished += ReactToFinishedAttack;
        _zombieMovement.OnTargetReached += HandleTargetReached;
        _zombieVision.OnPoliceDetected += ToggleVisionOnPoliceDetected;
        _zombieSensor.OnBaseDetected += ToggleVisionOnBaseDetected;
        _zombieSensor.OnBridgeDestroyedDetected += ReactToBridgeDestruction;
        _zombieSensor.OnPoliceContact += ApplyDamageToPolice;
    }

    private void OnDisable()
    {
        _zombieWeapon.OnAttackFinished -= ReactToFinishedAttack;
        _zombieMovement.OnTargetReached -= HandleTargetReached;
        _zombieVision.OnPoliceDetected -= ToggleVisionOnPoliceDetected;
        _zombieSensor.OnBaseDetected -= ToggleVisionOnBaseDetected;
        _zombieSensor.OnBridgeDestroyedDetected -= ReactToBridgeDestruction;
        _zombieSensor.OnPoliceContact -= ApplyDamageToPolice;
    }

    private void Update()
    {
        _zombieMovement.Move(_isMoving, _target); 
        _zombieRotation.Rotate(_target);
    }
    
    public void SetFinish(Transform finish)
    {
        _finish = finish;
    }
    
    public void SetHealthPoint(int healthPoint)
    {
        _health.SetHealthPoint(healthPoint);
    }
    
    public void SetSettingsToActivateUnit()
    {
        _target = _finish;
        _isMoving = true;
        _enemyWave.ResumeAnimation();
        _zombieMovement.ResetSpeed();
        _isAttacked = false;
        _isKilledByBullet = false;
        _enterToBase = null;
        _policeOfficer = null;
        _isBaseEntryCompleted = false;
        _zombieVision.ScanForEnemies();
    }
    
    public void SetNumberEnemy(int zombieId)
    {
        _id = zombieId;
    }

    public void TakeDamage(int damage)
    {
        _health.TakeDamage(damage);
        
        if (_health.HealthPoint <= 0)
        {
            _isKilledByBullet = true;
            _zombieVision.ScanOff();
            OnZombieDeath?.Invoke(this);
        }
    }
    
    public void StopMoving()
    {
        _isMoving = false;
        _enemyWave.PauseAnimation();
    }
    
    private void ReactToFinishedAttack(bool isKilled)
    {
        if (isKilled)
        {
            if (_policeOfficer != null)
            {
                _policeOfficer.OnPoliceDeath -= LookingNewTarget;
                _policeOfficer = null;
            }
            
            OnZombieDeath?.Invoke(this);
        }
        else
        {
            _isAttacked = false;
            _zombieVision.ScanForEnemies();
        }
    }
    
    private void HandleTargetReached()
    {
        if (_isFoundBase)
        {
            if (_isBaseEntryCompleted)
            {
                _finish.position = new Vector3(_startPositionToScanEnemy.position.x, _transform.position.y, _startPositionToScanEnemy.position.z);
                _target = _finish;
                _isBaseEntryCompleted = false;
            }
            else
            {
                _zombieVision.ScanForEnemies();
                _enterToBase = null;
            }
        }
        else
        {
            _target = _finish;
        }
    }
    
    private void ToggleVisionOnBaseDetected(Base policeBase)
    {
        if (_isFoundBase == false)
        {
            _isFoundBase = true;
            _zombieMovement.ResetSpeed();
            _zombieVision.ScanOff();
            _enterToBase = policeBase.BaseEntryTransform;
            _startPositionToScanEnemy = policeBase.StartPositionGeneration;
            _isBaseEntryCompleted = true;
            _finish.position = new Vector3(_enterToBase.position.x, _transform.position.y, _enterToBase.position.z);
            _target = _finish;
        }
    }

    private void ApplyDamageToPolice(PoliceOfficer policeOfficer)
    {
        if (_isAttacked == false)
        {
            _isAttacked = true;
            _zombieMovement.SetSpeed(policeOfficer.SpeedOfficer);
            _zombieWeapon.Attack(policeOfficer);
        }
    }
    
    private void ReactToBridgeDestruction()
    {
        _zombieJump.Jump();
    }
    
    private void ToggleVisionOnPoliceDetected(PoliceOfficer policeOfficer)
    {
        _policeOfficer = policeOfficer;
        _policeOfficer.OnPoliceDeath += LookingNewTarget;
        _target = _policeOfficer.transform;
        _zombieMovement.Accelerate();
    }

    private void LookingNewTarget(PoliceOfficer police)
    {
        police.OnPoliceDeath -= LookingNewTarget;
        _policeOfficer = null;
        
        if (gameObject.activeInHierarchy)
        {
            _target = _finish;
            _zombieMovement.ResetSpeed();
            _zombieVision.ScanForEnemies();
        }
    }
}