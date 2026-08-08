using System.Collections;
using UnityEngine;

public class UmpireAnims : MonoBehaviour
{
    [SerializeField] private AudioClip startWhistleClip;
    [SerializeField] private AudioClip ballMissClip;
    private AudioSource audioSource;
    private Animator umpireAnimator;

    private void Start()
    {
        umpireAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        //sub for ball controller when ball kicked
        //sub for ball controller when ball stopped
    }

    private void HandlePlayerKick()
    {
        umpireAnimator.SetTrigger("StartWhistle");
        StartCoroutine(PlayDelayedSFX(startWhistleClip));
        //unsub the ball controller
    }

    private IEnumerator PlayDelayedSFX(AudioClip clip, float delay = 0.4f)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(clip);
    }

    private void HandleBallStopped()
    {
        umpireAnimator.SetTrigger("SafeSignal");
    }

    private void HandleBallMiss()
    {
        umpireAnimator.SetTrigger("GoalWhistle");
        StartCoroutine(PlayDelayedSFX(ballMissClip, 0.5f));
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            HandlePlayerKick();
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            HandleBallStopped();
        }
        if( Input.GetKeyDown(KeyCode.W))
        {
            HandleBallMiss();
        }
    }
}
