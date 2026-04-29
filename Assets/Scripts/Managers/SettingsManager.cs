using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("AudioSource controlled by the volume slider and music toggle.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Toggle that mutes / unmutes the AudioSource.")]
    [SerializeField] private Toggle musicToggle;

    [Tooltip("Slider (0–1) that sets AudioSource.volume in real time.")]
    [SerializeField] private Slider volumeSlider;

    [Header("Settings Panels")]
    [Tooltip("Panel shown when the Audio tab is active in settings.")]
    [SerializeField] private GameObject musicPanel;

    [Tooltip("Panel shown when the Help / Instructions tab is active in settings.")]
    [SerializeField] private GameObject instructionsPanel;

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
        if (audioSource != null)
            audioSource.volume = value;
    }

    private void OnMusicToggled(bool isOn)
    {
        if (audioSource != null)
            audioSource.mute = !isOn;
    }

    void OnEnable()
    {
        ShowAudio();
    }

    public void ShowAudio()
    {
        musicPanel?.SetActive(true);
        instructionsPanel?.SetActive(false);
    }

    public void ShowHelp()
    {
        musicPanel?.SetActive(false);
        instructionsPanel?.SetActive(true);
    }
}
