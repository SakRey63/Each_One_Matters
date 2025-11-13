using System;
using System.Collections;
using EachOneMatters.Gameplay.EnemyUnits;
using EachOneMatters.Gameplay.PlayerUnits;
using EachOneMatters.Generation.Spawners;
using EachOneMatters.Systems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EachOneMatters.Generation.Bridge
{
    public class PointSpawnTrigger : MonoBehaviour
    {
        [SerializeField] private Transform _zombieSpawnArea;
        [SerializeField] private int _zombieMultiplier = 3;
        [SerializeField] private int _minCounZombie = 10;
        [SerializeField] private float _spawnDelay = 0.1f;
        [SerializeField] private float _border = 5.5f;

        private SpawnerZombie _spawnerZombie;
        private EnemyGroupController _enemyGroup;
        private int _index;

        public event Action<PointSpawnTrigger> OnHordeSpawning;

        public int Index => _index;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PoliceOfficer>(out _))
            {
                OnHordeSpawning?.Invoke(this);
            }
        }

        public void SetDirectionAndIndex(int index)
        {
            _index = index;
        }

        public void AssignSpawnerAndGroup(SpawnerZombie spawnerZombie, EnemyGroupController enemyGroup)
        {
            _spawnerZombie = spawnerZombie;
            _enemyGroup = enemyGroup;
        }

        public void SpawnAdaptiveWave(int countOfficer, Transform targetPosition)
        {
            int countZombie = GetCalculateCountZombies(countOfficer);

            if (_spawnerZombie != null)
            {
                StartCoroutine(SpawnWithDelay(countZombie, targetPosition));
            }
        }

        private IEnumerator SpawnWithDelay(int count, Transform targetPosition)
        {
            for (int i = 0; i < count; i++)
            {
                Zombie zombie = _spawnerZombie.CreateEnemy();
                zombie.SetFinish(targetPosition);
                _enemyGroup.AddEnemy(zombie);
                zombie.transform.position = GetRandomPositionSpawn();
                zombie.transform.rotation = _zombieSpawnArea.rotation;

                yield return new WaitForSeconds(_spawnDelay);
            }
        }

        private Vector3 GetRandomPositionSpawn()
        {
            float xPosition = Random.Range(_zombieSpawnArea.localPosition.x - _border,
                _zombieSpawnArea.localPosition.x + _border);

            Vector3 position = _zombieSpawnArea.TransformPoint(xPosition, _zombieSpawnArea.localPosition.y, 0);

            return position;
        }

        private int GetCalculateCountZombies(int policeCount)
        {
            int count = policeCount * _zombieMultiplier;

            if (count < _minCounZombie)
            {
                count = _minCounZombie;
            }

            return count;
        }

        private void OnDrawGizmos()
        {
            BoxCollider collider = GetComponent<BoxCollider>();

            if (collider == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(collider.center, collider.size);
        }
    }
}