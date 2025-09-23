using TMPro;
using UnityEngine;
using YG;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    
    private const string English = "en";
    private const string Russian = "ru";
    private const string Turkish = "tr";
    private int _numberEnglish = 1;
    private int _numberRussian = 0;
    private int _numberTurkish = 2;
    
    private void Awake()
    {
        _dropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    public void CreateLanguageToSetting(string language)
    {
        int number;
        
        switch (language)
        {
            case English: number = _numberEnglish;
                break;
            case Russian: number = _numberRussian;
                break;
            case Turkish: number = _numberTurkish;
                break;
            default: number = _numberEnglish;
                break;
        }
        
        OnLanguageChanged(number);
        _dropdown.SetValueWithoutNotify(number);
    }

    private void OnLanguageChanged(int index)
    {
        string language;
        
        switch (index)
        {
            case 0: language = Russian;
                break;
            case 1: language = English;
                break;
            case 2:  language = Turkish;
                break;
            default:  language = English;
                break;
        }
        
        YG2.SwitchLanguage(language);
    }
}