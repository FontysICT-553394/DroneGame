using UnityEngine;

public class RocketMovement : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 2.6f;
    [SerializeField] private float destroyXPosition = -25f;

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.GameRunning)
        {
            return;
        }

        float speed = 8f;

        if (GameManager.Instance != null)
        {
            speed = GameManager.Instance.GameSpeed * speedMultiplier;
        }

        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}