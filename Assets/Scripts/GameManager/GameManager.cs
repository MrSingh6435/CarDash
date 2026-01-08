using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // Required for scene management

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameOver = false;
    public bool isPaused = false;

    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject pauseGameUI;
    [SerializeField] GameObject pauseBTN;
    [SerializeField] GameObject startPanel;
    [SerializeField] private float gameOverDelay = 2.5f;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float resumeCountdownTime = 3f;


    private static bool hasGameStarted = false; // NEW: track if start panel was already used

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (!hasGameStarted)
        {
            Time.timeScale = 0f;    // Stop game at start
            startPanel.SetActive(true);
            pauseBTN.SetActive(false);
        }
        else
        {
            // Game restarted, skip start panel
            startPanel.SetActive(false);
            Time.timeScale = 1f;
            pauseBTN.SetActive(true);
        }
    }

    void Update()
    {
        // Detect Android back button / Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.instance.isPaused)
                GameManager.instance.PauseGame();
            else
                GameManager.instance.ResumeGame();
        }
    }

    // Called by Start Button
    public void StartGame()
    {
        Time.timeScale = 1f;   // Resume game
        startPanel.SetActive(false);
        pauseBTN.SetActive(true);

        hasGameStarted = true; // mark game as started
    }

    public void GameOver()
    {
        if (isGameOver) return; // safety

        isGameOver = true;
        pauseBTN.SetActive(false);

        Debug.Log("Game Over triggered, waiting before showing panel...");
        StartCoroutine(GameOverRoutine());
    }
    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(gameOverDelay);

        gameOverUI.SetActive(true);
        Time.timeScale = 0f;   // pause AFTER UI appears
    }


    // Pause the game
    public void PauseGame()
    {
        if (isGameOver) return;

        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
        pauseGameUI.SetActive(true);
        pauseBTN.SetActive(false);
    }

    // Resume the game
    public void ResumeGame()
    {
        if (!isPaused || isGameOver) return;

        StartCoroutine(ResumeCountdownRoutine());
    }

    private IEnumerator ResumeCountdownRoutine()
    {
        pauseGameUI.SetActive(false);
        pauseBTN.SetActive(false);

        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.gameObject.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;
        pauseBTN.SetActive(true);

        Debug.Log("Game Resumed after countdown");
    }


    // Restart Button
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // hasGameStarted stays true, so start panel is skipped
    }

    // Exit the game
    public void ExitGame()
    {
        Debug.Log("Exiting Game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#else
        Application.Quit(); // Quit on mobile or build
#endif
    }
}
