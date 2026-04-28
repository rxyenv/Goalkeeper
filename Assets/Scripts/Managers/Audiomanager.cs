using UnityEngine;

public class Audiomanager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip kickSound;
    [SerializeField] private AudioClip saveSound;
    [SerializeField] private AudioClip goalSound;
    [SerializeField] private AudioClip gameOverSound;

    private void Play(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        if (audioSource.isPlaying)
            audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayKick() => Play(kickSound);
    public void PlaySave() => Play(saveSound);
    public void PlayGoal() => Play(goalSound);
    public void PlayGameOver() => Play(gameOverSound);
}
