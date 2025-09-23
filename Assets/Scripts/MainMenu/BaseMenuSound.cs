using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using YG;

public class BaseMenuSound : MonoBehaviour
{
    private const string MusicVolumeParam = "MusicVolume";
    private const string SfxVolumeParam   = "SFXVolume";
    private const string UiVolumeParam    = "UiVolume";
    
    [SerializeField] private AudioSource _baseMenuMusic;
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Slider _sliderUi;
    [SerializeField] private Slider _sliderSFX;
    [SerializeField] private AudioMixer _audioMixer;

    private void Start()
    {
        _sliderMusic.SetValueWithoutNotify(YG2.saves.VolumeMusic);
        _sliderSFX.SetValueWithoutNotify(YG2.saves.VolumeSFX);
        _sliderUi.SetValueWithoutNotify(YG2.saves.VolumeUi);
        
        _audioMixer.SetFloat(MusicVolumeParam, YG2.saves.VolumeMusic);
        _audioMixer.SetFloat(SfxVolumeParam, YG2.saves.VolumeSFX);
        _audioMixer.SetFloat(UiVolumeParam, YG2.saves.VolumeUi);
        
        _sliderMusic.onValueChanged.AddListener(CreateVolumeMusic);
        _sliderUi.onValueChanged.AddListener(CreateVolumeUi);
        _sliderSFX.onValueChanged.AddListener(CreateVolumeSFX);
    }

    public void CreateMainMenuMusic()
    {
        _baseMenuMusic.Play();
    }

    private void CreateVolumeMusic(float volume)
    {
        _audioMixer.SetFloat(MusicVolumeParam, volume);
        YG2.saves.VolumeMusic = volume;
        YG2.SaveProgress();
    }
    
    private void CreateVolumeUi(float volume)
    {
        _audioMixer.SetFloat(UiVolumeParam, volume);
        YG2.saves.VolumeUi = volume;
        YG2.SaveProgress();
    }
    
    private void CreateVolumeSFX(float volume)
    {
        _audioMixer.SetFloat(SfxVolumeParam, volume);
        YG2.saves.VolumeSFX = volume;
        YG2.SaveProgress();
    }
}
