using UnityEngine;

public class HandleCollisionDroneRacing : MonoBehaviour
{
    [SerializeField] private DroneRacing_ScoreSystem scoreSystem;
    [SerializeField] private DroneRacing_ShowFinishUI showFinishUI;
    [SerializeField] private int scorePerCurrency = 10;
    [SerializeField] private ParticleSystem obstacleExplosionPrefab;

    private DroneMovementRacingDrone droneMovement;

    private void Awake()
    {
        droneMovement = GetComponent<DroneMovementRacingDrone>();
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other, other.ClosestPoint(transform.position));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPoint = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
        HandleHit(collision.collider, hitPoint);
    }

    private void HandleHit(Collider other, Vector3 hitPoint)
    {
        Debug.Log("Trigger entered: " + other.gameObject.name);

        if (other.CompareTag("Currency"))
        {
            Debug.Log("Collided with currency!");
            Destroy(other.gameObject);

            scoreSystem.AddScore(scorePerCurrency);
            droneMovement?.AddBatteryCharge(1);
        }

        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Hit obstacle!");

            if (obstacleExplosionPrefab != null)
            {
                Instantiate(obstacleExplosionPrefab, hitPoint, Quaternion.identity);
            }

            droneMovement?.ApplyObstacleHit(hitPoint);

            Transform obstacleToDestroy = FindObstacleTransform(other.transform);
            if (obstacleToDestroy != null)
            {
                Destroy(obstacleToDestroy.gameObject);
            }
        }

        if (other.CompareTag("FinishLine"))
        {
            showFinishUI.ShowFinishLinePanel(scoreSystem.CurrentScore);
        }
    }

    private Transform FindObstacleTransform(Transform start)
    {
        Transform current = start;

        while (current != null)
        {
            if (current.CompareTag("Obstacle"))
            {
                return current;
            }

            current = current.parent;
        }

        return start;
    }
}
