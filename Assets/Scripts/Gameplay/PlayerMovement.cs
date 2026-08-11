using System;
using System.Collections;
using System.Data;
using Unity.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [Header("Movement")]
  [Tooltip("Units per second the goalkeeper moves toward the target lane position.")]
  [SerializeField] private float speed = 12f;

  [Tooltip("World-space distance between adjacent lanes. Must match the ball's lane spacing.")]
  [SerializeField] private float laneDistance = 4f;

  [Header("References")]
  [SerializeField] private BallController ballController;
  [SerializeField] private CrowdController crowdController;
  [SerializeField] private BackGroundTeamManager backGroundTeamManager;

    private static readonly int leftDiveHash=Animator.StringToHash("LeftDive");
    private static readonly int rightDiveDash=Animator.StringToHash("RightDive");
    private static readonly int headerHash=Animator.StringToHash("Header");
    private static readonly int leftMoveHash=Animator.StringToHash("LeftMove");
    private static readonly int rightMoveHash=Animator.StringToHash("RightMove");
    private const int TotalLanes = 5;
    private int _currentLane;
    private float _targetX;
    private Rigidbody _rb;
    private Animator _animator;
    private bool _scoredThisBall;
    private bool _isDiving;
    private Vector3 pos;
    private float _initialY;
    private Quaternion _initialYRotation;
    public bool canHeader=false;
    public Action onBallStop;

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
    _initialY = transform.position.y;
    _initialYRotation=transform.rotation;
    _animator = GetComponent<Animator>();
    if (_animator == null)
      Debug.LogWarning("Animator not found on " + gameObject.name, this);
    // Snap to center lane immediately so first FixedUpdate has correct target
    pos = _rb.position;
    pos.x = _targetX;
    _rb.position = pos;
  }

  void Update()
  {
    if(_isDiving)
      return;
    if (Input.GetKeyDown(KeyCode.Space))
    {
      AudioManager.instance.PlayPlayerDiveSound();
      TriggerDive();
    }
    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
    {
      _animator.applyRootMotion=false;
      _currentLane++;
      
      _animator?.SetTrigger(leftMoveHash);
    }
    else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
    {
      _animator.applyRootMotion=false;
      _currentLane--;
      
      _animator?.SetTrigger(rightMoveHash);
    }
    _currentLane = Mathf.Clamp(_currentLane, 0, TotalLanes - 1);
    _targetX = (_currentLane - (TotalLanes / 2)) * laneDistance;
  }

  void FixedUpdate()
  {
    if (!_isDiving)
    {
      Vector3 targetPosition = new Vector3(_targetX, _rb.position.y, _rb.position.z);
      _rb.MovePosition(Vector3.MoveTowards(_rb.position, targetPosition, speed * Time.fixedDeltaTime));
    }
    
  }
  public void CanCatch()
  {
    
    if(_currentLane==2 && ballController.laneX == 0f)
    {
      canHeader=true;
      Debug.Log("Checked");
    }
    else
    {
      canHeader=false;
    }
  }
  public void TriggerDive()
  {
    if (ballController.shouldCurve)
    {
      _isDiving=true;
      _animator.applyRootMotion=true;
      if (ballController.curveDirection == -1)
      {
        _animator?.SetTrigger(leftDiveHash);
      }
      else if (ballController.curveDirection == 1)
      {
        _animator?.SetTrigger(rightDiveDash);
      }
    }
    else
      return;
    
  }

  //this will be called inside animation clip at the end of dive animation
  public void OnDiveFinished()
  {
    _isDiving=false;
    _animator.applyRootMotion=false;
    Vector3 currentPos = _rb.position;
    Quaternion currentRot= _rb.rotation;
    currentPos.y = _initialY;
    _rb.position = currentPos;
    _rb.rotation=_initialYRotation;
  }

  public void ResetSaveGuard() => _scoredThisBall = false;

  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.CompareTag("Ball") && !_scoredThisBall)
    {
      if (canHeader)
      {
        _animator?.SetTrigger(headerHash);
      }
      onBallStop?.Invoke();
      backGroundTeamManager.PlayWin();
      crowdController.PlayCheer();
      _scoredThisBall = true;
      AudioManager.instance.PlaySave();
      ballController?.RegisterSave();
    }
  }
}
