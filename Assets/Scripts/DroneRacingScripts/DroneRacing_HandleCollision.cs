using UnityEngine;

public class HandleCollisionDroneRacing : MonoBehaviour
{
    [SerializeField] private DroneRacing_ScoreSystem scoreSystem;
    [SerializeField] private DroneRacing_ShowFinishUI showFinishUI;
    [SerializeField] private int scorePerCurrency = 10;

   private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered: " + other.gameObject.name);

        if (other.CompareTag("Currency"))
        {
            Debug.Log("Collided with currency!");
            Destroy(other.gameObject);

            scoreSystem.AddScore(scorePerCurrency);
        }


        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Hit obstacle trigger!");
        }

        if (other.CompareTag("FinishLine"))
        {
            showFinishUI.ShowFinishLinePanel(scoreSystem.CurrentScore);
        }
    }
}
