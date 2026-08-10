using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int maxLives = 3;

    [Header("Win Targets")]
    [SerializeField] private int[] winTargets = { 10, 20, 30 };

    [Header("Scoring")]
    [SerializeField] private int savesPerStreakTick = 3;



    public int Lives { get; private set; }
    public int TotalSaves { get; private set; }
    public int SaveStreak { get; private set; }
    public int BestScore { get; private set; }
    public Action OnGameStarted;
    public bool IsGameStarted = false;

    public event Action<int> OnSaveScored;
    public event Action<int> OnStreakMilestone;
    public event Action<int> OnLiveLost;
    public event Action<int, int> OnWin;

    public static GameManager instance;
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        InitGame();
        
    }

    private void Start()
    {
        OnGameStarted += HandleGameStart;
        AudioManager.instance.PlayBgm();
    }

    public void HandleGameStart()
    {
        IsGameStarted = true;
    }
    private void OnDestroy()
    {
        OnGameStarted = HandleGameStart;
    }

    public void InitGame()
    {
        Lives = maxLives;
        TotalSaves = 0;
        SaveStreak = 0;
        BestScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void RegisterSave()
    {
        SaveStreak++;
        TotalSaves++;

        OnSaveScored?.Invoke(TotalSaves);

        if (savesPerStreakTick > 0 && SaveStreak % savesPerStreakTick == 0)
            OnStreakMilestone?.Invoke(SaveStreak);

        int level = PlayerPrefs.GetInt("Level", 0);
        int target = level < winTargets.Length ? winTargets[level] : winTargets[0];
        if (TotalSaves >= target)
            TriggerWin();
    }

    public void RegisterGoal()
    {
        SaveStreak = 0;
        Lives--;
        if (Lives <= 0)
            BestScore = CommitHighScore();
        OnLiveLost?.Invoke(Lives);
    }

    private void TriggerWin()
    {
        BestScore = CommitHighScore();
        OnWin?.Invoke(TotalSaves, BestScore);
    }

    public int CommitHighScore()
    {
        int prev = PlayerPrefs.GetInt("HighScore", 0);
        if (TotalSaves > prev) PlayerPrefs.SetInt("HighScore", TotalSaves);
        BestScore = Mathf.Max(TotalSaves, prev);
        return BestScore;
    }
}
