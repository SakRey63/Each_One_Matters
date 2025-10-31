using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using YG;

public class BaseMenuSound : MonoBehaviour
{
    private const string MusicVolumeParam = "MusicVolume";
    private const string SfxVolumeParam   = "SFXVolume";
    private const string UiVolumeParam    = "UiVolume";

    [Header("Настройки")]
    [Tooltip("Основная музыка меню")]
    [SerializeField] private AudioSource _baseMenuMusic;

    [Tooltip("Слайдер громкости музыки")]
    [SerializeField] private Slider _desctopSliderMusic;
    [SerializeField] private Slider _mobileSliderMusic;

    [Tooltip("Слайдер громкости интерфейса")]
    [SerializeField] private Slider _desctopSliderUi;
    [SerializeField] private Slider _mobileSliderUi;

    [Tooltip("Слайдер громкости SFX")]
    [SerializeField] private Slider _desctopSliderSFX;
    [SerializeField] private Slider _mobileSliderSFX;

    [Tooltip("Audio Mixer с настройками громкости")]
    [SerializeField] private AudioMixer _audioMixer;

    private Coroutine _waitForSavesLoad;
    
    private void OnDisable()
    {
        if (_waitForSavesLoad != null)
        {
            StopCoroutine(_waitForSavesLoad);
        }
    }
    
    private void Start()
    {
        _waitForSavesLoad = StartCoroutine(WaitForSavesLoad());
    }
    
    public void CreateMainMenuMusic()
    {
        if (_baseMenuMusic != null && !_baseMenuMusic.isPlaying)
        {
            _baseMenuMusic.Play();
        }
    }
    
    private IEnumerator WaitForSavesLoad()
    {
        while (YG2.saves == null)
        {
            yield return null;
        }

        Initialize();
    }
    
    private void Initialize()
    {
        if (_audioMixer == null)
        {
            Debug.LogError("BaseMenuSound: AudioMixer не назначен в инспекторе.");
            return;
        }
        
        float musicVol = YG2.saves.audio.VolumeMusic;
        float sfxVol   = YG2.saves.audio.VolumeSFX;
        float uiVol    = YG2.saves.audio.VolumeUi;
        
        ApplyVolume(MusicVolumeParam, musicVol);
        ApplyVolume(SfxVolumeParam, sfxVol);
        ApplyVolume(UiVolumeParam, uiVol);

        if (_mobileSliderMusic) _mobileSliderMusic.SetValueWithoutNotify(musicVol);
        if (_desctopSliderMusic) _desctopSliderMusic.SetValueWithoutNotify(musicVol);
        if (_mobileSliderUi) _mobileSliderUi.SetValueWithoutNotify(musicVol);
        if (_desctopSliderUi)    _desctopSliderUi.SetValueWithoutNotify(uiVol);
        if (_mobileSliderSFX)   _mobileSliderSFX.SetValueWithoutNotify(sfxVol);
        if (_desctopSliderSFX)   _desctopSliderSFX.SetValueWithoutNotify(sfxVol);
        
        if (_mobileSliderMusic) _mobileSliderMusic.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (_desctopSliderMusic) _desctopSliderMusic.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (_mobileSliderUi)    _mobileSliderUi.onValueChanged.AddListener(OnUiVolumeChanged);
        if (_desctopSliderUi)    _desctopSliderUi.onValueChanged.AddListener(OnUiVolumeChanged);
        if (_mobileSliderSFX)   _mobileSliderSFX.onValueChanged.AddListener(OnSfxVolumeChanged);
        if (_desctopSliderSFX)   _desctopSliderSFX.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    private void ApplyVolume(string paramName, float value)
    {
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat(paramName, value);
        }
        else
        {
            Debug.LogWarning($"BaseMenuSound: Не удалось установить параметр '{paramName}'. Проверь, экспортирован ли он в Audio Mixer.");
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        ApplyVolume(MusicVolumeParam, value);
        YG2.saves.audio.VolumeMusic = value;
        YG2.SaveProgress();
    }

    private void OnUiVolumeChanged(float value)
    {
        ApplyVolume(UiVolumeParam, value);
        YG2.saves.audio.VolumeUi = value;
        YG2.SaveProgress();
    }

    private void OnSfxVolumeChanged(float value)
    {
        ApplyVolume(SfxVolumeParam, value);
        YG2.saves.audio.VolumeSFX = value;
        YG2.SaveProgress();
    }
}