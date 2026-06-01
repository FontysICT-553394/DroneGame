using UnityEngine;

public class RocketMover : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 2.5f;
    [SerializeField] private float destroyXPosition = -20f;

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.GameRunning)
        {
            return;
        }

        float gameSpeed = 6f;

        if (GameManager.Instance != null)
        {
            gameSpeed = GameManager.Instance.GameSpeed;
        }

        transform.position += Vector3.left * gameSpeed * speedMultiplier * Time.deltaTime;

        if (transform.position.x <= destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}

