using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform _bulletSpawnPosition;

    public Transform BulletSpawnPosition => _bulletSpawnPosition;
}
