using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class MainMenuController : MonoBehaviour
{
    private const string LevelScene = "LevelScene";
    
    [SerializeField] private BaseMenu _desctopBaseMenu;
    [SerializeField] private BaseMenu _mobileBaseMenu;
    [SerializeField] private CharacterCreation _desctopCharacterCreation;
    [SerializeField] private CharacterCreation _mobileCharacterCreation;
    [SerializeField] private LeaderboardUI _desctopLeaderboardUI;
    [SerializeField] private LeaderboardUI _mobileLeaderboardUI;
    [SerializeField] private SettingsUI _desctopSettingsUI;
    [SerializeField] private SettingsUI _mobileSettingsUI;
    [SerializeField] private Button _desctopContinueButton;
    [SerializeField] private Button _mobileContinueButton;
    [SerializeField] private TextMeshProUGUI _desctopWelcomeText;
    [SerializeField] private TextMeshProUGUI _mobileWelcomeText;
    [SerializeField] private TextMeshProUGUI _desctopContinueText;
    [SerializeField] private TextMeshProUGUI _mobileContinueText;
    [SerializeField] private TextMeshProUGUI _nameGameText;
    [SerializeField] private Image _desctopStartMenuPanel;
    [SerializeField] private Image _mobileStartMenuPanel;
    [SerializeField] private Image _desctopContinueMenuPanel;
    [SerializeField] private Image _mobileContinueMenuPanel;
    
    [Header("Demo & Audio")]
    [SerializeField] private Game _demoGame; 
    [SerializeField] private GameObject _tapToStartPrompt; 

    private BaseMenu _baseMenu;
    private CharacterCreation _characterCreation;
    private LeaderboardUI _leaderboardUI;
    private SettingsUI _settingsUI;
    private Button _continueButton;
    private BaseMenuSound _menuSound;
    private bool _hasSavedPlayer;
    private TextMeshProUGUI _welcomeText;
    private TextMeshProUGUI _continueText;
    private Image _startMenuPanel;
    private Image _continueMenuPanel;

    private void Awake()
    {
        _menuSound = GetComponent<BaseMenuSound>();
    }

    private void Start()
    {
        if (YG2.saves.IsLoadedMainMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            _tapToStartPrompt?.SetActive(true);
            StartCoroutine(WaitForFirstInput());
        }
    }
    
    public void StartNewGame()
    {
        _baseMenu.gameObject.SetActive(false);

        if (_hasSavedPlayer)
        {
            _characterCreation.gameObject.SetActive(true);
            _characterCreation.ShowWarningAndConfirm();
        }
        else
        {
            _characterCreation.TryCreatePlayer();
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
        CreateMenuToDevise();
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
        _settingsUI.CreateLanguageToSetting(YG2.lang);
        _welcomeText.gameObject.SetActive(!_hasSavedPlayer);
        _continueText.gameObject.SetActive(_hasSavedPlayer);
        _startMenuPanel.gameObject.SetActive(!_hasSavedPlayer);
        _continueMenuPanel.gameObject.SetActive(_hasSavedPlayer);
        _continueButton.gameObject.SetActive(_hasSavedPlayer);
    }

    private void CreateMenuToDevise()
    {
        if (YG2.envir.isMobile)
        {
            _baseMenu = _mobileBaseMenu;
            _characterCreation = _mobileCharacterCreation;
            _continueButton = _mobileContinueButton;
            _leaderboardUI = _mobileLeaderboardUI;
            _settingsUI = _mobileSettingsUI;
            _welcomeText = _mobileWelcomeText;
            _continueText = _mobileContinueText;
            _startMenuPanel = _mobileStartMenuPanel;
            _continueMenuPanel = _mobileContinueMenuPanel;
        }
        else
        {
            _baseMenu = _desctopBaseMenu;
            _characterCreation = _desctopCharacterCreation;
            _continueButton = _desctopContinueButton;
            _leaderboardUI = _desctopLeaderboardUI;
            _settingsUI = _desctopSettingsUI;
            _welcomeText = _desctopWelcomeText;
            _continueText = _desctopContinueText;
            _startMenuPanel = _desctopStartMenuPanel;
            _continueMenuPanel = _desctopContinueMenuPanel;
        }
    }
}