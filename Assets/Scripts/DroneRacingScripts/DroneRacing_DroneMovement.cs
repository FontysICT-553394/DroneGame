using UnityEngine;
using UnityEngine.InputSystem;

public class DroneMovemetnRacingDrone : MonoBehaviour
{
    private Vector2 movementInput;
    private Rigidbody rb;

    [SerializeField] private float moveSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }


    void FixedUpdate()
    {
        Vector3 velocity = new Vector3(
            0f,
            movementInput.y * moveSpeed,  
        movementInput.x * moveSpeed   
        );

        rb.linearVelocity = velocity;
    }
}
