using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceInvaderUIManager : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private TMP_Text scoreTextGameOver;
    [SerializeField] private TMP_Text scoreTextInGame;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject gameOverUI;
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void GameOver(int score)
    {
        gameOverUI.SetActive(true);
        scoreTextGameOver.text = $"Score: {score}";
    
        Time.timeScale = 0;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1;
        
        if (gameOverUI != null) 
            gameOverUI.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        scoreTextInGame.text = $"Score: {score}";
    }
    
    public void UpdateHealth(int health)
    {
        healthText.text = $"Lives: {health}";
    }

    public void RestartBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void MainMenuBtn()
    {
        SceneManager.LoadScene(0);
    }
}
