using UnityEngine;

using UnityEngine;

public class RocketMovement : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 2.2f;
    [SerializeField] private float destroyXPosition = -20f;

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.GameRunning) return;

        float speed = GameManager.Instance.GameSpeed * speedMultiplier;

        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}