using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private float forwardForce = 18f;

    [SerializeField]
    private float upwardForce = 10f;

    [SerializeField]
    private float startingZPos = 78f;

    [SerializeField]
    private float curveForce = 14f;

    [SerializeField]
    private GameObject kicker;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private Animator ballKickAnimator;

    [SerializeField]
    private PlayerMovement player;

    [SerializeField]
    private UIManager uiManager;

    //[SerializeField]
    //private BackGroundTeamManager backGroundTeamManager;


    private static readonly int KickHash = Animator.StringToHash("Kick");
    private readonly float[] lanes = { -8f, -4f ,0 ,4f ,8f };
    private Rigidbody rb;
    private bool isResetting;
    private Vector3 startPosition;
    private Vector3 kickerStartPosition;
    private Quaternion kickerStartRotation;
    private Vector3 kickerStartScale;
    private Vector3 direction;
    private Coroutine activeCoroutine;
    private bool isGameOver;
    private bool _pendingSave;
    public bool shouldCurve=false;
    private float laneX;
    public float curveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = new Vector3(0, transform.position.y, startingZPos);
        if (kicker != null)
        {
            kickerStartPosition = kicker.transform.position;
            kickerStartRotation = kicker.transform.localRotation;
            kickerStartScale = kicker.transform.localScale;
        }
        transform.position = startPosition;
        activeCoroutine = StartCoroutine(BallWait());
    }

    public void StartBall()
    {
        if (isGameOver || activeCoroutine != null) return;
        activeCoroutine = StartCoroutine(BallWait());
    }

    public void ResetGame()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
        isGameOver = false;
        isResetting = false;
        _pendingSave = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
        transform.localScale = Vector3.one;
        if (kicker != null)
        {
            kicker.SetActive(true);
            kicker.transform.position = kickerStartPosition;
            kicker.transform.localRotation = kickerStartRotation;
            kicker.transform.localScale = kickerStartScale;
        }
        activeCoroutine = StartCoroutine(BallWait());
    }

    public void RegisterSave() => _pendingSave = true;

    public void RegisterGoal() => _pendingSave = false;

    void Shoot()
    {
        if (isGameOver)
            return;
        player?.ResetSaveGuard();
        laneX = lanes[Random.Range(0, lanes.Length)];
        shouldCurve = Mathf.Abs(laneX) == 8f;
        curveDirection = Mathf.Sign(laneX);
        if (shouldCurve)
        {
            float wideAimX=laneX+(curveDirection*5f);
            direction = new Vector3(wideAimX - transform.position.x, 0, 26f).normalized;
            rb.AddForce(direction * forwardForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * (upwardForce-1f), ForceMode.Impulse);
        }
        else
        {
            direction = new Vector3(laneX - transform.position.x, 0, 26f).normalized;
            rb.AddForce(direction * forwardForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
        }

        
        
        
        audioManager?.PlayKick();
    }
    private void FixedUpdate()
    {
        if (shouldCurve)
        {
            rb.AddForce(Vector3.right * -curveDirection * curveForce, ForceMode.Force);
        }
    }

    public void StopBall()
    {
        isGameOver = true;
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    IEnumerator BallWait()
    {
        if (kicker != null)
        {
            kicker.transform.position = kickerStartPosition;
            kicker.transform.localRotation = kickerStartRotation;
        }
        if (ballKickAnimator != null)
        {
            ballKickAnimator.Play("Kick", 0, 0f);
            ballKickAnimator.SetTrigger(KickHash);
        }
        yield return new WaitForSeconds(2.1f);
        if (!isGameOver)
            Shoot();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isResetting)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            TriggerReset();
            //backGroundTeamManager.PlayWin();
        }

    }

    public void TriggerReset()
    {
        if (isResetting)
            return;
        isResetting = true;
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ResetBall());
    }

    private IEnumerator ResetBall()
    {
        yield return new WaitForSeconds(1.3f);
        shouldCurve = false;
        if (_pendingSave)
        {
            _pendingSave = false;
            uiManager?.ScoreIncrease();
        }
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if (kicker != null)
        {
            kicker.transform.position = kickerStartPosition;
            kicker.transform.localRotation = kickerStartRotation;
        }
        transform.position = startPosition;
        isResetting = false;
        activeCoroutine = StartCoroutine(BallWait());
    }
}
