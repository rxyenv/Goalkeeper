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

    [Tooltip("Plays kick, save, and goal sound effects.")]
    [SerializeField] private AudioManager audioManager;

    [Tooltip("Animator on the kicker rig — receives the Kick trigger before each shot.")]
    [SerializeField] private Animator ballKickAnimator;

    private enum ShotType { Driven, Lofted, Curling, Knuckleball, PowerDriven }

    private static readonly int KickHash = Animator.StringToHash("Kick");
    private readonly float[] lanes = { -8f, -4f, 0f, 4f, 8f };

    private Rigidbody rb;
    private bool isResetting;
    private Vector3 startPosition;
    private Vector3 kickerStartPosition;
    private Quaternion kickerStartRotation;
    private Coroutine activeCoroutine;
    private bool isGameOver;

    private float speedMultiplier = 1f;
    private int saveStreak;
    private bool isShot;

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

    void FixedUpdate()
    {
        if (!isShot) return;

        if (transform.position.y > maxBallHeight)
        {
            Vector3 pos = transform.position;
            pos.y = maxBallHeight;
            transform.position = pos;
            if (rb.linearVelocity.y > 0f)
            {
                Vector3 vel = rb.linearVelocity;
                vel.y = 0f;
                rb.linearVelocity = vel;
            }
        }
    }


    public void RegisterSave()
    {
        saveStreak++;
        speedMultiplier = Mathf.Min(1f + saveStreak * speedIncreasePerSave, maxSpeedMultiplier);
    }

    public void RegisterGoal()
    {
        saveStreak = 0;
        speedMultiplier = Mathf.Max(1f, speedMultiplier - speedIncreasePerSave);
    }

    private void Shoot()
    {
        if (isGameOver) return;
        isShot = true;

        ShotType shot = PickShotType();
        float targetX = lanes[Random.Range(0, lanes.Length)];
        float sm = speedMultiplier;

        switch (shot)
        {
            case ShotType.Driven:
                ShootDriven(targetX, sm);
                break;
            case ShotType.Lofted:
                ShootLofted(targetX, sm);
                break;
            case ShotType.Curling:
                ShootCurling(targetX, sm);
                break;
            case ShotType.Knuckleball:
                ShootKnuckleball(targetX, sm);
                break;
            case ShotType.PowerDriven:
                ShootPowerDriven(targetX, sm);
                break;
        }

        audioManager?.PlayKick();
    }

    // Flat, fast, low trajectory — hardest to read
    private void ShootDriven(float targetX, float sm)
    {
        Vector3 dir = new Vector3(targetX - transform.position.x, 0f, 30f).normalized;
        float power = baseForwardForce * sm * Random.Range(0.95f, 1.1f);
        rb.AddForce(dir * power, ForceMode.Impulse);
        rb.AddForce(Vector3.up * baseUpwardForce * 0.5f, ForceMode.Impulse);
    }

    // High arc, drops sharply — goalkeeper must wait
    private void ShootLofted(float targetX, float sm)
    {
        Vector3 dir = new Vector3(targetX - transform.position.x, 0f, 30f).normalized;
        float power = baseForwardForce * sm * Random.Range(0.75f, 0.9f);
        rb.AddForce(dir * power, ForceMode.Impulse);
        rb.AddForce(Vector3.up * baseUpwardForce * 1.6f, ForceMode.Impulse);
    }

    // Aimed slightly off target then curves toward goal
    private void ShootCurling(float targetX, float sm)
    {
        float offset = targetX > 0f ? -3f : 3f;
        Vector3 dir = new Vector3((targetX + offset) - transform.position.x, 0f, 30f).normalized;
        float power = baseForwardForce * sm * Random.Range(0.85f, 1.0f);
        rb.AddForce(dir * power, ForceMode.Impulse);
        rb.AddForce(Vector3.up * baseUpwardForce * 0.9f, ForceMode.Impulse);
    }

    // Unpredictable wobble via mid-flight perturbations
    private void ShootKnuckleball(float targetX, float sm)
    {
        Vector3 dir = new Vector3(targetX - transform.position.x, 0f, 30f).normalized;
        float power = baseForwardForce * sm * Random.Range(0.8f, 0.95f);
        rb.AddForce(dir * power, ForceMode.Impulse);
        rb.AddForce(Vector3.up * baseUpwardForce * 0.8f, ForceMode.Impulse);
        StartCoroutine(KnuckleballPerturbations());
    }

    // Maximum power, slight upward — intimidating
    private void ShootPowerDriven(float targetX, float sm)
    {
        Vector3 dir = new Vector3(targetX - transform.position.x, 0f, 30f).normalized;
        float power = baseForwardForce * sm * Random.Range(1.15f, 1.35f);
        rb.AddForce(dir * power, ForceMode.Impulse);
        rb.AddForce(Vector3.up * baseUpwardForce * 0.6f, ForceMode.Impulse);
    }

    private IEnumerator KnuckleballPerturbations()
    {
        for (int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(0.08f);
            if (isGameOver || isResetting) yield break;
            rb.AddForce(new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 1f), 0f), ForceMode.Impulse);
        }
    }

    private ShotType PickShotType()
    {
        // Weight distribution shifts at higher speeds — more power shots
        float r = Random.value;
        if (speedMultiplier < 1.4f)
        {
            // Early game: mostly driven/lofted, some curl
            if (r < 0.35f) return ShotType.Driven;
            if (r < 0.60f) return ShotType.Lofted;
            if (r < 0.80f) return ShotType.Curling;
            if (r < 0.92f) return ShotType.Knuckleball;
            return ShotType.PowerDriven;
        }
        else
        {
            // Late game: more power, more curl
            if (r < 0.25f) return ShotType.Driven;
            if (r < 0.40f) return ShotType.Lofted;
            if (r < 0.62f) return ShotType.Curling;
            if (r < 0.72f) return ShotType.Knuckleball;
            return ShotType.PowerDriven;
        }
    }

    public void StopBall()
    {
        isGameOver = true;
        isShot = false;
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
        if (isResetting) return;
        if (collision.gameObject.CompareTag("Player"))
            TriggerReset();
    }

    public void TriggerReset()
    {
        if (isResetting) return;
        isResetting = true;
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ResetBall());
    }

    private IEnumerator ResetBall()
    {
        yield return new WaitForSeconds(1f);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isShot = false;
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
