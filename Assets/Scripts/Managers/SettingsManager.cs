using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public Toggle musicToggle;
    public Slider volumeSlider;
    [SerializeField] private GameObject musicPanel;
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
