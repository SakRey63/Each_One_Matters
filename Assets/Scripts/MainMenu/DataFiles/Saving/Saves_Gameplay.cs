using System;

namespace YG
{
    [Serializable]
    public class GameplayData
    {
        public int Level = 1;
        public int Score = 0;
        public int CallHelpButtonPrice = 150;
        public int CountHelpPoliceOfficer = 3;
        public bool IsCallHelpUpgradePurchased = false;
        public bool IsFirstLaunch = true;
        public bool IsLoadedMainMenu = true;
    }

    public partial class SavesYG
    {
        public GameplayData gameplay = new GameplayData();
    }
}