using Unity.Mathematics;
using UnityEngine;

public class PoleceOfficerSpawner : ObjectPool<PoliceOfficer>
{
    private Transform _transformSpawn;
    
    public PoliceOfficer CreatePoliceUnits()
    {
        PoliceOfficer policeOfficer = GetObject();
        policeOfficer.OnDeathAnimationFinished += ReturnPoliceOfficer;
        return policeOfficer;
    }

    private void ReturnPoliceOfficer(PoliceOfficer policeOfficer)
    {
        policeOfficer.OnDeathAnimationFinished -= ReturnPoliceOfficer;
        ReturnObject(policeOfficer);
    }
}