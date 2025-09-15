using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class GameManager : MonoBehaviour
{
    private const string LevelScene = "LevelScene";
    
    [SerializeField] private BaseMenu _baseMenu;
    [SerializeField] private CharacterCreation _characterCreation;
    [SerializeField] private LeaderboardUI _leaderboardUI;
    [SerializeField] private SettingsUI _settingsUI;
    [SerializeField] private Button _continueButton;
    [SerializeField] private TextMeshProUGUI _welcomeText;
    [SerializeField] private TextMeshProUGUI _continueText;

    private BaseMenuSound _menuSound;
    private bool _hasSavedPlayer;

    private void Awake()
    {
        _menuSound = GetComponent<BaseMenuSound>();
    }

    private void Start()
    {
        UpdateGreetingText();
        _menuSound.CreateBaseMusic();
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
    
    public void ExitGame()
    {
        // В редакторе работает нормально, на сборке — закрывает игру
        Application.Quit();

        // Для редактора добавьте:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    private void UpdateGreetingText()
    {
        _hasSavedPlayer = YG2.saves.HasSavedPlayer;
        _welcomeText.gameObject.SetActive(!_hasSavedPlayer);
        _continueText.gameObject.SetActive(_hasSavedPlayer);

        if (_hasSavedPlayer)
        {
            _settingsUI.OnLanguageChanged(YG2.saves.NumberLanguage);
            string text = _continueText.text;
            text += ", " + YG2.saves.PlayerName;
            _continueText.text = text;
        }
        
        _continueButton.gameObject.SetActive(_hasSavedPlayer);
    }
}