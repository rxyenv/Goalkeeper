using System.Collections;
using UnityEngine;

public class UmpireAnims : MonoBehaviour
{
    [SerializeField] private AudioClip startWhistleClip;
    [SerializeField] private AudioClip ballMissClip;
    [SerializeField] private BallController ballController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GoalLine goalLine;
    private Animator umpireAnimator;

    private void Start()
    {
        umpireAnimator = GetComponent<Animator>();
        goalLine.onBallMiss+=HandleBallMiss;
        playerMovement.onBallStop+=HandleBallStopped;
        GameManager.instance.OnGameStarted += HandleGameStart;

        umpireAnimator.SetTrigger("StartWhistle");

    }

    private void HandleGameStart()
    {
        umpireAnimator.SetTrigger("StartWhistle");
        AudioManager.instance.PlayUmpireClip(startWhistleClip);
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
        AudioManager.instance.PlayUmpireClip(ballMissClip);
    }

    void OnDestroy()
    {
        goalLine.onBallMiss-=HandleBallMiss;
        playerMovement.onBallStop-=HandleBallStopped;
    }



}
