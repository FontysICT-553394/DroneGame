using UnityEngine;

public class WorldMover : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float destroyXPosition = -20f;

    void Update()
    {
        if (!GameManager.Instance.GameRunning) return;

        float speed = GameManager.Instance.GameSpeed * speedMultiplier;

        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}