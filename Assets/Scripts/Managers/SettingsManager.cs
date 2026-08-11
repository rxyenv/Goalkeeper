using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    [Tooltip("Toggle that mutes / unmutes the AudioSource.")]
    [SerializeField] private Toggle musicToggle;

    [Tooltip("Slider (0–1) that sets AudioSource.volume in real time.")]
    [SerializeField] private Slider volumeSlider;



    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = 1f;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnMusicToggled);
    }

    void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        if (musicToggle != null)
            musicToggle.onValueChanged.RemoveListener(OnMusicToggled);
    }

    private void OnVolumeChanged(float value)
    {
        AudioManager.instance.SetMusicVol(value);
    }

    private void OnMusicToggled(bool isOn)
    {
        AudioManager.instance.MuteSFX();
    }
}
