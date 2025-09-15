using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using YG;

public class GameMenuHandler : MonoBehaviour
{
    private const string MainMenu = "MainMenuScene";
    private const string LevelScene = "LevelScene";

    [SerializeField] private Image _displayAlert;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _pauseText;
    [SerializeField] private TextMeshProUGUI _winText;
    [SerializeField] private TextMeshProUGUI _callHelpPoliceOfficerText;
    [SerializeField] private TextMeshProUGUI _callHelpPauseText;
    [SerializeField] private TextMeshProUGUI _callHelpPoliceOfficerPrice;
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
    [SerializeField] private int _cooldown = 5;
    [SerializeField] private string _rewardID = "newPolice";

    private bool _isEnableGameMenu;
    private bool _isEnableUpgradeMenu;
    private Coroutine _cooldownCoroutine;
    private Coroutine _displayAlertCoroutine;
    private LevelSounds _levelSounds;

    public event Action OnLevelUp;
    public event Action OnCallHelpPoliceOfficer; 
    public event Action OnRewardedAdWatched;
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

    private void Start()
    {
        _levelSounds.CreateBackgroundMusic();
        _elementToggler.gameObject.SetActive(true);
        
        if (YG2.envir.isMobile)
        {
            _menuButton.gameObject.SetActive(true);
        }
    }

    public void ShowPauseGameMenu()
    {
        if (_isEnableGameMenu == false && _isEnableUpgradeMenu == false)
        {
            _elementToggler.gameObject.SetActive(false);
            _isEnableGameMenu = true;
            _gameMenu.gameObject.SetActive(true);
            Time.timeScale = 0f;
            _pauseText.gameObject.SetActive(true);
            _continueGame.gameObject.SetActive(true);
            _levelSounds.CreateMusicMenuPause();
        }
    }

    public void ResumeGame()
    {
        _levelSounds.CreateBackgroundMusic();
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
        SceneManager.LoadScene(LevelScene);
        YG2.InterstitialAdvShow();
        Time.timeScale = 1f;
    }

    public void LoadMainMenu()
    {
        _isEnableGameMenu = false;
        SceneManager.LoadScene(MainMenu); 
        Time.timeScale = 1f;
    }

    public void LoadNextLevel()
    {
        OnLevelUp?.Invoke();
        _isEnableGameMenu = false;
        _gameMenu.gameObject.SetActive(false);
        _winText.gameObject.SetActive(false);
        _nextLevel.gameObject.SetActive(false);
        _upgradeButton.gameObject.SetActive(false);
        SceneManager.LoadScene(LevelScene);
        YG2.InterstitialAdvShow();
        Time.timeScale = 1f;
    }

    public void ShowGameOverMenu(bool isPoliceOnBase)
    {
        _elementToggler.gameObject.SetActive(false);
        _isEnableGameMenu = true;
        _gameMenu.gameObject.SetActive(true);
        Time.timeScale = 0f;
        _gameOverText.gameObject.SetActive(true);
        
        if (isPoliceOnBase == false)
        {
            _reviveWithAd.gameObject.SetActive(true);
        }

        _levelSounds.CreateGameOverMusic();
    }

    public void ShowWinGameMenu()
    {
        _elementToggler.gameObject.SetActive(false);
        _isEnableGameMenu = true;
        _gameMenu.gameObject.SetActive(true);
        _winText.gameObject.SetActive(true);
        _nextLevel.gameObject.SetActive(true);
        _upgradeButton.gameObject.SetActive(true);
        _levelSounds.CreateWinMusic();
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
        _callHelpPoliceOfficerPrice.text = YG2.saves.CallHelpButtonPrice.ToString();
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
        Time.timeScale = 1f;
        YG2.RewardedAdvShow(_rewardID);
        _reviveWithAd.gameObject.SetActive(false);
        _levelSounds.CreateBackgroundMusic();
        OnRewardedAdClicked?.Invoke();
    }
    
    private void OnReward(string id)
    {
        if (id == _rewardID)
        {
            _elementToggler.gameObject.SetActive(true);
            _isEnableGameMenu = false;
            _gameMenu.gameObject.SetActive(false);
            _gameOverText.gameObject.SetActive(false);
            OnRewardedAdWatched?.Invoke();
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

        _callHelpPoliceOfficerPrice.text = YG2.saves.CallHelpButtonPrice.ToString();
        _callHelpPoliceOfficerText.gameObject.SetActive(true);
        _callHelpPauseText.gameObject.SetActive(false);
        _callHelpPoliceOfficer.interactable = true;
        _cooldownCoroutine = null;
    }
}
