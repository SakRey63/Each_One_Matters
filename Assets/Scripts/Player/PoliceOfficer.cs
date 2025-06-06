using System;
using System.Collections;
using UnityEngine;

public class PoliceOfficer : MonoBehaviour
{
    [SerializeField] private float _speed = 6f;
    [SerializeField] private float _borderX = 5.7f;
    [SerializeField] private float _delay = 1f;
    [SerializeField] private float _tiltAngle = 45f;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private Transform _basePoint;
    
    private Weapon _weapon;
    private BulletPool _bulletPool;
    private Coroutine _coroutineRepeatShooting;
    private float _repeatShoot;
    private Vector3 _targetPositionInGroup;
    private Vector3 _borderPosition;
    private Transform _transformPoliceOfficer;
    private Transform _centerPoint;
    private bool _isTargetToPoint;
    private int _officerId;
    private bool _isDead;
    private bool _isFallingToDie;

    public int OfficerId => _officerId;
    public bool IsDead => _isDead;

    public event Action<PoliceOfficer> OnPoliceDeath;
    public event Action<PoliceOfficer> OnDeathAnimationFinished;
    
    private void Awake()
    {
        _transformPoliceOfficer = transform;
        _bulletPool = GetComponent<BulletPool>();
        _weapon = GetComponentInChildren<Weapon>();
    }

    private void Update()
    {
        
        if (_isTargetToPoint)
        {
            if (_transformPoliceOfficer.position.x > _borderX || _transformPoliceOfficer.position.x < -_borderX)
            {
                _borderPosition = new Vector3(_borderPosition.x, _transformPoliceOfficer.localPosition.y,
                    _transformPoliceOfficer.localPosition.z);
                
                _transformPoliceOfficer.localPosition = Vector3.MoveTowards(_transformPoliceOfficer.localPosition, _borderPosition, _speed * Time.deltaTime);
            }
            else
            {
                _transformPoliceOfficer.localPosition = Vector3.MoveTowards(_transformPoliceOfficer.localPosition, _targetPositionInGroup, _speed * Time.deltaTime);
            }
        }
    }

    public void TakeDamage()
    {
        _isDead = true;
        OnPoliceDeath?.Invoke(this);
        _isTargetToPoint = false;

        if (_coroutineRepeatShooting != null)
        {
            StopCoroutine(_coroutineRepeatShooting);
        }
        
        if (_isFallingToDie)
        {
            StartCoroutine(TurnModelToCenter());
        }
        else
        {
            StartCoroutine(PlayDeathAnimationAndReturn());   
        }
    }

    public void SetTargetPositionInGroup(Vector3 position)
    {
        _isTargetToPoint = true;
        
        _targetPositionInGroup = position;
    }

    public void SetNumberOfficer(int number)
    {
        _officerId = number;
    }

    public void SetCenterPoint(Transform point)
    {
        _centerPoint = point;
        _isFallingToDie = true;
    }

    public void Shooting(float repeat)
    {
        _repeatShoot = repeat;
        
        _coroutineRepeatShooting = StartCoroutine(RepeatShooting());
    }
    
    public void BoostFireRate(float repeat)
    {
        _repeatShoot = repeat;
    }
    
    private IEnumerator TurnModelToCenter()
    {
        Vector3 direction = _centerPoint.position - _basePoint.position;
        Vector3 tiltAxis = Vector3.Cross(direction, Vector3.up);
        Quaternion targetRotation = _basePoint.rotation * Quaternion.AngleAxis(_tiltAngle, tiltAxis);
        
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            float step = elapsed / _duration;
            _basePoint.rotation = Quaternion.Slerp(_basePoint.rotation, targetRotation, step);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        _isFallingToDie = false;
        
        StartCoroutine(PlayDeathAnimationAndReturn());
    }

    private IEnumerator PlayDeathAnimationAndReturn()
    {
        yield return new WaitForSeconds(_delay);
        
        _isDead = false;
        OnDeathAnimationFinished?.Invoke(this);
    }

    private IEnumerator RepeatShooting()
    {
        while (enabled)
        {
            _bulletPool.GetBullet(_weapon.BulletSpawnPosition);
            
            yield return new WaitForSeconds(_repeatShoot);
        }
    }
}
