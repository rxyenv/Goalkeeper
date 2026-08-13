using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;


    void Start()
    {
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnEnable()
    {
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.instance.SetMusicVol(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.instance.SetSFXVol(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

}
