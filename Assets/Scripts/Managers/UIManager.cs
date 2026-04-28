using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject PausePannel;
    [SerializeField] private GameObject MainMenuPannel;
    [SerializeField] private GameObject GameOverPannel;
    [SerializeField] private GameObject WinPannel;
    [SerializeField] private GameObject LivesPannel;
    [SerializeField] private GameObject CountdownPannel;
    [SerializeField] private TMP_Text CountdownText;
    [SerializeField] private GameObject SettingsPannel;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI winScoreText;
    [SerializeField] private int beginnerWinScore = 5;
    [SerializeField] private int intermediateWinScore = 10;
    [SerializeField] private int difficultWinScore = 15;

    public GameObject LevelsPannel;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private BallController ballController;
    [SerializeField] private SettingsManager settingsManager;

    [SerializeField] private GameObject Ball1;
    [SerializeField] private GameObject Ball2;
    [SerializeField] private GameObject Ball3;
    [SerializeField] private GameObject Kicker;
    [SerializeField] private Audiomanager audiomanager;

    private int goals = 3;
    private int points;

    private static bool SkipMenu;
    private static bool SkipLevelPanel;

    void Start()
    {
        Time.timeScale = 0f;
        if (score != null)
            score.text = "Points: 0";

        if (!SkipMenu)
        {
            MainMenuPannel?.SetActive(true);
            LivesPannel?.SetActive(false);
            return;
        }

        MainMenuPannel?.SetActive(false);
        SkipMenu = false;

        if (SkipLevelPanel)
        {
            LevelsPannel?.SetActive(false);
            LivesPannel?.SetActive(true);
            SkipLevelPanel = false;
            Time.timeScale = 1f;
        }
        else
        {
            LevelsPannel?.SetActive(true);
            LivesPannel?.SetActive(false);
        }
    }

    public void ScoreIncrease()
    {
        points++;
        if (score != null)
            score.text = "Points: " + points;

        int targetScore = PlayerPrefs.GetInt("Level") switch
        {
            0 => beginnerWinScore,
            1 => intermediateWinScore,
            2 => difficultWinScore,
            _ => beginnerWinScore
        };

        if (points >= targetScore)
        {
            WinGame(targetScore);
        }
    }

    private void WinGame(int targetScore)
    {
        ballController?.StopBall();
        if (ballController != null) ballController.enabled = false;
        if (player != null) player.enabled = false;

        if (winScoreText != null)
            winScoreText.text = "POINTS: " + points;

        Time.timeScale = 0f;
        WinPannel?.SetActive(true);
    }

    public void Losegoal()
    {
        goals--;

        if (goals == 2)
        {
            Ball3?.SetActive(false);
        }
        else if (goals == 1)
        {
            Ball2?.SetActive(false);
        }
        else if (goals == 0)
        {
            Ball1?.SetActive(false);
            StartCoroutine(FadeOutGameObjects());
        }
    }

    private IEnumerator FadeOutGameObjects()
    {
        ballController?.StopBall();
        if (ballController != null) ballController.enabled = false;
        if (player != null) player.enabled = false;

        GameObject[] fadeObjects = new GameObject[] { ballController?.gameObject, Kicker };
        float duration = 1f;
        float timer = 0f;

        Vector3[] initialScales = new Vector3[fadeObjects.Length];
        for (int i = 0; i < fadeObjects.Length; i++)
            if (fadeObjects[i] != null)
                initialScales[i] = fadeObjects[i].transform.localScale;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float scale = 1f - t;
            for (int i = 0; i < fadeObjects.Length; i++)
            {
                if (fadeObjects[i] != null)
                    fadeObjects[i].transform.localScale = initialScales[i] * scale;
            }
            yield return null;
        }

        for (int i = 0; i < fadeObjects.Length; i++)
            fadeObjects[i]?.SetActive(false);

        StartCoroutine(HoldGame());
    }

    private IEnumerator HoldGame()
    {
        yield return new WaitForSeconds(3f);
        audiomanager?.PlayGameOver();
        GameOver();
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        if (finalScoreText != null)
            finalScoreText.text = "POINTS: " + points;
        GameOverPannel?.SetActive(true);
    }

    public void Beginner()
    {
        PlayerPrefs.SetInt("Level", 0);
        LevelsPannel?.SetActive(false);
        LivesPannel?.SetActive(true);
        Time.timeScale = 1f;
    }

    public void Intermediate()
    {
        PlayerPrefs.SetInt("Level", 1);
        LevelsPannel?.SetActive(false);
        LivesPannel?.SetActive(true);
        Time.timeScale = 1f;
    }

    public void Difficult()
    {
        PlayerPrefs.SetInt("Level", 2);
        LevelsPannel?.SetActive(false);
        LivesPannel?.SetActive(true);
        Time.timeScale = 1f;
    }

    public void StartButton()
    {
        SkipMenu = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowMenu()
    {
        MainMenuPannel?.SetActive(false);
        LevelsPannel?.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void ShowPause()
    {
        PausePannel?.SetActive(true);
        Time.timeScale = 0f;
        if (player != null) player.enabled = false;
        if (ballController != null) ballController.enabled = false;
    }

    public void ResumeButton()
    {
        StartCoroutine(ResumeCountdown());
    }

    public void HomeButton()
    {
        PausePannel?.SetActive(false);
        SettingsPannel?.SetActive(false);
        GameOverPannel?.SetActive(false);
        MainMenuPannel?.SetActive(true);
    }

    public void BackButton()
    {
        SettingsPannel?.SetActive(false);
    }

    public void ShowSettings()
    {
        SettingsPannel?.SetActive(true);
    }

    public void Restart()
    {
        points = 0;
        SkipMenu = true;
        SkipLevelPanel = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator ResumeCountdown()
    {
        PausePannel?.SetActive(false);
        CountdownPannel?.SetActive(true);
        Time.timeScale = 0f;
        if (player != null) player.enabled = false;
        if (ballController != null) ballController.enabled = false;

        if (CountdownText != null) CountdownText.text = "3";
        yield return new WaitForSecondsRealtime(1f);

        if (CountdownText != null) CountdownText.text = "2";
        yield return new WaitForSecondsRealtime(1f);

        if (CountdownText != null) CountdownText.text = "1";
        yield return new WaitForSecondsRealtime(1f);

        CountdownPannel?.SetActive(false);
        Time.timeScale = 1f;
        if (ballController != null) ballController.enabled = true;
        if (player != null) player.enabled = true;
    }
}
