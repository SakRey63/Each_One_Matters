using EachOneMatters.Inputs;
using EachOneMatters.UI.GameplayUI;
using UnityEngine;

namespace EachOneMatters.Systems
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private GameSessionSystem _gameSessionSystem;
        [SerializeField] private ProgressSystem _progressSystem;
        [SerializeField] private PlayerInputCoordinator _playerInputCoordinator;
        [SerializeField] private LevelMenuHandler _levelMenuHandler;
        [SerializeField] private DemoModeController _demoMode;

        private int _levelPlayer;
    
        private int _currentScore;
        private Coroutine _buffCoroutine;

        private void OnEnable()
        {
            _playerInputCoordinator.OnLevelMenuClicked += ShowLevelMenu;
            _levelMenuHandler.OnRewardedAdWatched += SetupRevivalWithAd;
        }

        private void OnDisable()
        {
            _playerInputCoordinator.OnLevelMenuClicked -= ShowLevelMenu;
            _levelMenuHandler.OnRewardedAdWatched -= SetupRevivalWithAd;
        }

        public void StartNewLevel()
        {
            _gameSessionSystem.StartNewLevel();
        }

        public void StartDemoScene()
        {
            _demoMode.SetupDemoScene();
        }
    
        private void ShowLevelMenu()
        {
            _levelMenuHandler.ShowPauseGameMenu();
        }
    
        private void SetupRevivalWithAd(bool isRewarded)
        {
            if (isRewarded)
            {
                _gameSessionSystem.ResumeAfterAd();
            }
            else
            {
                _levelMenuHandler.ShowGameOverMenu(true);
            }
        }
    }
}