using System;
using System.Collections;
using UnityEngine;

public class PoliceOfficer : MonoBehaviour
{
    [SerializeField] private float _delay = 1f;
    [SerializeField] private float _tiltAngle = 90f;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private Transform _basePoint;
    [SerializeField] private Animations _animations;

    private Transform _startPositionGenerationSquad;
    private Quaternion _rotationPositionToDamageBridge;
    private PoliceOfficerMovement _officerMovement;
    private PoliceOfficerRotation _officerRotation;
    private PoliceOfficerVision _officerVision;
    private PoliceOfficerSound _policeOfficerSound;
    private Health _health = new Health();
    private Weapon _weapon;
    private ChunkPool _chunkPool;
    private Transform _targetCenterPoint;
    private int _officerId;
    private bool _isDead;
    private bool _isFallingToDie;
    private bool _isReachedBaseEntry;
    private bool _isFoundBase;
    private bool _isBaseEntryCompleted;
    private bool _isLeadingZombie;
    
    public int OfficerId => _officerId;
    public float SpeedOfficer => _officerMovement.Speed;
    public bool IsDead => _isDead;

    public event Action<PoliceOfficer> OnPoliceDeath;
    public event Action<PoliceOfficer> OnDeathAnimationFinished;
    public event Action<PoliceOfficer> OnPoliceReachedToGeneratePositionOnBase;
    
    private void Awake()
    {
        _officerVision = GetComponent<PoliceOfficerVision>();
        _officerRotation = GetComponent<PoliceOfficerRotation>();
        _policeOfficerSound = GetComponent<PoliceOfficerSound>();
        _officerMovement = GetComponent<PoliceOfficerMovement>();
        _weapon = GetComponentInChildren<Weapon>();
    }

    private void OnEnable()
    {
        _officerVision.OnZombieDetected += ToggleVisionOnZombieDetected;
        _officerMovement.OnReachedRegroupPoint += SubscribeAndHandleBaseDetection;
    }

    private void OnDisable()
    {
        _officerVision.OnZombieDetected -= ToggleVisionOnZombieDetected;
        _officerMovement.OnReachedRegroupPoint -= SubscribeAndHandleBaseDetection;
    }

    private void Update()
    {
        _officerMovement.Move(_isFoundBase);

        if (_isLeadingZombie)
        {
            _officerRotation.Rotation();
        }
    }

    public void TakeDamage(int damage)
    {
        _health.TakeDamage(damage);

        if (_health.HealthPoint <= 0)
        {
            _isDead = true;
            _officerMovement.StopMove(false);
            _officerVision.ScanOff();
            _weapon.StopShooting();
            OnPoliceDeath?.Invoke(this);
            
            _chunkPool.GetEffect(transform);
            OnDeathAnimationFinished?.Invoke(this);
        }
    }

    public void OnDeathAnimationComplete()
    {
        _basePoint.localRotation = Quaternion.identity;
        OnDeathAnimationFinished?.Invoke(this);
    }

    public void SetRotationPositionToDamageBridge(Quaternion rotation)
    {
        _rotationPositionToDamageBridge = rotation;
    }
    
    public void SetPoliceOfficerActive(BulletPool bulletPool, ChunkPool chunkPool, bool isBaseEntryCompleted)
    {
        _chunkPool = chunkPool;
        _weapon.SetBulletPool(bulletPool, _policeOfficerSound);
        _isDead = false;
        _isLeadingZombie = false;
        _isFoundBase = isBaseEntryCompleted;
        _isBaseEntryCompleted = isBaseEntryCompleted;
    }

    public void SetTargetPositionInGroup(Vector3 position)
    {
        _officerMovement.SetTargetPosition(position, true);
    }

    public void SetNumberOfficer(int number)
    {
        _officerId = number;
    }

    public void ScanningEnemiesActive()
    {
        _officerVision.ScanForEnemies();
    }

    public void SetHealthPoint(int healthPoint)
    {
        _health.SetHealthPoint(healthPoint);
    }

    public void SetCenterPoint(Transform point)
    {
        _policeOfficerSound.PlayScream();
        _targetCenterPoint = point;
        _weapon.StopShooting();

        StartCoroutine(TurnModelToCenter());
    }

    public void Shooting(float repeat)
    {
        _weapon.Shooting(repeat);
    }

    public void StopShooting()
    {
        _animations.MoveWinAnimation();
        _weapon.StopShooting();
    }
    
    public void SetSpeed(float backwardMovementSpeed)
    {
        _animations.MoveRunAnimation(true);
        _officerMovement.SetSpeed(backwardMovementSpeed);
    }
    
    public void SetHorizontalAndBorderStatus(bool isHorizontal, float minBorderPosition, float maxBorderPosition)
    {
        _officerMovement.SetHorizontalAndBorder(isHorizontal, maxBorderPosition, minBorderPosition);
    }

    public void SetFinishingTargets(Vector3 enteredPosition, Transform startPositionGeneration)
    {
        _isFoundBase = true;
        _isReachedBaseEntry = true;
        _officerMovement.SetTargetPosition(enteredPosition, true);
        _startPositionGenerationSquad = startPositionGeneration;
    }
    
    private void ToggleVisionOnZombieDetected(Zombie zombie)
    {
        zombie.OnZombieDeath += LookingNewTarget;
        _officerRotation.SetTarget(zombie);
        _isLeadingZombie = true;
    }

    private void LookingNewTarget(Zombie zombie)
    {
        zombie.OnZombieDeath -= LookingNewTarget;
        _officerVision.ScanForEnemies();
        _isLeadingZombie = false;
    }

    private void SubscribeAndHandleBaseDetection()
    {
        if (_isFoundBase)
        {
            if (_isReachedBaseEntry)
            {
                _isReachedBaseEntry = false;
                _officerMovement.SetTargetPosition(_startPositionGenerationSquad.localPosition, true);
            }
            else
            {
                if (_isBaseEntryCompleted)
                {
                    _officerMovement.SetSpeed(0);
                    _animations.MoveRunAnimation(false);

                    if (_isLeadingZombie == false)
                    {
                        _officerVision.ScanForEnemies();
                    }
                }
                else
                {
                    OnPoliceReachedToGeneratePositionOnBase?.Invoke(this);
                    _isBaseEntryCompleted = true;
                }
            }
        }
    }
    
    private IEnumerator TurnModelToCenter()
    {
        _isDead = true;
        _officerMovement.StopMove(false);
        _animations.MoveFallingAnimation();
        OnPoliceDeath?.Invoke(this);
        transform.rotation = _rotationPositionToDamageBridge;
        
        Vector3 direction = _targetCenterPoint.position - _basePoint.position;
        Vector3 tiltAxis = Vector3.Cross(direction, Vector3.up);
        Quaternion startRotation = _basePoint.rotation;
        Quaternion targetRotation = startRotation * Quaternion.AngleAxis(_tiltAngle, tiltAxis);
        
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            float step = elapsed / _duration;
            _basePoint.rotation = Quaternion.Slerp(_basePoint.rotation, targetRotation, step);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        _basePoint.rotation = targetRotation;
        StartCoroutine(PlayDeathAnimationAndReturn());
    }

    private IEnumerator PlayDeathAnimationAndReturn()
    {
        yield return new WaitForSeconds(_delay);

        _basePoint.localRotation = Quaternion.identity;
        OnDeathAnimationFinished?.Invoke(this);
    }
}
