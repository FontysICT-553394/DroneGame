using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score")]
    [SerializeField] private float score;
    [SerializeField] private int coins;

    [Header("Difficulty")]
    [SerializeField] private float gameSpeed = 6f;
    [SerializeField] private float speedIncreaseRate = 0.15f;
    [SerializeField] private float maxGameSpeed = 16f;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;

    public float GameSpeed => gameSpeed;
    public bool GameRunning => gameRunning;

    private bool gameRunning = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateUI();
    }

    private void Update()
    {
        if (!gameRunning) return;

        score += Time.deltaTime * 10f;

        gameSpeed += speedIncreaseRate * Time.deltaTime;
        gameSpeed = Mathf.Clamp(gameSpeed, 0f, maxGameSpeed);

        UpdateUI();
    }

    public void AddCoin()
    {
        if (!gameRunning) return;

        coins++;
        UpdateUI();
    }

    public void GameOver()
    {
        gameRunning = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + Mathf.FloorToInt(score);
        }

        Debug.Log("Game Over");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(score);
        }

        if (coinText != null)
        {
            coinText.text = "Coins: " + coins;
        }
    }
}