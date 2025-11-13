using EachOneMatters.Gameplay.PlayerUnits;
using EachOneMatters.Pool;

namespace EachOneMatters.Gameplay.Player.Class
{
    public class PoleceOfficerSpawner : ObjectPool<PoliceOfficer>
    {
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
}