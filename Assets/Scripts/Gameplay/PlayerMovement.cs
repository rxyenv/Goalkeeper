using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float laneDistance = 4f;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Audiomanager audioManager;

    private static readonly int MoveHash = Animator.StringToHash("Move");
    private const int TotalLanes = 5;
    private int currentLane;
    private float targetX;
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentLane = TotalLanes / 2;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentLane++;
            animator?.SetTrigger(MoveHash);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentLane--;
            animator?.SetTrigger(MoveHash);
        }
        currentLane = Mathf.Clamp(currentLane, 0, TotalLanes - 1);
        targetX = (currentLane - (TotalLanes / 2)) * laneDistance;
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = new Vector3(targetX, rb.position.y, rb.position.z);
        rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            uiManager?.ScoreIncrease();
            audioManager?.PlaySave();
        }
    }
}
