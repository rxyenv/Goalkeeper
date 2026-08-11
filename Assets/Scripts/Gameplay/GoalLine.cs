using UnityEngine;
using System.Collections;
using System;

public class GoalLine : MonoBehaviour
{
  [Header("References")]
  [Tooltip("Notified when a goal is conceded — decrements lives and triggers flash.")]
  [SerializeField] private UIManager uiManager;

  [Tooltip("Registers the goal (resets streak/speed) and triggers ball reset.")]
  private static readonly int victoryHash=Animator.StringToHash("Victory");
  [SerializeField] private BallController ballController;
  [SerializeField] private BackGroundTeamManager backGroundTeamManager;
  [SerializeField]private Animator kickerAnimator;
  [SerializeField] private Transform mainCamera;
  [SerializeField] private CrowdController crowdController;
  
  public Action onBallMiss;


  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Ball"))
    {
      kickerAnimator?.SetTrigger(victoryHash);
      onBallMiss?.Invoke();
      backGroundTeamManager.PlayLose();
      crowdController.PlaySad();
      StartCoroutine(CameraShake());
      uiManager?.Losegoal();
      AudioManager.instance.PlayGoal();
      AudioManager.instance.PlayBallNetHit();
      ballController?.RegisterGoal();
      ballController?.TriggerReset();
    }
  }
  IEnumerator CameraShake()
  {
    Vector3 originalPosition = mainCamera.position;
    mainCamera.position = originalPosition + new Vector3(0.30f, 0.20f, 0);
    yield return new WaitForSeconds(0.04f);
    mainCamera.position = originalPosition + new Vector3(-0.30f, -0.20f, 0);
    yield return new WaitForSeconds(0.04f);
    mainCamera.position = originalPosition + new Vector3(0.22f, -0.15f, 0);
    yield return new WaitForSeconds(0.04f);
    mainCamera.position = originalPosition + new Vector3(-0.18f, 0.12f, 0);
    yield return new WaitForSeconds(0.04f);
    mainCamera.position = originalPosition;
  }
}

