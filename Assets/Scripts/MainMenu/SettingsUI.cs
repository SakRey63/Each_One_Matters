using TMPro;
using UnityEngine;
using YG;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    
    private const string English = "en";
    private const string Russian = "ru";
    private const string Turkish = "tr";
    
    private string _language;

    private void OnEnable()
    {
        _dropdown.SetValueWithoutNotify(YG2.saves.NumberLanguage);
    }

    public void OnLanguageChanged(int index)
    {
        switch (index)
        {
            case 0:  _language = Russian;
                break;
            case 1: _language = English;
                break;
            case 2:  _language = Turkish;
                break;
            default:  _language = Russian;
                break;
        }

        YG2.saves.NumberLanguage = index;
        YG2.SwitchLanguage(_language);
        YG2.SaveProgress();
    }
}
