using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _ambineceSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip[] kickSounds;
    [SerializeField] private AudioClip crowdCheerSound;
    [SerializeField] private AudioClip goalSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip btnClickSound;
    [SerializeField] private AudioClip streakSound;
    [SerializeField] private AudioClip crowdShoutSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip balHitImpact;
    [SerializeField] private AudioClip ballNetHitSound;
    [SerializeField] private AudioClip bgm;
    [SerializeField] private AudioClip diveSound;


    public static AudioManager instance;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        GameManager.instance.OnGameStarted += HandleGameStart;
    }

    private void OnDestroy()
    {
        GameManager.instance.OnGameStarted -= HandleGameStart;
    }

    private void HandleGameStart()
    {
        PlayCrowdShoutAmbience();
    }

    public void PlayKick() => _sfxSource.PlayOneShot(kickSounds[Random.Range(0, kickSounds.Length)], 20f);
    public void PlaySave()
    {
        _sfxSource.pitch = GetRandomPitch();
        _sfxSource.PlayOneShot(crowdCheerSound, 1.5f);
        StartCoroutine(ResetSFXPitch());
    }
    private IEnumerator ResetSFXPitch(){
        yield return new WaitForSeconds(1.2f);
        _sfxSource.pitch = 1;
    }
    public void PlayGoal() => _sfxSource.PlayOneShot(goalSound);
    public void PlayGameOver() => _sfxSource.PlayOneShot(gameOverSound);
    public void PlayBtnClick() => _sfxSource.PlayOneShot(btnClickSound);
    public void PlayStreakSound() => _sfxSource.PlayOneShot(streakSound);
    public void PlayWin() => _sfxSource.PlayOneShot(winSound);
    public void PlayBallHitImpact() => _sfxSource.PlayOneShot(balHitImpact, 1.5f);
    public void PlayBallNetHit() => _sfxSource.PlayOneShot(ballNetHitSound);
    public void PlayPlayerDiveSound() =>StartCoroutine(PlayDelayedSFX(diveSound));

    private IEnumerator PlayDelayedSFX(AudioClip clip, float dealy = 0.2f)
    {
        yield return new WaitForSeconds(dealy);
        _sfxSource.PlayOneShot(clip, 1.5f);
    }
    public void PlayCrowdShoutAmbience()
    {
        if(_ambineceSource.isPlaying) _ambineceSource.Stop();

        _ambineceSource.clip = crowdShoutSound;
        _ambineceSource.loop = true;
        _ambineceSource.volume = 0.8f;
        _ambineceSource.Play();
    }
    public void PlayBgm()
    {
        if(_musicSource.isPlaying) _musicSource.Stop();

        _musicSource.clip = bgm;
        _musicSource.loop = true;
        _musicSource.volume = 0.5f;
        _musicSource.Play();
    }

    public void StopBgm()
    {
        if (_musicSource.isPlaying) _musicSource.Stop();
    }

    private float GetRandomPitch()
    {
        return Random.Range(0.85f, 1.2f);
    }
}
