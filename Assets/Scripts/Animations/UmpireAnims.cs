using System.Collections;
using UnityEngine;

public class UmpireAnims : MonoBehaviour
{
    [SerializeField] private AudioClip startWhistleClip;
    [SerializeField] private AudioClip ballMissClip;
    [SerializeField] private BallController ballController;
    [SerializeField]private PlayerMovement playerMovement;
    [SerializeField] private GoalLine goalLine;
    private AudioSource audioSource;
    private Animator umpireAnimator;

    private void Start()
    {
        umpireAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        ballController.onBallKick+=HandlePlayerKick;
        goalLine.onBallMiss+=HandleBallMiss;
        playerMovement.onBallStop+=HandleBallStopped;
    }

    private void HandlePlayerKick()
    {
        umpireAnimator.SetTrigger("StartWhistle");
        StartCoroutine(PlayDelayedSFX(startWhistleClip));
        ballController.onBallKick-=HandlePlayerKick;
    }

    private IEnumerator PlayDelayedSFX(AudioClip clip, float delay = 0.4f)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(clip);
    }

    private void HandleBallStopped()
    {
        umpireAnimator.SetTrigger("SafeSignal");
        VFXManager.instance.PlayGroundTouchEffect();
    }

    private void HandleBallMiss()
    {
        umpireAnimator.SetTrigger("GoalWhistle");
        VFXManager.instance.PlayGroundTouchEffect();
        StartCoroutine(PlayDelayedSFX(ballMissClip, 0.5f));
        
    }
    void OnDestroy()
    {
        goalLine.onBallMiss-=HandleBallMiss;
        playerMovement.onBallStop-=HandleBallStopped;
    }



}
