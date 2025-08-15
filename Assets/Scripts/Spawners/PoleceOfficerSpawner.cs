using Unity.Mathematics;
using UnityEngine;

public class PoleceOfficerSpawner : ObjectPool<PoliceOfficer>
{
    private Transform _transformSpawn;
    
    public PoliceOfficer CreatePoliceUnits(Transform transformSpawn, BulletPool bulletPool)
    {
        _transformSpawn = transformSpawn;
        
        PoliceOfficer policeOfficer = GetObject();

        policeOfficer.transform.parent = _transformSpawn;
        policeOfficer.transform.position = _transformSpawn.position;
        policeOfficer.transform.rotation = _transformSpawn.rotation;
        policeOfficer.SetPoliceOfficerActive(bulletPool);
        policeOfficer.OnDeathAnimationFinished += ReturnPoliceOfficer;
        
        return policeOfficer;
    }

    private void ReturnPoliceOfficer(PoliceOfficer policeOfficer)
    {
        policeOfficer.OnDeathAnimationFinished -= ReturnPoliceOfficer;
        
        ReturnObject(policeOfficer);
    }
}