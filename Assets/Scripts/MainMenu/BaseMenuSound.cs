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
    [SerializeField] private Slider _sliderMusic;

    [Tooltip("Слайдер громкости интерфейса")]
    [SerializeField] private Slider _sliderUi;

    [Tooltip("Слайдер громкости SFX")]
    [SerializeField] private Slider _sliderSFX;

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
        
        float musicVol = YG2.saves.VolumeMusic;
        float sfxVol   = YG2.saves.VolumeSFX;
        float uiVol    = YG2.saves.VolumeUi;
        
        ApplyVolume(MusicVolumeParam, musicVol);
        ApplyVolume(SfxVolumeParam, sfxVol);
        ApplyVolume(UiVolumeParam, uiVol);

        if (_sliderMusic) _sliderMusic.SetValueWithoutNotify(musicVol);
        if (_sliderUi)    _sliderUi.SetValueWithoutNotify(uiVol);
        if (_sliderSFX)   _sliderSFX.SetValueWithoutNotify(sfxVol);
        
        if (_sliderMusic) _sliderMusic.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (_sliderUi)    _sliderUi.onValueChanged.AddListener(OnUiVolumeChanged);
        if (_sliderSFX)   _sliderSFX.onValueChanged.AddListener(OnSfxVolumeChanged);
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
        YG2.saves.VolumeMusic = value;
        YG2.SaveProgress();
    }

    private void OnUiVolumeChanged(float value)
    {
        ApplyVolume(UiVolumeParam, value);
        YG2.saves.VolumeUi = value;
        YG2.SaveProgress();
    }

    private void OnSfxVolumeChanged(float value)
    {
        ApplyVolume(SfxVolumeParam, value);
        YG2.saves.VolumeSFX = value;
        YG2.SaveProgress();
    }
}