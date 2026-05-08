using UnityEngine;

public class JetPackMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Jetpack Settings")]
    [SerializeField] private float gravityForce = 25f;
    [SerializeField] private float jetpackForce = 35f;
    [SerializeField] private float maxFallSpeed = -12f;
    [SerializeField] private float maxRiseSpeed = 8f;

    private Vector3 moveInput;
    private Rigidbody rb;
    private float verticalVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // We doen gravity zelf, zodat het meer als Jetpack Joyride voelt
        rb.useGravity = false;
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.z = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // Standaard val je naar beneden
        verticalVelocity -= gravityForce * Time.fixedDeltaTime;

        // Jetpack omhoog als je Space ingedrukt houdt
        if (Input.GetKey(KeyCode.Space))
        {
            verticalVelocity += jetpackForce * Time.fixedDeltaTime;

            // Directe start omhoog
            if (verticalVelocity < 2f)
            {
                verticalVelocity = 2f;
            }
        }

        // Limieten zodat het controllable blijft
        verticalVelocity = Mathf.Clamp(verticalVelocity, maxFallSpeed, maxRiseSpeed);

        Vector3 horizontalVelocity = new Vector3(moveInput.x, 0f, moveInput.z).normalized * moveSpeed;

        rb.linearVelocity = new Vector3(
            horizontalVelocity.x,
            verticalVelocity,
            horizontalVelocity.z
        );
    }
}