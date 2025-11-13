using System;

namespace YG
{
    [Serializable]
    public class PlayerData
    {
        public int MaxPoliceCount = 44;
        public int CountPoliceOfficer = 5;
        public bool HasSavedPlayer = false;
    }

    public partial class SavesYG
    {
        public PlayerData player = new PlayerData();
    }
}