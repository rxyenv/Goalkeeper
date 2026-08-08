using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [Tooltip("The AudioSource component used for all one-shot SFX playback.")]
    [SerializeField] private AudioSource audioSource;

    [Header("Sound Clips")]
    [Tooltip("Played when the kicker strikes the ball.")]
    [SerializeField] private AudioClip[] kickSounds;

    [Tooltip("Played when the goalkeeper successfully saves a shot.")]
    [SerializeField] private AudioClip saveSound;

    [Tooltip("Played when the ball crosses the goal line.")]
    [SerializeField] private AudioClip goalSound;

    [Tooltip("Played after the game-over fade-out sequence completes.")]
    [SerializeField] private AudioClip gameOverSound;

    private void Play(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        if (audioSource.isPlaying)
            audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
    
    public void PlayKick() => Play(kickSounds[Random.Range(0, kickSounds.Length)]);
    public void PlaySave() => Play(saveSound);
    public void PlayGoal() => Play(goalSound);
    public void PlayGameOver() => Play(gameOverSound);

}
