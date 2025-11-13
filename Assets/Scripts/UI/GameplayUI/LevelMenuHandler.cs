using System;
using System.Collections;
using EachOneMatters.Audio;
using EachOneMatters.Shops;
using EachOneMatters.UI.MainMenuUI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

namespace EachOneMatters.UI.GameplayUI
{
    public class LevelMenuHandler : MonoBehaviour
    {
        private const string LevelScene = "LevelScene";
        private const string RewardID = "newPolice";

        [SerializeField] private Image _displayAlert;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private TextMeshProUGUI _pauseText;
        [SerializeField] private TextMeshProUGUI _winText;
        [SerializeField] private TextMeshProUGUI _callHelpPoliceOfficerText;
        [SerializeField] private TextMeshProUGUI _callHelpPauseText;
        [SerializeField] private TextMeshProUGUI _callHelpPoliceOfficerPrice;
        [SerializeField] private TextMeshProUGUI _warningMaxPoliceReached;
        [SerializeField] private Button _reviveWithAd;
        [SerializeField] private Button _continueGame;
        [SerializeField] private Button _nextLevel;
        [SerializeField] private Button _callHelpPoliceOfficer;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private GameMenu _gameMenu;
        [SerializeField] private Upgrade _upgradeMenu;
        [SerializeField] private UIElementToggler _elementToggler;
        [SerializeField] private float _delay = 1;
        [SerializeField] private float _delayAlert = 1.5f;
        [SerializeField] private float _durationWarningMessage = 3f;
        [SerializeField] private int _cooldown = 5;

        private bool _isEnableGameMenu;
        private bool _isEnableUpgradeMenu;
        private Coroutine _cooldownCoroutine;
        private Coroutine _displayAlertCoroutine;
        private Coroutine _warningMessageCoroutine;
        private LevelSounds _levelSounds;

        public event Action OnLevelUp;
        public event Action OnCallHelpPoliceOfficer;
        public event Action<bool> OnRewardedAdWatched;
        public event Action OnRewardedAdClicked;

        private void OnEnable()
        {
            YG2.onRewardAdv += OnReward;
        }

        private void OnDisable()
        {
            YG2.onRewardAdv -= OnReward;
        }

        private void Awake()
        {
            _levelSounds = GetComponent<LevelSounds>();
        }

        public void ShowMaxPoliceWarning()
        {
            if (_warningMessageCoroutine != null)
            {
                StopCoroutine(_warningMessageCoroutine);
            }

            _warningMessageCoroutine = StartCoroutine(ShowTemporaryWarning());
        }

        public void InitializeLevelMenu()
        {
            _levelSounds.PlayBackgroundMusic();
            _elementToggler.gameObject.SetActive(true);

            if (YG2.envir.isMobile)
            {
                _menuButton.gameObject.SetActive(true);
            }

            YG2.saves.gameplay.IsLoadedMainMenu = true;
            YG2.SaveProgress();
        }

        public void ShowPauseGameMenu()
        {
            if (_isEnableGameMenu == false && _isEnableUpgradeMenu == false)
            {
                _elementToggler.gameObject.SetActive(false);
                _isEnableGameMenu = true;
                _gameMenu.gameObject.SetActive(true);
                _pauseText.gameObject.SetActive(true);
                _continueGame.gameObject.SetActive(true);
                _levelSounds.PlayPauseMusic();
                Time.timeScale = 0f;
            }
        }

        public void ResumeGame()
        {
            SetCursorState(true, false);
            _levelSounds.PlayBackgroundMusic();
            _elementToggler.gameObject.SetActive(true);
            _isEnableGameMenu = false;
            _gameMenu.gameObject.SetActive(false);
            _pauseText.gameObject.SetActive(false);
            _continueGame.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        public void ReloadLevel()
        {
            _isEnableGameMenu = false;
            YG2.saves.gameplay.IsLoadedMainMenu = false;
            YG2.SaveProgress();
            YG2.InterstitialAdvShow();
            Time.timeScale = 1f;
            SceneManager.LoadScene(LevelScene);
        }

        public void LoadMainMenu()
        {
            YG2.saves.gameplay.IsLoadedMainMenu = true;
            YG2.SaveProgress();
            _isEnableGameMenu = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(LevelScene);
        }

        public void LoadNextLevel()
        {
            OnLevelUp?.Invoke();
            _isEnableGameMenu = false;
            _gameMenu.gameObject.SetActive(false);
            _winText.gameObject.SetActive(false);
            _nextLevel.gameObject.SetActive(false);
            _upgradeButton.gameObject.SetActive(false);
            YG2.saves.gameplay.IsLoadedMainMenu = false;
            YG2.SaveProgress();
            YG2.InterstitialAdvShow();
            SceneManager.LoadScene(LevelScene);
        }

        public void ShowGameOverMenu(bool isPoliceOnBase)
        {
            SetCursorState(false, true);
            _elementToggler.gameObject.SetActive(false);
            _isEnableGameMenu = true;
            _gameMenu.gameObject.SetActive(true);
            _gameOverText.gameObject.SetActive(true);

            if (isPoliceOnBase == false)
            {
                _reviveWithAd.gameObject.SetActive(true);
            }

            _levelSounds.PlayGameOverMusic();
        }

        public void ShowWinGameMenu()
        {
            SetCursorState(false, true);
            _elementToggler.gameObject.SetActive(false);
            _isEnableGameMenu = true;
            _gameMenu.gameObject.SetActive(true);
            _winText.gameObject.SetActive(true);
            _nextLevel.gameObject.SetActive(true);
            _upgradeButton.gameObject.SetActive(true);
            _levelSounds.PlayWinMusic();
        }

        public void ShowUpgradeMenu()
        {
            _isEnableGameMenu = false;
            _isEnableUpgradeMenu = true;
            _gameMenu.gameObject.SetActive(false);
            _upgradeMenu.gameObject.SetActive(true);
        }

        public void CloseUpgradeMenu()
        {
            _isEnableGameMenu = true;
            _isEnableUpgradeMenu = false;
            _gameMenu.gameObject.SetActive(true);
            _upgradeMenu.gameObject.SetActive(false);
        }

        public void ActivateCallHelp()
        {
            _callHelpPoliceOfficer.gameObject.SetActive(true);
            _callHelpPoliceOfficerPrice.text = YG2.saves.gameplay.CallHelpButtonPrice.ToString();
        }

        public void CallHelpPoliceOfficer()
        {
            OnCallHelpPoliceOfficer?.Invoke();
        }

        public void LockButtonDuringCooldown()
        {
            if (_displayAlertCoroutine != null)
            {
                StopCoroutine(_displayAlertCoroutine);
            }

            if (_cooldownCoroutine == null)
            {
                _cooldownCoroutine = StartCoroutine(CooldownLockRoutine());
            }
        }

        public void ShowNotEnoughPointsWindow()
        {
            if (_displayAlertCoroutine != null)
            {
                StopCoroutine(_displayAlertCoroutine);
            }

            _displayAlertCoroutine = StartCoroutine(DisplayTemporaryAlert());
        }

        public void ShowReviveWithAd()
        {
            YG2.RewardedAdvShow(RewardID);
            _reviveWithAd.gameObject.SetActive(false);
            _levelSounds.PlayBackgroundMusic();
            OnRewardedAdClicked?.Invoke();
        }

        private void OnReward(string id)
        {
            bool isRewarded = id == RewardID;

            _isEnableGameMenu = false;
            _gameMenu.gameObject.SetActive(false);
            _gameOverText.gameObject.SetActive(false);

            StartCoroutine(RestoreGameStateAfterAd(isRewarded));
        }

        private IEnumerator ShowTemporaryWarning()
        {
            WaitForSeconds delay = new WaitForSeconds(_durationWarningMessage);
            _warningMaxPoliceReached.gameObject.SetActive(true);

            yield return delay;

            _warningMaxPoliceReached.gameObject.SetActive(false);
        }

        private IEnumerator RestoreGameStateAfterAd(bool isRewarded)
        {
            yield return null;

            if (isRewarded)
            {
                _elementToggler.gameObject.SetActive(true);
                SetCursorState(true, false);

                OnRewardedAdWatched?.Invoke(true);
            }
            else
            {
                SetCursorState(false, true);

                OnRewardedAdWatched?.Invoke(false);
            }
        }

        private IEnumerator DisplayTemporaryAlert()
        {
            WaitForSeconds delay = new WaitForSeconds(_delayAlert);
            _displayAlert.gameObject.SetActive(true);

            yield return delay;

            _displayAlert.gameObject.SetActive(false);
            _displayAlertCoroutine = null;
        }

        private IEnumerator CooldownLockRoutine()
        {
            var wait = new WaitForSecondsRealtime(_delay);
            int cooldownSeconds = _cooldown;
            _callHelpPoliceOfficer.interactable = false;
            _callHelpPoliceOfficerText.gameObject.SetActive(false);
            _callHelpPauseText.gameObject.SetActive(true);

            while (cooldownSeconds > 0)
            {
                _callHelpPoliceOfficerPrice.text = cooldownSeconds.ToString();
                cooldownSeconds--;

                yield return wait;
            }

            _callHelpPoliceOfficerPrice.text = YG2.saves.gameplay.CallHelpButtonPrice.ToString();
            _callHelpPoliceOfficerText.gameObject.SetActive(true);
            _callHelpPauseText.gameObject.SetActive(false);
            _callHelpPoliceOfficer.interactable = true;
            _cooldownCoroutine = null;
        }

        private void SetCursorState(bool isLocked, bool isVisible)
        {
            if (YG2.envir.isDesktop)
            {
                Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
                Cursor.visible = isVisible;
            }
        }
    }
}