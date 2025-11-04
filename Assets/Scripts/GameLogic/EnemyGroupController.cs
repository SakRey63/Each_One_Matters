using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupController : MonoBehaviour
{
    [SerializeField] private int _healthPoint = 75;

    private int _indexEnemy;
    private Dictionary<int, Zombie> _activeZombies;
    private int _countAllZombies;
    private bool _isMovingStop;
    
    public int CountAllZombies => _countAllZombies;
    
    public event Action<UnitStatus> OnZombieKilled;

    private void Awake()
    {
        _activeZombies = new Dictionary<int, Zombie>();
    }

    public void AddEnemy(Zombie zombie)
    {
        _countAllZombies++;
        
        zombie.OnZombieDeath += RemoveZombies;
        zombie.SetNumberEnemy(_indexEnemy);
        zombie.SetHealthPoint(_healthPoint);
        zombie.SetSettingsToActivateUnit();
        _activeZombies.Add(zombie.Id, zombie);
        
        if (_isMovingStop)
        {
            zombie.StopMoving();
        }
        
        _indexEnemy++;
    }

    public void StopAllZombies()
    {
        _isMovingStop = true;
        
        foreach (Zombie zombie in _activeZombies.Values)
        {
            if (zombie != null && zombie.gameObject.activeInHierarchy)
            { 
                zombie.StopMoving();  
            }
        }
    }

    public void ResumeAllZombies()
    {
        _isMovingStop = false;
        
        foreach (Zombie zombie in _activeZombies.Values)
        {
            if (zombie != null && zombie.gameObject.activeInHierarchy)
            {
                zombie.SetSettingsToActivateUnit();
            }
        }
    }

    private void RemoveZombies(Zombie zombie)
    {
        _countAllZombies--;
        
        zombie.OnZombieDeath -= RemoveZombies;
        OnZombieKilled?.Invoke(zombie.Status);

        _activeZombies.Remove(zombie.Id);
    }
}
