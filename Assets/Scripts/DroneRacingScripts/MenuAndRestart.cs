using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAndRestart : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainScene";

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}