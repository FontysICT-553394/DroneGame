using UnityEngine;
using TMPro;

public class DroneRacing_ShowFinishUI : MonoBehaviour
{
    [SerializeField] private GameObject finishLinePanel;
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        if (finishLinePanel != null)
        {
            finishLinePanel.SetActive(false);
        }
    }

    public void ShowFinishLinePanel(int score)
    {
        if (finishLinePanel != null)
        {
            finishLinePanel.SetActive(true);
            scoreText.text = "Your Score: " + score.ToString();
        }
    }
}
