using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class ScoreCalculation : MonoBehaviour
{
    [Header("HUD Text")]
    [Tooltip("Label that shows the number of coins collected this session.")]
    [FormerlySerializedAs("Coins")]
    [SerializeField] private TextMeshProUGUI coins;

    [Tooltip("Label that shows elapsed time in seconds (used as the score in this mode).")]
    [FormerlySerializedAs("Score")]
    [SerializeField] private TextMeshProUGUI score;

    private int coinsCollected;
    private float timer;

    public static int FinalScore;
    public static int FinalCoins;

    void OnEnable()
    {
        timer = 0;
        coinsCollected = 0;
        FinalScore = 0;
        FinalCoins = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        FinalScore = (int)timer;
        if (score != null)
            score.text = FinalScore.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coinsCollected++;
            FinalCoins = coinsCollected;
            if (coins != null)
                coins.text = coinsCollected.ToString();
        }
    }
}
