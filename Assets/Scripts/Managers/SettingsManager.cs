using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SettingsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Toggle _sfxToggle;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;


    void Start()
    {
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        _sfxToggle.isOn = PlayerPrefs.GetInt("IsSFXOn", 1) == 1 ? true:false;
        _sfxToggle.onValueChanged.AddListener((v) =>
        {
            OnSFXToggled(v);
        });
    }

    void OnDestroy()
    {
        if (_musicVolumeSlider != null)
            _musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        if (_sfxToggle != null)
            _sfxToggle.onValueChanged.RemoveListener(OnSFXToggled);
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

    private void OnSFXToggled(bool isOn)
    {
        if (isOn) AudioManager.instance.UnMuteSFX();
        else AudioManager.instance.MuteSFX();

        PlayerPrefs.SetInt("IsSFXOn", isOn ? 1 : 0);
    }
}
