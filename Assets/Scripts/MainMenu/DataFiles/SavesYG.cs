using UnityEngine.Serialization;

namespace YG
{
    public partial class SavesYG
    {
        private int _startScore = 0;
        private int _startLevel = 1;
        private int _startCountHelpPoliceOfficer = 3;
        private int _startCountPoliceOfficer = 5;
        private int _startPrice = 1000;
        private int _startPriceCallHelpButton = 150;
        private float _startBuffDuration = 5f;
        
        public string PlayerName;
        public int Level;
        public int Score;
        public int CountPoliceOfficer;
        public int IncreaseSquadPrices;
        public int ExtendFireRateDurationPrices;
        public int CallHelpOnBasePrices;
        public int CountHelpPoliceOfficer;
        public int CallHelpButtonPrice;
        public float BuffDuration;
        public bool HasSavedPlayer;
        public bool IsCallHelpUpgradePurchased;
        public bool IsLoadedMainMenu;
        public float VolumeMusic;
        public float VolumeUi;
        public float VolumeSFX;
        

        public void SetNewPlayer(string name)
        {
            PlayerName = name;
            HasSavedPlayer = true;
            IsCallHelpUpgradePurchased = false;
            Score = _startScore;
            Level = _startLevel;
            CountPoliceOfficer = _startCountPoliceOfficer;
            BuffDuration = _startBuffDuration;
            IncreaseSquadPrices = _startPrice;
            ExtendFireRateDurationPrices = _startPrice;
            CallHelpOnBasePrices = _startPrice;
            CountHelpPoliceOfficer = _startCountHelpPoliceOfficer;
            CallHelpButtonPrice = _startPriceCallHelpButton;
        }
    }
}