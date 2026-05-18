using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float resetXPosition = -20f;
    [SerializeField] private float startXPosition = 20f;

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (transform.position.x <= resetXPosition)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = startXPosition;
            transform.position = newPosition;
        }
    }
}