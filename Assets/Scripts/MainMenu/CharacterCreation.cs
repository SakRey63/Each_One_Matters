using EachOneMatters.Saving;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{
    private const string LevelScene = "LevelScene";

    [SerializeField] private Button _okButton;
    [SerializeField] private TextMeshProUGUI _warningText;

    public void TryCreatePlayer()
    {
        PlayerSaveInitializer.SetNewPlayer();
        SceneManager.LoadScene(LevelScene);
    }

    public void ShowWarningAndConfirm()
    {
        _warningText.gameObject.SetActive(true);
        _okButton.gameObject.SetActive(true);
    }
}
