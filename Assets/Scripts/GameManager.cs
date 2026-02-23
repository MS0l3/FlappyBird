using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        WaitingToStart,
        Playing,
        GameOver
    }

    private const string BestScoreKey = "BestScore";

    [Header("References")]
    [SerializeField] private BirdController bird;
    [SerializeField] private PipeSpawner pipeSpawner;

    [Header("UI")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text gameOverScoreText;
    [SerializeField] private Text bestScoreText;
    [SerializeField] private Text startHintText;

    [Header("Gameplay")]
    [SerializeField] private bool autoRestartOnTap = true;

    private int currentScore;
    private int bestScore;

    public GameState State { get; private set; } = GameState.WaitingToStart;

    private void Awake()
    {
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    private void Start()
    {
        EnterWaitingState();
    }

    private void Update()
    {
        if (IsStartInputPressed())
        {
            if (State == GameState.WaitingToStart)
            {
                StartGame();
            }
            else if (State == GameState.GameOver && autoRestartOnTap)
            {
                RestartGame();
            }
        }
    }

    public void AddScore(int amount = 1)
    {
        if (State != GameState.Playing)
        {
            return;
        }

        currentScore += amount;
        RefreshScoreUI();
    }

    public void TriggerGameOver()
    {
        if (State != GameState.Playing)
        {
            return;
        }

        State = GameState.GameOver;

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.Save();
        }

        bird.OnGameOver();
        pipeSpawner.StopSpawning();

        gameOverPanel.SetActive(true);
        gameOverScoreText.text = $"Current Score: {currentScore}";
        bestScoreText.text = $"Best Score: {bestScore}";
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void StartGame()
    {
        State = GameState.Playing;
        currentScore = 0;

        startPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        RefreshScoreUI();

        bird.OnGameStarted();
        pipeSpawner.BeginSpawning();
    }

    private void EnterWaitingState()
    {
        State = GameState.WaitingToStart;

        currentScore = 0;

        gameOverPanel.SetActive(false);
        startPanel.SetActive(true);

        if (startHintText != null)
        {
            startHintText.text = "Tap screen or press X to fly";
        }

        RefreshScoreUI();
        bestScoreText.text = $"Best Score: {bestScore}";

        bird.OnWaitingToStart();
        pipeSpawner.ResetSpawner();
    }

    private void RefreshScoreUI()
    {
        currentScoreText.text = currentScore.ToString();
    }

    private static bool IsStartInputPressed()
    {
        bool touchPressed = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        return Input.GetMouseButtonDown(0) || touchPressed || Input.GetKeyDown(KeyCode.X);
    }
}
