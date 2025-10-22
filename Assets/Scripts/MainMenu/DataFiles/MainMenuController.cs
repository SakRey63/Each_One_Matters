using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class MainMenuController : MonoBehaviour
{
    private const string LevelScene = "LevelScene";
    
    [SerializeField] private BaseMenu _baseMenu;
    [SerializeField] private CharacterCreation _characterCreation;
    [SerializeField] private LeaderboardUI _leaderboardUI;
    [SerializeField] private SettingsUI _settingsUI;
    [SerializeField] private Button _continueButton;
    [SerializeField] private TextMeshProUGUI _welcomeText;
    [SerializeField] private TextMeshProUGUI _continueText;
    [SerializeField] private TextMeshProUGUI _nameGameText;
    
    [Header("Demo & Audio")]
    [SerializeField] private Game _demoGame; 
    [SerializeField] private GameObject _tapToStartPrompt; 

    private BaseMenuSound _menuSound;
    private bool _hasSavedPlayer;

    private void Awake()
    {
        _menuSound = GetComponent<BaseMenuSound>();
    }

    private void Start()
    {
        if (YG2.saves.IsLoadedMainMenu)
        {
            _settingsUI.CreateLanguageToSetting(YG2.lang);
            _tapToStartPrompt?.SetActive(true);
            StartCoroutine(WaitForFirstInput());
        }
    }
    
    public void StartNewGame()
    {
        _baseMenu.gameObject.SetActive(false);
        _characterCreation.gameObject.SetActive(true);

        if (_hasSavedPlayer)
        {
            _characterCreation.ShowWarningAndConfirm();
        }
        else
        {
            _characterCreation.ActivatedInputField();
        }
    }

    public void ContinueGame()
    {
        YG2.saves.IsLoadedMainMenu = false;
        YG2.SaveProgress();
        SceneManager.LoadScene(LevelScene);
    }

    public void ReturnToMainMenu()
    {
        _baseMenu.gameObject.SetActive(true);
        _characterCreation.gameObject.SetActive(false);
        _leaderboardUI.gameObject.SetActive(false);
        _settingsUI.gameObject.SetActive(false);
        UpdateGreetingText();
    }

    public void ShowSettingsMenu()
    {
        _baseMenu.gameObject.SetActive(false);
        _settingsUI.gameObject.SetActive(true);
    }

    public void ShowLeaderboard()
    {
        _baseMenu.gameObject.SetActive(false);
        _leaderboardUI.gameObject.SetActive(true);
    }
    
    private IEnumerator WaitForFirstInput()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);

        _tapToStartPrompt?.SetActive(false);
        _baseMenu.gameObject.SetActive(true);
        _nameGameText.gameObject.SetActive(true);
        UpdateGreetingText();
        _menuSound.CreateMainMenuMusic();
        _demoGame.StartDemoScene();
    }
    
    private void UpdateGreetingText()
    {
        _hasSavedPlayer = YG2.saves.HasSavedPlayer;
        _welcomeText.gameObject.SetActive(!_hasSavedPlayer);
        _continueText.gameObject.SetActive(_hasSavedPlayer);

        if (_hasSavedPlayer)
        {
            string text = _continueText.text;
            text += ", " + YG2.saves.PlayerName;
            _continueText.text = text;
        }
        
        _continueButton.interactable = _hasSavedPlayer;
    }
}