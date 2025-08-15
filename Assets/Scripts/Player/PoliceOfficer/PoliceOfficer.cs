using System;
using System.Collections;
using UnityEngine;

public class PoliceOfficer : MonoBehaviour
{
    
    [SerializeField] private float _delay = 1f;
    [SerializeField] private float _tiltAngle = 90f;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private Transform _basePoint;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Color _color;

    private Transform _startPositionGenerationSquad;
    private Quaternion _rotationPositionToDamageBridge;
    private PoliceOfficerMovement _officerMovement;
    private Health _health = new Health();
    private Weapon _weapon;
    private BulletPool _bulletPool;
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
    public event Action<PoliceOfficer> OnPoliceReachedBase;
    
    private void Awake()
    {
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
        _officerMovement.Move();
    }

    public void TakeDamage(int damage)
    {
        _health.TakeDamage(damage);

        if (_health.HealthPoint <= 0)
        {
            _isDead = true;
            _officerMovement.StopMove(false);

            gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            
            OnPoliceDeath?.Invoke(this);
            
            if (_coroutineRepeatShooting != null)
            {
                StopCoroutine(_coroutineRepeatShooting);
            }
                    
            StartCoroutine(PlayDeathAnimationAndReturn());
        }
    }

    public void SetRotationPositionToDamageBridge(Quaternion rotation)
    {
        _rotationPositionToDamageBridge = rotation;
    }
    public void SetPoliceOfficerActive(BulletPool bulletPool)
    {
        _bulletPool = bulletPool;
        _meshRenderer.material.color = _color;
        gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        _isDead = false;
        _isFoundBase = false;
        _isBaseEntryCompleted = false;
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
        _targetCenterPoint = point;

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
                }
                else
                {
                    OnPoliceReachedToGeneratePositionOnBase?.Invoke(this);
                    _isBaseEntryCompleted = true;
                }
            }
        }
    }

    public void AssignPoliceDestination(Transform enterBase, Transform startGenerationSquad)
    {
        _startPositionGenerationSquad = startGenerationSquad;
        _isFoundBase = true;
        _isReachedBaseEntry = true;
        _officerMovement.SetTargetPosition(enterBase.localPosition, true); 
        OnPoliceReachedBase?.Invoke(this);
    }
    
    private IEnumerator TurnModelToCenter()
    {
        _isDead = true;
        _officerMovement.StopMove(false);
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
        _meshRenderer.material.color = Color.red;
        
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
                
                yield return new WaitForSeconds(_repeatShoot);
            }
        }
    }
}
