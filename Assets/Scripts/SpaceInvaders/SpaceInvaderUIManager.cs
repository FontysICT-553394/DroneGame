using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceInvaderUIManager : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private TMP_Text scoreTextGameOver;
    [SerializeField] private TMP_Text scoreTextWin;
    [SerializeField] private TMP_Text scoreTextInGame;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winUI;
    
    [SerializeField] private List<GameObject> enemyLayers = new List<GameObject>();
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Update()
    {
        int enemiesLeft = 0;
        foreach(GameObject enemyLayer in enemyLayers)
            enemiesLeft += enemyLayer.transform.childCount;

        if (enemiesLeft <= 0)
            ShowWin(GameObject.Find("Player-2D").GetComponent<SpaceInvadersPlayerStats>().score);
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

    private void ShowWin(int score)
    {
        winUI.SetActive(true);
        scoreTextWin.text = $"Score: {score}";
    
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
