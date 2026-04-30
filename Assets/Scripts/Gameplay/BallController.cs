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
    private GameObject kicker;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private Animator ballKickAnimator;

    [SerializeField]
    private PlayerMovement player;

    [SerializeField]
    private UIManager uiManager;

    private static readonly int KickHash = Animator.StringToHash("Kick");
    private readonly float[] lanes = { -4f, 0f, 4f };
    private Rigidbody rb;
    private bool isResetting;
    private Vector3 startPosition;
    private Vector3 kickerStartPosition;
    private Quaternion kickerStartRotation;
    private Coroutine activeCoroutine;
    private bool isGameOver;
    private bool _pendingSave;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = new Vector3(0, transform.position.y, startingZPos);
        if (kicker != null)
        {
            kickerStartPosition = kicker.transform.position;
            kickerStartRotation = kicker.transform.localRotation;
        }
        transform.position = startPosition;
        activeCoroutine = StartCoroutine(BallWait());
    }

    public void StartBall()
    {
        if (isGameOver || activeCoroutine != null) return;
        activeCoroutine = StartCoroutine(BallWait());
    }

    public void RegisterSave() => _pendingSave = true;

    public void RegisterGoal() => _pendingSave = false;

    void Shoot()
    {
        if (isGameOver)
            return;
        player?.ResetSaveGuard();
        float laneX = lanes[Random.Range(0, lanes.Length)];
        Vector3 direction = new Vector3(laneX - transform.position.x, 0, 30f).normalized;
        rb.AddForce(direction * forwardForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
        audioManager?.PlayKick();
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
            TriggerReset();
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
        yield return new WaitForSeconds(1f);
        if (_pendingSave)
        {
            _pendingSave = false;
            uiManager?.AddScore();
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
