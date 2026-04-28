using UnityEngine;
using TMPro;

public class ScoreCalculation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Coins;
    [SerializeField] private TextMeshProUGUI Score;

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
        if (Score != null)
            Score.text = FinalScore.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            coinsCollected++;
            FinalCoins = coinsCollected;
            if (Coins != null)
                Coins.text = coinsCollected.ToString();
        }
    }
}
