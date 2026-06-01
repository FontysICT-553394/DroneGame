using UnityEngine;
using UnityEngine.InputSystem;

public class DroneMovementRacingDrone : MonoBehaviour
{
    private Vector2 movementInput;
    private Vector2 delayedInput;
    private Rigidbody rb;

    [SerializeField] private float moveSpeed = 5f;

    [Header("Drone feel")]
    [SerializeField] private float inputLatency = 4f;     // lager = meer vertraging
    [SerializeField] private float acceleration = 8f;     // lager = trager op snelheid
    [SerializeField] private float dragWhenNoInput = 3f;  // hoger = sneller afremmen

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Input reageert niet instant, maar loopt erachteraan
        delayedInput = Vector2.Lerp(
            delayedInput,
            movementInput,
            inputLatency * Time.fixedDeltaTime
        );

        Vector3 targetVelocity = new Vector3(
            0f,
            delayedInput.y * moveSpeed,
            delayedInput.x * moveSpeed
        );

        // Drone accelereert naar de target velocity i.p.v. direct teleport-speed
        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );

        // Als je geen input geeft, zweeft/remt hij langzaam af
        if (movementInput.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = Vector3.Lerp(
                rb.linearVelocity,
                Vector3.zero,
                dragWhenNoInput * Time.fixedDeltaTime
            );
        }
    }
}