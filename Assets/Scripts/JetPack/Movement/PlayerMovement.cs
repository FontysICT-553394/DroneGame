using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 3f;
    private float gravity = 9.8f;
    private Vector3 moveInput;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // X and Z map to standard horizontal/forward movements
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.z = Input.GetAxisRaw("Vertical");

        // Y maps to vertical movement (Space for Up, Left Shift for Down)
        moveInput.y = 0f;
        if (Input.GetKey(KeyCode.Space)) moveInput.y += 1f;
        if (Input.GetKey(KeyCode.LeftShift)) moveInput.y -= 1f;

        // TODO: configure a custom Joycon axis in the Input Manager:
        //moveInput.y += Input.GetAxisRaw("UpDown");
    }

    void FixedUpdate()
    {
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        // Apply velocity to all 3 axes allowing Up/Down movement
        rb.linearVelocity = moveVelocity;
    }
}
