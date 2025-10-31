using System;

namespace YG
{
    [Serializable]
    public class UpgradeData
    {
        public int IncreaseSquadPrices = 1000;
        public int ExtendFireRateDurationPrices = 1000;
        public int CallHelpOnBasePrices = 1000;
        public float BuffDuration = 5f;
    }

    public partial class SavesYG
    {
        public UpgradeData upgrades = new UpgradeData();
    }
}