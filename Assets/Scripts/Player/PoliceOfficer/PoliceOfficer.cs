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
    private PoliceOfficerSound _policeOfficerSound;
    private Health _health = new Health();
    private Weapon _weapon;
    private BulletPool _bulletPool;
    private ChunkPool _chunkPool;
    private Coroutine _coroutineRepeatShooting;
    private float _repeatShoot;
    private Transform _targetCenterPoint;
    private int _officerId;
    private bool _isDead;
    private bool _isFallingToDie;
    private bool _isReachedBaseEntry;
    private bool _isFoundBase;
    private bool _isBaseEntryCompleted;
    
    public int OfficerId => _officerId;
    public float SpeedOfficer => _officerMovement.Speed;
    public bool IsDead => _isDead;
    public bool IsFoundBase => _isFoundBase;

    public event Action<PoliceOfficer> OnPoliceDeath;
    public event Action<PoliceOfficer> OnDeathAnimationFinished;
    public event Action<PoliceOfficer> OnPoliceReachedToGeneratePositionOnBase;
    public event Action<PoliceOfficer, Base> OnPoliceReachedBase;
    
    private void Awake()
    {
        _policeOfficerSound = GetComponent<PoliceOfficerSound>();
        _officerMovement = GetComponent<PoliceOfficerMovement>();
        _weapon = GetComponentInChildren<Weapon>();
    }

    private void OnEnable()
    {
        _officerMovement.OnReachedRegroupPoint += SubscribeAndHandleBaseDetection;
    }

    private void OnDisable()
    {
        _officerMovement.OnReachedRegroupPoint -= SubscribeAndHandleBaseDetection;
    }

    private void Update()
    {
        _officerMovement.Move(_isFoundBase);
    }

    public void TakeDamage(int damage)
    {
        _health.TakeDamage(damage);

        if (_health.HealthPoint <= 0)
        {
            _isDead = true;
            _officerMovement.StopMove(false);
            
            OnPoliceDeath?.Invoke(this);
            
            if (_coroutineRepeatShooting != null)
            {
                StopCoroutine(_coroutineRepeatShooting);
            }
                    
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
        _bulletPool = bulletPool;
        _isDead = false;
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

    public void SetHealthPoint(int healthPoint)
    {
        _health.SetHealthPoint(healthPoint);
    }

    public void SetCenterPoint(Transform point)
    {
        _policeOfficerSound.PlayScream();
        _targetCenterPoint = point;

        if (_coroutineRepeatShooting != null)
        {
            StopCoroutine(_coroutineRepeatShooting);
        }

        StartCoroutine(TurnModelToCenter());
    }

    public void Shooting(float repeat)
    {
        _repeatShoot = repeat;
        
        _coroutineRepeatShooting = StartCoroutine(RepeatShooting());
    }

    public void StopShooting()
    {
        if (_coroutineRepeatShooting != null)
        {
            StopCoroutine(_coroutineRepeatShooting);
        }
    }
    
    public void ChangeFireRate(float repeat)
    {
        _repeatShoot = repeat;
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
                }
                else
                {
                    OnPoliceReachedToGeneratePositionOnBase?.Invoke(this);
                    _isBaseEntryCompleted = true;
                }
            }
        }
    }

    public void AssignPoliceDestination(Base basePolice)
    {
        _startPositionGenerationSquad = basePolice.StartPositionGeneration;
        _isFoundBase = true;
        _isReachedBaseEntry = true;
        _officerMovement.SetTargetPosition(basePolice.BaseEntryTransform.localPosition, true); 
        OnPoliceReachedBase?.Invoke(this, basePolice);
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

    private IEnumerator RepeatShooting()
    {
        if (_bulletPool != null)
        {
            while (enabled)
            {
                _bulletPool.GetBullet(_weapon.BulletSpawnPosition);
                _policeOfficerSound.PlayShoot();
                
                yield return new WaitForSeconds(_repeatShoot);
            }
        }
    }
}
