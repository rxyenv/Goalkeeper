using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // ── Panels ────────────────────────────────────────────────────────────────
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private GameObject levelsPanel;
    [SerializeField] private GameObject settingsPanel;

    // ── HUD Text ──────────────────────────────────────────────────────────────
    [Header("HUD Text")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text scoreText;

    // ── Scene References ──────────────────────────────────────────────────────
    [Header("Scene References")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private BallController ballController;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private GameObject kicker;

    private Coroutine _fadeOutCoroutine;
    private Coroutine _holdGameCoroutine;
    private Coroutine _resumeCoroutine;

    private int _score;

    private static bool _skipMenu;
    private static bool _skipLevelPanel;

    void Start()
    {
        Time.timeScale = 0f;

        pausePanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        countdownPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        mainMenuPanel?.SetActive(false);
        levelsPanel?.SetActive(false);

        _score = 0;
        if (scoreText != null) scoreText.text = "0";

        if (!_skipMenu)
        {
            mainMenuPanel?.SetActive(true);
            return;
        }

        _skipMenu = false;

        if (_skipLevelPanel)
        {
            _skipLevelPanel = false;
            Time.timeScale = 1f;
            ballController?.StartBall();
        }
        else
        {
            levelsPanel?.SetActive(true);
        }
    }

    public void AddScore()
    {
        _score++;
        if (scoreText != null) scoreText.text = _score.ToString();
    }

    public void Losegoal()
    {
        _fadeOutCoroutine = StartCoroutine(FadeOutGameObjects());
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
        Time.timeScale = 0f;
        gameOverPanel?.SetActive(true);
    }

    public void Beginner() => SetDifficulty(0);
    public void Intermediate() => SetDifficulty(1);
    public void Difficult() => SetDifficulty(2);

    public void SetDifficulty(int level)
    {
        PlayerPrefs.SetInt("Level", level);
        _score = 0;
        if (scoreText != null) scoreText.text = "0";
        gameOverPanel?.SetActive(false);
        if (player != null) player.enabled = true;
        if (ballController != null) ballController.enabled = true;
        levelsPanel?.SetActive(false);
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
                    mainMenuPanel?.SetActive(true);
                });
        }
        else
        {
            pausePanel?.SetActive(false);
            settingsPanel?.SetActive(false);
            gameOverPanel?.SetActive(false);
            mainMenuPanel?.SetActive(true);
        }
    }

    private void ResetGameState()
    {
        Time.timeScale = 1f;
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
        _skipMenu = true;
        _skipLevelPanel = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
