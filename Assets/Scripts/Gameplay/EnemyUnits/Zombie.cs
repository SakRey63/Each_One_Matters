using System;
using EachOneMatters.Animation;
using EachOneMatters.Common;
using EachOneMatters.Gameplay.PlayerUnits;
using EachOneMatters.Generation.Bridge;
using UnityEngine;

namespace EachOneMatters.Gameplay.EnemyUnits
{
    public class Zombie : MonoBehaviour, IBridgeObject, IUnitState
    {
        [SerializeField] private ZombieVision _zombieVision;
        [SerializeField] private ZombieSensor _zombieSensor;

        private Health _health = new Health();
        private EnemyAnimation _enemyAnimation;
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
        private bool _isAttacked;
        private int _id;
        private ProgressionState _progressionState;

        public event Action<Zombie> OnZombieDeath;
        
        public UnitStatus Status { get; private set; }
        public int Id => _id;
        public BridgeObjectType Type => BridgeObjectType.Zombie;

        private void Awake()
        {
            _transform = transform;
            _enemyAnimation = GetComponent<EnemyAnimation>();
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
            _progressionState = ProgressionState.Idle;
            Status = UnitStatus.Alive;
            _enemyAnimation.ResumeAnimation();
            _zombieMovement.ResetSpeed();
            _isAttacked = false;
            _enterToBase = null;
            _policeOfficer = null;
            _zombieVision.ScanForEnemies();
        }

        public void SetNumberEnemy(int zombieId)
        {
            _id = zombieId;
        }

        public void TakeDamage(int damage, UnitStatus killedStatus)
        {
            _health.TakeDamage(damage);

            if (_health.HealthPoint <= 0)
            {
                Status = killedStatus;
                _zombieVision.ScanOff();
                OnZombieDeath?.Invoke(this);
            }
        }

        public void StopMoving()
        {
            _isMoving = false;
            _enemyAnimation.PauseAnimation();
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
                _progressionState = ProgressionState.Idle;
                _isAttacked = false;
                _zombieVision.ScanForEnemies();
            }
        }

        private void HandleTargetReached()
        {
            switch (_progressionState)
            {
                case ProgressionState.BaseEntryCompleted:
                    _progressionState = ProgressionState.ReachedBaseEntry;
                    _finish.position = new Vector3(_startPositionToScanEnemy.position.x, _transform.position.y,
                        _startPositionToScanEnemy.position.z);
                    _target = _finish;
                    break;
                case ProgressionState.ReachedBaseEntry:
                    _zombieVision.ScanForEnemies();
                    break;
                case ProgressionState.Idle:
                    _target = _finish;
                    break;
            }
        }

        private void ToggleVisionOnBaseDetected(Base policeBase)
        {
            if (_progressionState == ProgressionState.BaseEntryCompleted)
                return;

            if (_policeOfficer != null)
            {
                _policeOfficer.OnPoliceDeath -= LookingNewTarget;
                _policeOfficer = null;
            }

            _progressionState = ProgressionState.BaseEntryCompleted;
            _zombieMovement.ResetSpeed();
            _zombieVision.ScanOff();
            _enterToBase = policeBase.BaseEntryTransform;
            _startPositionToScanEnemy = policeBase.StartPositionGeneration;
            _finish.position = new Vector3(_enterToBase.position.x, _transform.position.y, _enterToBase.position.z);
            _target = _finish;
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
            _progressionState = ProgressionState.MovingToEnemy;
            _zombieMovement.Accelerate();
        }

        private void LookingNewTarget(PoliceOfficer police)
        {
            police.OnPoliceDeath -= LookingNewTarget;
            _policeOfficer = null;

            if (gameObject.activeInHierarchy)
            {
                _progressionState = ProgressionState.Idle;
                _target = _finish;
                _zombieMovement.ResetSpeed();
                _zombieVision.ScanForEnemies();
            }
        }
    }
}