using UnityEngine;
using System.Collections;
using System;

public class GoalLine : MonoBehaviour
{
  [Header("References")]
  [Tooltip("Notified when a goal is conceded — decrements lives and triggers flash.")]
  [SerializeField] private UIManager uiManager;

  [Tooltip("Plays the goal sound effect.")]
  [SerializeField] private AudioManager audioManager;

  [Tooltip("Registers the goal (resets streak/speed) and triggers ball reset.")]
  [SerializeField] private BallController ballController;
  [SerializeField] private BackGroundTeamManager backGroundTeamManager;
  [SerializeField] private Transform mainCamera;
  public Action onBallMiss;
  private Vector3 cameraPosition;
  void Start()
  {
    cameraPosition = mainCamera.localPosition;
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Ball"))
    {
      onBallMiss?.Invoke();
      backGroundTeamManager.PlayLose();
      Debug.Log("GOAL!");
      StartCoroutine(CameraShake());
      uiManager?.Losegoal();
      audioManager?.PlayGoal();
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

