using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenuHandler : MonoBehaviour
{
    private const string MainMenu = "MainMenuScene";
    private const string LevelScene = "LevelScene";
        
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _pauseText;
    [SerializeField] private TextMeshProUGUI _winText;
    [SerializeField] private Button _continueGame;
    [SerializeField] private Button _nextLevel;
    [SerializeField] private Button _callHelpPoliceOfficer;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private GameMenu _gameMenu;
    [SerializeField] private Upgrade _upgradeMenu;

    private bool _isEnableGameMenu;
    private bool _isEnableUpgradeMenu;

    public event Action OnLevelUp; 
    
    public void ShowPauseGameMenu()
    {
        if (_isEnableGameMenu == false && _isEnableUpgradeMenu == false)
        {
            _isEnableGameMenu = true;
            _gameMenu.gameObject.SetActive(true);
            Time.timeScale = 0f;
            _pauseText.gameObject.SetActive(true);
            _continueGame.gameObject.SetActive(true);
        }
    }

    public void ResumeGame()
    {
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
        Time.timeScale = 1f;
    }

    public void ShowGameOverMenu()
    {
        _isEnableGameMenu = true;
        _gameMenu.gameObject.SetActive(true);
        Time.timeScale = 0f;
        _gameOverText.gameObject.SetActive(true);
    }

    public void ShowWinGameMenu()
    {
        _isEnableGameMenu = true;
        _gameMenu.gameObject.SetActive(true);
        _winText.gameObject.SetActive(true);
        _nextLevel.gameObject.SetActive(true);
        _upgradeButton.gameObject.SetActive(true);
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
    }
}
