using UnityEngine;

public class SpaceInvadersMovementScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] private float acceleration = 6f;
    [SerializeField] private float deceleration = 8f;

    private Vector3 moveInput;
    private Vector3 currentVelocity;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");

        moveInput.y = 0f;
        if (Input.GetKey(KeyCode.Space)) moveInput.y += 1f;
        if (Input.GetKey(KeyCode.LeftShift)) moveInput.y -= 1f;
    }

    void FixedUpdate()
    {
        Vector3 targetVelocity = moveInput.normalized * moveSpeed;
        float rate = (targetVelocity.sqrMagnitude > 0.001f) ? acceleration : deceleration;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);
        rb.linearVelocity = currentVelocity;
    }
}