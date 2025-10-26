using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class CharacterCreation : MonoBehaviour
{
    private const string LevelScene = "LevelScene";

    [SerializeField] private Button _okButton;
    [SerializeField] private TextMeshProUGUI _warningText;

    public void TryCreatePlayer()
    {
        YG2.saves.SetNewPlayer();
        YG2.saves.IsLoadedMainMenu = false;
        YG2.SaveProgress();
        SceneManager.LoadScene(LevelScene);
    }

    public void ShowWarningAndConfirm()
    {
        _warningText.gameObject.SetActive(true);
        _okButton.gameObject.SetActive(true);
    }
}
