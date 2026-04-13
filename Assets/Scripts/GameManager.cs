using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("UI Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("UI Text Objects (Text o TMP)")]
    [SerializeField] private GameObject currentScoreTextObject;
    [SerializeField] private GameObject gameOverScoreTextObject;
    [SerializeField] private GameObject bestScoreTextObject;
    [SerializeField] private GameObject startHintTextObject;

    [Header("Gameplay")]
    [SerializeField] private bool autoRestartOnTap = true;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip scoreSound;

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
        if (!IsStartInputPressed())
        {
            return;
        }

        if (State == GameState.WaitingToStart)
        {
            StartGame();
        }
        else if (State == GameState.GameOver && autoRestartOnTap)
        {
            RestartGame();
        }
    }
    

    public void AddScore(int amount = 1)
    {
        if (State != GameState.Playing)
        {
            return;
        }

        currentScore += amount;
        audioSource.PlayOneShot(scoreSound);
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
        SetUIText(gameOverScoreTextObject, $"Current Score: {currentScore}");
        SetUIText(bestScoreTextObject, $"Best Score: {bestScore}");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

        SetUIText(startHintTextObject, "Tap screen or press X to fly");
        RefreshScoreUI();
        SetUIText(bestScoreTextObject, $"Best Score: {bestScore}");

        bird.OnWaitingToStart();
        pipeSpawner.ResetSpawner();
    }

    private void RefreshScoreUI()
    {
        SetUIText(currentScoreTextObject, currentScore.ToString());
    }

    private static void SetUIText(GameObject textObject, string value)
    {
        if (textObject == null)
        {
            return;
        }

        TMP_Text tmpText = textObject.GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = value;
            return;
        }

        Text legacyText = textObject.GetComponent<Text>();
        if (legacyText != null)
        {
            legacyText.text = value;
        }
    }

    private static bool IsStartInputPressed()
    {
        bool touchPressed = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        return Input.GetMouseButtonDown(0) || touchPressed || Input.GetKeyDown(KeyCode.X);
    }
}