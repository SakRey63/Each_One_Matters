using UnityEngine;
using YG;

namespace EachOneMatters.Systems
{
    public class ProgressSystem : MonoBehaviour
    {
        private const string LeaderboardId = "EachOfMatersLB";
    
        public void SetCurrentLevel(int level)
        {
            YG2.saves.gameplay.Level = level;
            SaveProgress();
        }

        public int GetCurrentLevel()
        {
            return YG2.saves.gameplay.Level;
        }

        public void AddScore(int scoreToAdd)
        {
            int currentTotalScore = YG2.saves.gameplay.Score;
            currentTotalScore += scoreToAdd;
            YG2.saves.gameplay.Score = currentTotalScore;
            YG2.SetLeaderboard(LeaderboardId, currentTotalScore);
            SaveProgress();
        }

        public void SetPlayerCount(int count)
        {
            YG2.saves.player.CountPoliceOfficer = count;
            SaveProgress();
        }

        public int GetCurrentScore()
        {
            return YG2.saves.gameplay.Score;
        }

        public int GetPlayerCount()
        {
            return YG2.saves.player.CountPoliceOfficer;
        }

        public int GetMaxPlayerCount()
        {
            return YG2.saves.player.MaxPoliceCount;
        }
    
        public int GetCallHelpPrice()
        {
            return YG2.saves.gameplay.CallHelpButtonPrice;
        }

        private void SaveProgress()
        {
            YG2.SaveProgress();
        }
    }
}