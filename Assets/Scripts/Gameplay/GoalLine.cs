using UnityEngine;

public class GoalLine : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Audiomanager audioManager;
    [SerializeField] private BallController ballController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            uiManager?.Losegoal();
            audioManager?.PlayGoal();
            ballController?.TriggerReset();
        }
    }
}
