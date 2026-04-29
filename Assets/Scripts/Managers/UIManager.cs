using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // ── Panels ────────────────────────────────────────────────────────────────
    [Header("Panels")]
    [Tooltip("Shown when the player pauses the game.")]
    [SerializeField] private GameObject pausePanel;

    [Tooltip("First screen shown on launch — Play / Quit buttons.")]
    [SerializeField] private GameObject mainMenuPanel;

    [Tooltip("Shown when the player loses all lives.")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("Shown when the player reaches the target score.")]
    [SerializeField] private GameObject winPanel;

    [Tooltip("HUD bar showing remaining lives (ball icons).")]
    [SerializeField] private GameObject livesPanel;

    [Tooltip("Overlay that counts down 3-2-1 after unpausing.")]
    [SerializeField] private GameObject countdownPanel;

    [Tooltip("Difficulty selection screen shown before the first kick.")]
    [SerializeField] private GameObject levelsPanel;

    [Tooltip("Settings screen toggled from the pause menu.")]
    [SerializeField] private GameObject settingsPanel;

    // ── HUD Text ──────────────────────────────────────────────────────────────
    [Header("HUD Text")]
    [Tooltip("Live score label shown during gameplay.")]
    [SerializeField] private TextMeshProUGUI score;

    [Tooltip("Score label on the Game Over screen.")]
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Tooltip("Score label on the Win screen.")]
    [SerializeField] private TextMeshProUGUI winScoreText;

    [Tooltip("Countdown digits inside the countdown panel (3 / 2 / 1).")]
    [SerializeField] private TMP_Text countdownText;

    [Tooltip("Floating text that shows the save streak (e.g. '6 SAVES!'). Assign the TMP object; it is toggled by code.")]
    [SerializeField] private TextMeshProUGUI streakText;

    [Tooltip("High score label shown on the Game Over screen.")]
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Tooltip("High score label shown on the Win screen.")]
    [SerializeField] private TextMeshProUGUI winHighScoreText;

    // ── Win Save Targets ──────────────────────────────────────────────────────
    [Header("Win Save Targets")]
    [Tooltip("Total saves needed to win on Beginner difficulty.")]
    [SerializeField] private int beginnerWinSaves = 10;

    [Tooltip("Total saves needed to win on Intermediate difficulty.")]
    [SerializeField] private int intermediateWinSaves = 20;

    [Tooltip("Total saves needed to win on Difficult difficulty.")]
    [SerializeField] private int difficultWinSaves = 30;

    // ── Scoring ───────────────────────────────────────────────────────────────
    [Header("Scoring")]
    [Tooltip("Base points awarded per save before multiplier.")]
    [SerializeField] private int basePointsPerSave = 100;

    [Tooltip("Streak saves required to advance the multiplier by 1.")]
    [SerializeField] private int savesPerMultiplierTick = 3;

    [Tooltip("Maximum streak multiplier cap.")]
    [SerializeField] private int maxMultiplier = 5;

    // ── Lives Icons ───────────────────────────────────────────────────────────
    [Header("Lives Icons")]
    [Tooltip("Ball icon representing life 1 (last to disappear).")]
    [SerializeField] private GameObject ball1;

    [Tooltip("Ball icon representing life 2.")]
    [SerializeField] private GameObject ball2;

    [Tooltip("Ball icon representing life 3 (first to disappear).")]
    [SerializeField] private GameObject ball3;

    // ── Scene References ──────────────────────────────────────────────────────
    [Header("Scene References")]
    [Tooltip("The player goalkeeper — disabled on game over / win.")]
    [SerializeField] private PlayerMovement player;

    [Tooltip("Controls ball spawning and physics — disabled on game over / win.")]
    [SerializeField] private BallController ballController;

    [Tooltip("Manages SFX and music playback.")]
    [SerializeField] private AudioManager audioManager;

    [Tooltip("Handles graphics / audio settings persistence.")]
    [SerializeField] private SettingsManager settingsManager;

    [Tooltip("The kicker GameObject — scaled to zero on game over fade-out.")]
    [SerializeField] private GameObject kicker;

    // ── Screen Flash ──────────────────────────────────────────────────────────
    [Header("Screen Flash")]
    [Tooltip("Fullscreen UI Image used for the save/goal flash. Set Raycast Target off. Anchors should stretch to fill the canvas.")]
    [SerializeField] private Image flashImage;

    private static readonly Color SaveColor = new Color(0f, 1f, 0f, 0.45f);
    private static readonly Color GoalColor = new Color(1f, 0f, 0f, 0.45f);
    private Coroutine _flashCoroutine;
    private Coroutine _fadeOutCoroutine;
    private Coroutine _holdGameCoroutine;
    private Coroutine _resumeCoroutine;

    private int _goals = 3;
    private int _points;
    private int _saveStreak;
    private int _totalSaves;
    private int _multiplier = 1;

    private static bool _skipMenu;
    private static bool _skipLevelPanel;

    void Start()
    {
        Time.timeScale = 0f;

        // Reset all panels to known-off state so saved scene state doesn't bleed in
        pausePanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        winPanel?.SetActive(false);
        countdownPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        mainMenuPanel?.SetActive(false);
        levelsPanel?.SetActive(false);
        livesPanel?.SetActive(false);

        // Re-enable all life icons
        ball1?.SetActive(true);
        ball2?.SetActive(true);
        ball3?.SetActive(true);

        _goals = 3;
        _points = 0;
        _saveStreak = 0;
        _totalSaves = 0;
        _multiplier = 1;

        if (score != null)
            score.text = "0";

        if (!_skipMenu)
        {
            mainMenuPanel?.SetActive(true);
            return;
        }

        _skipMenu = false;

        if (_skipLevelPanel)
        {
            _skipLevelPanel = false;
            livesPanel?.SetActive(true);
            Time.timeScale = 1f;
            ballController?.StartBall();
        }
        else
        {
            levelsPanel?.SetActive(true);
        }
    }

    public void ScoreIncrease()
    {
        _saveStreak++;
        _totalSaves++;
        _multiplier = Mathf.Min(_saveStreak / savesPerMultiplierTick + 1, maxMultiplier);
        _points += basePointsPerSave * _multiplier;
        Debug.Log($"[Score] ScoreIncrease → totalSaves={_totalSaves}, saveStreak={_saveStreak}, multiplier={_multiplier}, points={_points}");

        FlashScreen(SaveColor);

        if (score != null)
            score.text = _totalSaves.ToString();

        if (_saveStreak >= savesPerMultiplierTick && _saveStreak % savesPerMultiplierTick == 0)
            ShowStreakText(_saveStreak);

        int targetSaves = PlayerPrefs.GetInt("Level") switch
        {
            0 => beginnerWinSaves,
            1 => intermediateWinSaves,
            2 => difficultWinSaves,
            _ => beginnerWinSaves
        };

        if (_totalSaves >= targetSaves)
            WinGame();
    }

    private void WinGame()
    {
        ballController?.StopBall();
        if (ballController != null) ballController.enabled = false;
        if (player != null) player.enabled = false;

        int prev = PlayerPrefs.GetInt("HighScore", 0);
        if (_totalSaves > prev)
            PlayerPrefs.SetInt("HighScore", _totalSaves);
        int best = Mathf.Max(_totalSaves, prev);

        if (winScoreText != null)
            winScoreText.text = _totalSaves + " SAVES";
        if (winHighScoreText != null)
            winHighScoreText.text = "BEST: " + best.ToString("N0");

        Time.timeScale = 0f;
        winPanel?.SetActive(true);
    }

    public void Losegoal()
    {
        _saveStreak = 0;
        _multiplier = 1;
        FlashScreen(GoalColor);
        _goals--;
        if (_goals == 2)
            ball3?.SetActive(false);
        else if (_goals == 1)
            ball2?.SetActive(false);
        else if (_goals == 0)
        {
            ball1?.SetActive(false);
            _fadeOutCoroutine = StartCoroutine(FadeOutGameObjects());
        }
    }

    private IEnumerator FadeOutGameObjects()
    {
        ballController?.StopBall();
        if (ballController != null) ballController.enabled = false;
        if (player != null) player.enabled = false;

        GameObject[] fadeObjects = { ballController?.gameObject, kicker };
        float duration = 1f;
        float timer = 0f;

        Vector3[] initialScales = new Vector3[fadeObjects.Length];
        for (int index = 0; index < fadeObjects.Length; index++)
            if (fadeObjects[index] != null)
                initialScales[index] = fadeObjects[index].transform.localScale;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float scale = 1f - timer / duration;
            for (int index = 0; index < fadeObjects.Length; index++)
                if (fadeObjects[index] != null)
                    fadeObjects[index].transform.localScale = initialScales[index] * scale;
            yield return null;
        }

        for (int index = 0; index < fadeObjects.Length; index++)
            fadeObjects[index]?.SetActive(false);

        _holdGameCoroutine = StartCoroutine(HoldGame());
    }

    private IEnumerator HoldGame()
    {
        yield return new WaitForSeconds(3f);
        audioManager?.PlayGameOver();
        GameOver();
    }

    private void GameOver()
    {
        int prev = PlayerPrefs.GetInt("HighScore", 0);
        if (_totalSaves > prev)
            PlayerPrefs.SetInt("HighScore", _totalSaves);
        int best = Mathf.Max(_totalSaves, prev);

        Time.timeScale = 0f;
        if (finalScoreText != null)
            finalScoreText.text = _totalSaves + " SAVES";
        if (highScoreText != null)
            highScoreText.text = "BEST: " + best.ToString("N0");
        gameOverPanel?.SetActive(true);
    }

    public void Beginner() => SetDifficulty(0);
    public void Intermediate() => SetDifficulty(1);
    public void Difficult() => SetDifficulty(2);

    // Called by UI buttons with int arg: 0=Beginner, 1=Intermediate, 2=Difficult
    public void SetDifficulty(int level)
    {
        PlayerPrefs.SetInt("Level", level);
        levelsPanel?.SetActive(false);
        livesPanel?.SetActive(true);
        Time.timeScale = 1f;
        ballController?.StartBall();
    }

    public void StartButton()
    {
        _skipMenu = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowMenu()
    {
        mainMenuPanel?.SetActive(false);
        levelsPanel?.SetActive(true);
    }

    public void QuitButton() => Application.Quit();

    public void ShowPause()
    {
        if (pausePanel == null) return;
        pausePanel.SetActive(true);
        pausePanel.transform.localScale = Vector3.zero;
        Time.timeScale = 0f;
        if (player != null) player.enabled = false;
        if (ballController != null) ballController.enabled = false;
        pausePanel.transform.DOKill();
        pausePanel.transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack, 3.5f)
            .SetUpdate(true);
    }

    public void ResumeButton()
    {
        if (pausePanel != null)
        {
            pausePanel.transform.DOKill();
            pausePanel.transform.DOScale(Vector3.zero, 0.35f)
                .SetEase(Ease.InBack, 3.5f)
                .SetUpdate(true)
                .OnComplete(() => {
                    pausePanel.SetActive(false);
                    StartCoroutine(ResumeCountdown());
                });
        }
        else
        {
            _resumeCoroutine = StartCoroutine(ResumeCountdown());
        }
    }

    public void HomeButton()
    {
        ResetGameState();
        if (pausePanel != null && pausePanel.activeSelf)
        {
            pausePanel.transform.DOKill();
            pausePanel.transform.DOScale(Vector3.zero, 0.35f)
                .SetEase(Ease.InBack, 3.5f)
                .SetUpdate(true)
                .OnComplete(() => {
                    pausePanel.SetActive(false);
                    settingsPanel?.SetActive(false);
                    gameOverPanel?.SetActive(false);
                    livesPanel?.SetActive(false);
                    mainMenuPanel?.SetActive(true);
                });
        }
        else
        {
            pausePanel?.SetActive(false);
            settingsPanel?.SetActive(false);
            gameOverPanel?.SetActive(false);
            livesPanel?.SetActive(false);
            mainMenuPanel?.SetActive(true);
        }
    }

    private void ResetGameState()
    {
        Time.timeScale = 1f;
        _points = 0;
        _saveStreak = 0;
        _totalSaves = 0;
        _multiplier = 1;
        if (score != null) score.text = "0";
        ball1?.SetActive(true);
        ball2?.SetActive(true);
        ball3?.SetActive(true);
        if (ballController != null)
        {
            ballController.StopBall();
            ballController.enabled = true;
        }
        if (player != null) player.enabled = true;
    }

    public void BackButton() => settingsPanel?.SetActive(false);

    public void ShowSettings() => settingsPanel?.SetActive(true);

    public void Restart()
    {
        _points = 0;
        _skipMenu = true;
        _skipLevelPanel = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FlashScreen(Color color)
    {
        if (flashImage == null) return;
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashCoroutine(color));
    }

    private IEnumerator FlashCoroutine(Color color)
    {
        flashImage.color = color;
        flashImage.gameObject.SetActive(true);
        float duration = 0.35f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(color.a, 0f, timer / duration);
            flashImage.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }
        flashImage.gameObject.SetActive(false);
    }

    private void ShowStreakText(int streak)
    {
        if (streakText == null) return;

        streakText.DOKill();
        streakText.text = streak + " SAVES!";
        streakText.gameObject.SetActive(true);

        streakText.transform.DOKill();
        streakText.transform.localScale = Vector3.one;
        streakText.transform.DOPunchScale(Vector3.one * 0.4f, 0.3f, 6, 0.5f);

        streakText.DOFade(1f, 0f)
            .OnComplete(() =>
                streakText.DOFade(0f, 0.6f)
                    .SetDelay(0.8f)
                    .OnComplete(() => streakText.gameObject.SetActive(false)));
    }

    private IEnumerator ResumeCountdown()
    {
        pausePanel?.SetActive(false);
        countdownPanel?.SetActive(true);
        Time.timeScale = 0f;
        if (player != null) player.enabled = false;
        if (ballController != null) ballController.enabled = false;

        string[] counts = { "3", "2", "1" };
        foreach (string count in counts)
        {
            if (countdownText != null) countdownText.text = count;
            yield return new WaitForSecondsRealtime(1f);
        }

        _resumeCoroutine = null;
        countdownPanel?.SetActive(false);
        Time.timeScale = 1f;
        if (ballController != null) ballController.enabled = true;
        if (player != null) player.enabled = true;
    }

    void OnDestroy()
    {
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);
        if (_fadeOutCoroutine != null)
            StopCoroutine(_fadeOutCoroutine);
        if (_holdGameCoroutine != null)
            StopCoroutine(_holdGameCoroutine);
        if (_resumeCoroutine != null)
        {
            StopCoroutine(_resumeCoroutine);
            Time.timeScale = 1f;
            if (ballController != null) ballController.enabled = true;
            if (player != null) player.enabled = true;
        }
    }
}
