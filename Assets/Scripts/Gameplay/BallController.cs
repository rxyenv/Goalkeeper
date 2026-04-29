using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
	[Header("Shot Forces")]
	[Tooltip("Base impulse applied along the forward direction each kick. Higher = faster shots.")]
	[SerializeField] private float baseForwardForce = 18f;

	[Tooltip("Base upward impulse applied each kick. Controls default arc height.")]
	[SerializeField] private float baseUpwardForce = 8f;

	[Tooltip("World-space Z position where the ball spawns / resets before each kick.")]
	[SerializeField] private float startingZPos = 78f;

	[Header("Difficulty Scaling")]
	[Tooltip("Speed multiplier added per consecutive save. Increases ball speed as player performs well.")]
	[SerializeField] private float speedIncreasePerSave = 0.4f;

	[Tooltip("Hard cap on the speed multiplier. Ball will never exceed this multiple of base force.")]
	[SerializeField] private float maxSpeedMultiplier = 2.2f;

	[Header("Height Limit")]
	[Tooltip("Maximum world-space Y the ball can reach. Clamps position and zeroes upward velocity above this.")]
	[SerializeField] private float maxBallHeight = 2.5f;

	[Header("References")]
	[Tooltip("The kicker GameObject — repositioned and reanimated before each kick.")]
	[SerializeField] private GameObject kicker;

	[Tooltip("Rest world-space position the kicker returns to before each kick. Set this in the Inspector to the kicker's idle position.")]
	[SerializeField] private Vector3 kickerRestPosition;

	[Tooltip("Rest local rotation the kicker returns to before each kick.")]
	[SerializeField] private Vector3 kickerRestRotationEuler;

	[Tooltip("Plays kick, save, and goal sound effects.")]
	[SerializeField] private AudioManager audioManager;

	[Tooltip("Animator on the kicker rig — receives the Kick trigger before each shot.")]
	[SerializeField] private Animator ballKickAnimator;

	private enum ShotType { Driven, Lofted, Curling, Knuckleball, PowerDriven }

	private static readonly int KickHash = Animator.StringToHash("Kick");
	private readonly float[] lanes = { -4f, 0f, 4f };

	private Rigidbody _rb;
	private bool _isResetting;
	private Vector3 _startPosition;
	private Quaternion _kickerRestRotation;
	private Coroutine _activeCoroutine;
	private Coroutine _knuckleballCoroutine;
	private bool _isGameOver;

	private float _speedMultiplier = 1f;
	private int _saveStreak;
	private bool _isShot;

	void Start()
	{
	  _rb = GetComponent<Rigidbody>();
	  if (_rb == null)
	  {
	    Debug.LogError("Rigidbody not found on " + gameObject.name, this);
	    enabled = false;
	    return;
	  }
	  _startPosition = new Vector3(0, transform.position.y, startingZPos);
	  _kickerRestRotation = Quaternion.Euler(kickerRestRotationEuler);
	  _rb.position = _startPosition;
	  _rb.linearVelocity = Vector3.zero;
	  _rb.angularVelocity = Vector3.zero;
	}

	// Called by UIManager when gameplay starts (difficulty chosen or restart)
	public void StartBall()
	{
	  if (_isGameOver || _activeCoroutine != null) return;
	  _activeCoroutine = StartCoroutine(BallWait());
	}

	void FixedUpdate()
	{
	  if (!_isShot) return;

	  if (transform.position.y > maxBallHeight)
	  {
	    Vector3 pos = transform.position;
	    pos.y = maxBallHeight;
	    transform.position = pos;
	    if (_rb.linearVelocity.y > 0f)
	    {
	      Vector3 vel = _rb.linearVelocity;
	      vel.y = 0f;
	      _rb.linearVelocity = vel;
	    }
	  }
	}


	public void RegisterSave()
	{
	  _saveStreak++;
	  _speedMultiplier = Mathf.Min(1f + _saveStreak * speedIncreasePerSave, maxSpeedMultiplier);
	  Debug.Log($"[BallController] RegisterSave → _saveStreak={_saveStreak}, _speedMultiplier={_speedMultiplier:F2}");
	}

	public void RegisterGoal()
	{
	  _saveStreak = 0;
	  _speedMultiplier = Mathf.Max(1f, _speedMultiplier - speedIncreasePerSave);
	}

	private void Shoot()
	{
	  if (_isGameOver) return;
	  _isShot = true;

	      ShotType shot = PickShotType();
	      float targetX = lanes[Random.Range(0, lanes.Length)];
	      float speedMult = _speedMultiplier;

	      switch (shot)
	      {
	          case ShotType.Driven:
	              ShootDriven(targetX, speedMult);
	              break;
	          case ShotType.Lofted:
	              ShootLofted(targetX, speedMult);
	              break;
	          case ShotType.Curling:
	              ShootCurling(targetX, speedMult);
	              break;
	          case ShotType.Knuckleball:
	              ShootKnuckleball(targetX, speedMult);
	              break;
	          case ShotType.PowerDriven:
	              ShootPowerDriven(targetX, speedMult);
	              break;
	      }

	  audioManager?.PlayKick();
	}

	// Flat, fast, low trajectory — hardest to read
	  private void ShootDriven(float targetX, float speedMult)
	  {
	      Vector3 dir = new Vector3(targetX - transform.position.x, 0f, 30f).normalized;
	      float power = baseForwardForce * speedMult * Random.Range(0.95f, 1.1f);
	      _rb.AddForce(dir * power, ForceMode.Impulse);
	      _rb.AddForce(Vector3.up * baseUpwardForce * 0.5f, ForceMode.Impulse);
	  }

	  // High arc, drops sharply — goalkeeper must wait
	  private void ShootLofted(float targetX, float speedMult)
	  {
	      Vector3 dir = new Vector3(targetX - transform.position.x, 0f, 30f).normalized;
	      float power = baseForwardForce * speedMult * Random.Range(0.75f, 0.9f);
	      _rb.AddForce(dir * power, ForceMode.Impulse);
	      _rb.AddForce(Vector3.up * baseUpwardForce * 1.6f, ForceMode.Impulse);
	  }

	  // Aimed slightly off target then curves toward goal
	  private void ShootCurling(float targetX, float speedMult)
	  {
	      float offset = targetX > 0f ? -3f : 3f;
	      Vector3 dir = new Vector3((targetX + offset) - transform.position.x, 0f, 30f).normalized;
	      float power = baseForwardForce * speedMult * Random.Range(0.85f, 1.0f);
	      _rb.AddForce(dir * power, ForceMode.Impulse);
	      _rb.AddForce(Vector3.up * baseUpwardForce * 0.9f, ForceMode.Impulse);
	  }

	  // Unpredictable wobble via mid-flight perturbations
	  private void ShootKnuckleball(float targetX, float speedMult)
	  {
	      Vector3 dir = new Vector3(targetX - transform.position.x, 0f, 30f).normalized;
	      float power = baseForwardForce * speedMult * Random.Range(0.8f, 0.95f);
	      _rb.AddForce(dir * power, ForceMode.Impulse);
	      _rb.AddForce(Vector3.up * baseUpwardForce * 0.8f, ForceMode.Impulse);
	      _knuckleballCoroutine = StartCoroutine(KnuckleballPerturbations());
	  }

	  // Maximum power, slight upward — intimidating
	  private void ShootPowerDriven(float targetX, float speedMult)
	  {
	      Vector3 dir = new Vector3(targetX - transform.position.x, 0f, 30f).normalized;
	      float power = baseForwardForce * speedMult * Random.Range(1.15f, 1.35f);
	      _rb.AddForce(dir * power, ForceMode.Impulse);
	      _rb.AddForce(Vector3.up * baseUpwardForce * 0.6f, ForceMode.Impulse);
	  }

	private IEnumerator KnuckleballPerturbations()
	{
	  for (int i = 0; i < 6; i++)
	  {
	    yield return new WaitForSeconds(0.08f);
	    if (_isGameOver || _isResetting) yield break;
	    _rb.AddForce(new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 1f), 0f), ForceMode.Impulse);
	  }
	}

	private ShotType PickShotType()
	{
	  // Weight distribution shifts at higher speeds — more power shots
	  float randomValue = Random.value;
	  if (_speedMultiplier < 1.4f)
	  {
	          // Early game: mostly driven/lofted, some curl
	          if (randomValue < 0.35f) return ShotType.Driven;
	          if (randomValue < 0.60f) return ShotType.Lofted;
	          if (randomValue < 0.80f) return ShotType.Curling;
	          if (randomValue < 0.92f) return ShotType.Knuckleball;
	          return ShotType.PowerDriven;
	      }
	      else
	      {
	          // Late game: more power, more curl
	          if (randomValue < 0.25f) return ShotType.Driven;
	          if (randomValue < 0.40f) return ShotType.Lofted;
	          if (randomValue < 0.62f) return ShotType.Curling;
	          if (randomValue < 0.72f) return ShotType.Knuckleball;
	          return ShotType.PowerDriven;
	  }
	}

	public void StopBall()
	{
	  _isGameOver = true;
	  _isShot = false;
	  if (_activeCoroutine != null)
	  {
	    StopCoroutine(_activeCoroutine);
	    _activeCoroutine = null;
	  }
	  if (_knuckleballCoroutine != null)
	  {
	    StopCoroutine(_knuckleballCoroutine);
	    _knuckleballCoroutine = null;
	  }
	  if (_rb != null)
	  {
	    _rb.linearVelocity = Vector3.zero;
	    _rb.angularVelocity = Vector3.zero;
	  }
	}

	IEnumerator BallWait()
	{
	  if (kicker != null)
	  {
	    kicker.transform.position = kickerRestPosition;
	    kicker.transform.localRotation = _kickerRestRotation;
	  }
	  if (ballKickAnimator != null)
	  {
	    ballKickAnimator.Play("Kick", 0, 0f);
	    ballKickAnimator.SetTrigger(KickHash);
	  }
	  yield return new WaitForSeconds(2.1f);
	  _activeCoroutine = null;
	  if (!_isGameOver)
	    Shoot();
	}

	void OnCollisionEnter(Collision collision)
	{
	  if (_isResetting) return;
	  if (collision.gameObject.CompareTag("Player"))
	    TriggerReset();
	}

	public void TriggerReset()
	{
	  if (_isResetting) return;
	  _isResetting = true;
	  if (_activeCoroutine != null)
	  {
	    StopCoroutine(_activeCoroutine);
	    _activeCoroutine = null;
	  }
	  if (_knuckleballCoroutine != null)
	  {
	    StopCoroutine(_knuckleballCoroutine);
	    _knuckleballCoroutine = null;
	  }
	  _activeCoroutine = StartCoroutine(ResetBall());
	}

	private IEnumerator ResetBall()
	{
	  yield return new WaitForSeconds(1f);
	  if (_rb != null)
	  {
	    _rb.linearVelocity = Vector3.zero;
	    _rb.angularVelocity = Vector3.zero;
	  }
	  _isShot = false;
	  if (kicker != null)
	  {
	    kicker.transform.position = kickerRestPosition;
	    kicker.transform.localRotation = _kickerRestRotation;
	  }
	  _rb.position = _startPosition;
	  _isResetting = false;
	  _activeCoroutine = StartCoroutine(BallWait());
	}
}
