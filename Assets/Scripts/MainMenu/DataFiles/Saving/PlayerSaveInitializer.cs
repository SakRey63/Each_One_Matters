using YG;

namespace EachOneMatters.Saving
{
    public static class PlayerSaveInitializer
    {
        public static void SetNewPlayer()
        {
            var saves = YG2.saves;
            saves.player.HasSavedPlayer = true;
            saves.gameplay.IsCallHelpUpgradePurchased = false;
            saves.gameplay.Score = 0;
            saves.gameplay.Level = 1;
            saves.player.CountPoliceOfficer = 5;
            saves.upgrades.BuffDuration = 5f;
            saves.upgrades.IncreaseSquadPrices = 1000;
            saves.upgrades.ExtendFireRateDurationPrices = 1000;
            saves.upgrades.CallHelpOnBasePrices = 1000;
            saves.gameplay.CountHelpPoliceOfficer = 3;
            saves.gameplay.CallHelpButtonPrice = 150;
            saves.gameplay.IsLoadedMainMenu = false;
            
            YG2.SaveProgress();
        }
    }
}