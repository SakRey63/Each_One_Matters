using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class CharacterCreation : MonoBehaviour
{
    private const string LevelScene = "LevelScene";

    [SerializeField] private Button _statrtGameButton;
    [SerializeField] private Button _okButton;
    [SerializeField] private Image _hintImage;
    [SerializeField] private TextMeshProUGUI _enterNamePrompt;
    [SerializeField] private TextMeshProUGUI _nameOccupiedMessage;
    [SerializeField] private TextMeshProUGUI _newCharacterText;
    [SerializeField] private TextMeshProUGUI _nameChangeWarning;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private int _maxLimit = 10;

    public void ActivatedInputField()
    {
        _newCharacterText.gameObject.SetActive(true);
        _inputField.gameObject.SetActive(true);
        _inputField.Select();
        _inputField.ActivateInputField();
        _inputField.characterLimit = _maxLimit;
        _inputField.textComponent.color = Color.black;
        _inputField.text = "";
    }

    public void ResetNameText()
    {
        _statrtGameButton.gameObject.SetActive(true);
        _okButton.gameObject.SetActive(false);
        _hintImage.gameObject.SetActive(false);
        _enterNamePrompt.gameObject.SetActive(false);
        _nameOccupiedMessage.gameObject.SetActive(false);
        _nameChangeWarning.gameObject.SetActive(false);
    }

    public void TryCreatePlayer()
    {
        string playerName = _inputField.text; 
           
           if (string.IsNullOrEmpty(playerName))
           {
               _statrtGameButton.gameObject.SetActive(false);
               _okButton.gameObject.SetActive(true);
               _hintImage.gameObject.SetActive(true);
               _enterNamePrompt.gameObject.SetActive(true);
               return;
           }

           if (YG2.saves.PlayerName == playerName)
           {
               _inputField.textComponent.color = Color.red;
               _statrtGameButton.gameObject.SetActive(false);
               _okButton.gameObject.SetActive(true);
               _hintImage.gameObject.SetActive(true);
               _nameOccupiedMessage.gameObject.SetActive(true);
           }
           else
           {
               YG2.saves.SetNewPlayer(playerName);
               YG2.saves.IsLoadedMainMenu = false;
               YG2.SaveProgress();
               SceneManager.LoadScene(LevelScene);
           }
    }

    public void ShowWarningAndConfirm()
    {
        _newCharacterText.gameObject.SetActive(false);
        _nameChangeWarning.gameObject.SetActive(true);
        _statrtGameButton.gameObject.SetActive(false);
        _inputField.gameObject.SetActive(false);
        _okButton.gameObject.SetActive(true);
    }
}
