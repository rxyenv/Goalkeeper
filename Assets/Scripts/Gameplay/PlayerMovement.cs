using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [Header("Movement")]
  [Tooltip("Units per second the goalkeeper moves toward the target lane position.")]
  [SerializeField] private float speed = 12f;

  [Tooltip("World-space distance between adjacent lanes. Must match the ball's lane spacing.")]
  [SerializeField] private float laneDistance = 5f;

  [Header("References")]
  [SerializeField] private BallController ballController;
  [SerializeField] private CrowdController crowdController;
  [SerializeField] private BackGroundTeamManager backGroundTeamManager;

    private static readonly int leftDiveHash=Animator.StringToHash("LeftDive");
    private static readonly int rightDiveDash=Animator.StringToHash("RightDive");
    private static readonly int headerHash=Animator.StringToHash("Header");
    private static readonly int leftMoveHash=Animator.StringToHash("LeftMove");
    private static readonly int rightMoveHash=Animator.StringToHash("RightMove");
    private static readonly int playAfterLeftDiveHash=Animator.StringToHash("AfterLeftDive");
    private static readonly int playAfterRightDiveHash=Animator.StringToHash("AfterRightDive");
    private const int TotalLanes = 5;
    public int _currentLane;
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

    pos = _rb.position;
    pos.x = _targetX;
    _rb.position = pos;
    OnDiveFinished();
  }

  void Update()
  {
    if (_isDiving)
      return;
    if (Input.GetKeyDown(KeyCode.Space))
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            TriggerLeftDive();
            return;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            TriggerRightDive();
            return;
        }
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
    }
    else
    {
      canHeader=false;
    }
  }
  public void TriggerLeftDive()
  {
    if (!_isDiving)
    {
      if(_currentLane==4)
        return;
      else
      {
        _currentLane+=2;
        _animator.applyRootMotion=true;
        _isDiving=true;
        AudioManager.instance.PlayPlayerDiveSound();
        _animator?.SetTrigger(leftDiveHash);
        Invoke(nameof(OnDiveFinished), 2.5f);
      }
      
    }
    
  }
  public void TriggerRightDive()
  {
    if (!_isDiving)
    {
      if(_currentLane==0)
        return;
      else
      {
        _currentLane-=2;
        _isDiving=true;
        _animator.applyRootMotion=true;
        AudioManager.instance.PlayPlayerDiveSound();
        _animator?.SetTrigger(rightDiveDash);
        Invoke(nameof(OnDiveFinished), 2.5f);
      }
    }
    
  }

  public void OnDiveFinished()
  {
    _isDiving=false;
    _animator.applyRootMotion=false;
    if (_currentLane==2)
    {
      _animator?.SetBool(playAfterLeftDiveHash,false);
      _animator?.SetBool(playAfterRightDiveHash,false);
    }
    else
    {
      _animator?.SetBool(playAfterRightDiveHash,true);
      _animator?.SetBool(playAfterLeftDiveHash,true);
    }
    _currentLane = Mathf.Clamp(_currentLane, 0, TotalLanes - 1);
    Vector3 currentPos = _rb.position;
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
