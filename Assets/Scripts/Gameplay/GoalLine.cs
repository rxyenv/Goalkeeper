using UnityEngine;

public class GoalLine : MonoBehaviour
{
	[Header("References")]
	[Tooltip("Notified when a goal is conceded — decrements lives and triggers flash.")]
	[SerializeField] private UIManager uiManager;

	[Tooltip("Plays the goal sound effect.")]
	[SerializeField] private AudioManager audioManager;

	[Tooltip("Registers the goal (resets streak/speed) and triggers ball reset.")]
	[SerializeField] private BallController ballController;

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Ball"))
		{
			uiManager?.Losegoal();
			audioManager?.PlayGoal();
			ballController?.RegisterGoal();
			ballController?.TriggerReset();
		}
	}
}
