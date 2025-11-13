using YG;

namespace EachOneMatters.Data
{
    public static class PlayerSaveInitializer
    {
        private const int DefaultLevel = 1;
        private const int DefaultScore = 0;
        private const int DefaultPoliceCount = 5;
        private const int DefaultHelpPoliceCount = 3;
        private const int DefaultCallHelpPrice = 150;
        private const float DefaultBuffDuration = 5f;
        private const int DefaultUpgradePrice = 1000;
        
        public static void SetNewPlayer()
        {
            var saves = YG2.saves;
            saves.player.HasSavedPlayer = true;
            saves.gameplay.IsCallHelpUpgradePurchased = false;
            saves.gameplay.Score = DefaultScore;
            saves.gameplay.Level = DefaultLevel;
            saves.player.CountPoliceOfficer = DefaultPoliceCount;
            saves.upgrades.BuffDuration = DefaultBuffDuration;
            saves.upgrades.IncreaseSquadPrices = DefaultUpgradePrice;
            saves.upgrades.ExtendFireRateDurationPrices = DefaultUpgradePrice;
            saves.upgrades.CallHelpOnBasePrices = DefaultUpgradePrice;
            saves.gameplay.CountHelpPoliceOfficer = DefaultHelpPoliceCount;
            saves.gameplay.CallHelpButtonPrice = DefaultCallHelpPrice;
            saves.gameplay.IsLoadedMainMenu = false;
            
            YG2.SaveProgress();
        }
    }
}