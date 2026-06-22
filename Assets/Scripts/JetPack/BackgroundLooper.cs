using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.5f;
    [SerializeField] private float backgroundWidth = 23.75f;
    [SerializeField] private float leftResetX = -23.75f;

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.GameRunning)
        {
            return;
        }

        float speed = 4f;

        if (GameManager.Instance != null)
        {
            speed = GameManager.Instance.GameSpeed * speedMultiplier;
        }

        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= leftResetX)
        {
            transform.position += Vector3.right * backgroundWidth * 2f;
        }
    }
}
