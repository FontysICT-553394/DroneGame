using UnityEngine;

public class HandleCollisionDroneRacing : MonoBehaviour
{
    [SerializeField] private DroneRacing_ScoreSystem scoreSystem;
    [SerializeField] private DroneRacing_ShowFinishUI showFinishUI;
    [SerializeField] private int scorePerCurrency = 10;
    [SerializeField] private ParticleSystem obstacleExplosionPrefab;
    [SerializeField] private AudioClip currencyPickupSound;
    [SerializeField] private float pickupSoundVolume = 1f;
    [SerializeField] private AudioClip obstacleExplosionSound;
    [SerializeField] private float explosionSoundVolume = 1f;

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

            if (currencyPickupSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    currencyPickupSound,
                    other.transform.position,
                    pickupSoundVolume
                );
            }

            Destroy(other.gameObject);

            scoreSystem.AddScore(scorePerCurrency);
            droneMovement?.AddBatteryCharge(1);
        }

        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Hit obstacle!");

            if (obstacleExplosionSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    obstacleExplosionSound,
                    hitPoint,
                    explosionSoundVolume
                );
            }

            if (obstacleExplosionPrefab != null)
            {
                ParticleSystem explosion = Instantiate(obstacleExplosionPrefab, hitPoint, Quaternion.identity);
                explosion.Play();
                Destroy(explosion.gameObject, explosion.main.duration + explosion.main.startLifetime.constantMax);
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
