using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [Header("Movement")]
  [Tooltip("Units per second the goalkeeper moves toward the target lane position.")]
  [SerializeField] private float speed = 12f;

  [Tooltip("World-space distance between adjacent lanes. Must match the ball's lane spacing.")]
  [SerializeField] private float laneDistance = 4f;

  [Header("References")]
  [Tooltip("Notified on ball save — increments score and triggers green flash.")]
  [SerializeField] private UIManager uiManager;

  [Tooltip("Plays the save sound effect on ball contact.")]
  [SerializeField] private AudioManager audioManager;

  [Tooltip("Registers the save (increases speed multiplier) on ball contact.")]
  [SerializeField] private BallController ballController;

    private static readonly int MoveHash = Animator.StringToHash("Move");
    private const int TotalLanes = 3;
    private int _currentLane;
    private float _targetX;
    private Rigidbody _rb;
    private Animator _animator;
    private bool _scoredThisBall;

  void Start()
  {
    _rb = GetComponent<Rigidbody>();
    if (_rb == null)
    {
      Debug.LogError("Rigidbody not found on " + gameObject.name, this);
      enabled = false;
      return;
    }
    _rb.freezeRotation = true;
    _rb.isKinematic = true;
    _currentLane = TotalLanes / 2;
    _targetX = 0f;
    _animator = GetComponent<Animator>();
    if (_animator == null)
      Debug.LogWarning("Animator not found on " + gameObject.name, this);
    // Snap to center lane immediately so first FixedUpdate has correct target
    Vector3 pos = _rb.position;
    pos.x = _targetX;
    _rb.position = pos;
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
    {
      _currentLane++;
      _animator?.SetTrigger(MoveHash);
    }
    else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
    {
      _currentLane--;
      _animator?.SetTrigger(MoveHash);
    }
    _currentLane = Mathf.Clamp(_currentLane, 0, TotalLanes - 1);
    _targetX = (_currentLane - (TotalLanes / 2)) * laneDistance;
  }

  void FixedUpdate()
  {
    Vector3 targetPosition = new Vector3(_targetX, _rb.position.y, _rb.position.z);
    _rb.MovePosition(Vector3.MoveTowards(_rb.position, targetPosition, speed * Time.fixedDeltaTime));
  }

  public void ResetSaveGuard() => _scoredThisBall = false;

  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.CompareTag("Ball"))
    {
      Debug.Log($"[Save] OnCollisionEnter ball. _scoredThisBall={_scoredThisBall}");
      if (!_scoredThisBall)
      {
        _scoredThisBall = true;
        Debug.Log($"[Save] ScoreIncrease called. totalSaves will increment.");
        uiManager?.ScoreIncrease();
        audioManager?.PlaySave();
        ballController?.RegisterSave();
      }
      else
      {
        Debug.Log("[Save] Duplicate collision blocked by guard.");
      }
    }
  }

  void OnCollisionExit(Collision collision)
  {
    if (collision.gameObject.CompareTag("Ball"))
    {
      Debug.Log($"[Save] OnCollisionExit ball. _scoredThisBall was={_scoredThisBall} → resetting to false.");
      _scoredThisBall = false;
    }
  }
}
