using UnityEngine;

public class DroneRacing_PlaneMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // Rotate the plane 180 degrees so its forward direction matches movement
        transform.Rotate(0f, 180f, 0f, Space.Self);
    }

    private void Update()
    {
        Vector3 leftDirection = -mainCamera.transform.right;
        leftDirection.y = 0f;
        leftDirection.Normalize();

        transform.position += leftDirection * speed * Time.deltaTime;
    }
}