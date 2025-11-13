using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace EachOneMatters.UI.MainMenuUI
{
    public class SettingsUI : MonoBehaviour
    {
        private const string English = "en";
        private const string Russian = "ru";
        private const string Turkish = "tr";

        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private Toggle _tutorialToggle;
        [SerializeField] private Slider _mouseSensitivity;

        private int _numberEnglish = 1;
        private int _numberRussian = 0;
        private int _numberTurkish = 2;
        private Coroutine _waitForSaves;

        private void Awake()
        {
            _dropdown.onValueChanged.AddListener(OnLanguageChanged);
            _tutorialToggle.onValueChanged.AddListener(OnTutorialChanged);
            _mouseSensitivity.onValueChanged.AddListener(ChangedMouseSensitivity);
        }

        private void OnDisable()
        {
            if (_waitForSaves != null)
            {
                StopCoroutine(_waitForSaves);
            }
        }

        private void Start()
        {
            _waitForSaves = StartCoroutine(WaitForSaves());
        }

        public void CreateLanguageToSetting(string language)
        {
            int number;

            switch (language)
            {
                case English:
                    number = _numberEnglish;
                    break;

                case Russian:
                    number = _numberRussian;
                    break;

                case Turkish:
                    number = _numberTurkish;
                    break;

                default:
                    number = _numberEnglish;
                    break;
            }

            OnLanguageChanged(number);
            _dropdown.SetValueWithoutNotify(number);
        }

        private IEnumerator WaitForSaves()
        {
            while (YG2.saves == null)
            {
                yield return null;
            }

            _tutorialToggle.SetIsOnWithoutNotify(YG2.saves.gameplay.IsFirstLaunch);
            _mouseSensitivity.SetValueWithoutNotify(YG2.saves.settings.SpeedSideMovement);
        }

        private void ChangedMouseSensitivity(float speed)
        {
            YG2.saves.settings.SpeedSideMovement = speed;
            YG2.SaveProgress();
        }

        private void OnTutorialChanged(bool isPlayGameGuide)
        {
            YG2.saves.gameplay.IsFirstLaunch = isPlayGameGuide;
            YG2.SaveProgress();
        }

        private void OnLanguageChanged(int index)
        {
            string language;

            switch (index)
            {
                case 0:
                    language = Russian;
                    break;

                case 1:
                    language = English;
                    break;

                case 2:
                    language = Turkish;
                    break;

                default:
                    language = English;
                    break;
            }

            YG2.SwitchLanguage(language);
        }
    }
}