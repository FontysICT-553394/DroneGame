using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Orbit Target")]
    public Transform orbitCenter;

    [Header("Orbit Settings")]
    public float orbitRadius = 2.0f;

    [Header("Smooth Horizontal Orbit")]
    public float maxAngularSpeed = 35f;
    public float horizontalSmoothTime = 0.55f;

    [Header("Smooth Height Movement")]
    public float heightChangeSpeed = 0.65f;
    public float heightSmoothTime = 0.4f;
    public float minHeight = -0.9f;
    public float maxHeight = 0.9f;

    [Header("Input Delay")]
    public float inputDelay = 0.08f;

    [Header("Rotation Lock")]
    public bool lockRotation = true;

    private Vector2 currentInput;
    private Vector2 delayedInput;
    private float delayTimer;

    private float currentAngle = 0f;
    private float currentAngularSpeed = 0f;
    private float angularSpeedVelocity = 0f;

    private float targetHeight = 0f;
    private float currentHeight = 0f;
    private float heightVelocity = 0f;

    private Vector3 previousPosition;
    private float currentSpeed;

    private Quaternion fixedRotation = Quaternion.identity;

    public float CurrentSpeed
    {
        get { return currentSpeed; }
    }

    private void Start()
    {
        if (orbitCenter == null)
        {
            GameObject centerObject = new GameObject("OrbitCenter");
            centerObject.transform.position = Vector3.zero;
            orbitCenter = centerObject.transform;
        }

        currentHeight = transform.position.y;
        targetHeight = currentHeight;
        previousPosition = transform.position;

        fixedRotation = Quaternion.identity;

        UpdateDronePosition();
        LockDroneRotation();
    }

    private void Update()
    {
        previousPosition = transform.position;

        ReadInput();
        ApplyInputDelay();
        ApplyHorizontalOrbit();
        ApplyHeightMovement();
        UpdateDronePosition();
        UpdateCurrentSpeed();
        LockDroneRotation();
    }

    private void LateUpdate()
    {
        LockDroneRotation();
    }

    private void ReadInput()
    {
        currentInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (currentInput.magnitude > 1f)
        {
            currentInput.Normalize();
        }
    }

    private void ApplyInputDelay()
    {
        delayTimer += Time.deltaTime;

        if (delayTimer >= inputDelay)
        {
            delayedInput = currentInput;
            delayTimer = 0f;
        }
    }

    private void ApplyHorizontalOrbit()
    {
        float targetAngularSpeed = delayedInput.x * maxAngularSpeed;

        currentAngularSpeed = Mathf.SmoothDamp(
            currentAngularSpeed,
            targetAngularSpeed,
            ref angularSpeedVelocity,
            horizontalSmoothTime
        );

        currentAngle += currentAngularSpeed * Time.deltaTime;
    }

    private void ApplyHeightMovement()
    {
        targetHeight += delayedInput.y * heightChangeSpeed * Time.deltaTime;
        targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);

        currentHeight = Mathf.SmoothDamp(
            currentHeight,
            targetHeight,
            ref heightVelocity,
            heightSmoothTime
        );

        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
    }

    private void UpdateDronePosition()
    {
        float angleInRadians = currentAngle * Mathf.Deg2Rad;

        float x = Mathf.Sin(angleInRadians) * orbitRadius;
        float z = Mathf.Cos(angleInRadians) * orbitRadius;

        Vector3 center = orbitCenter.position;

        transform.position = new Vector3(
            center.x + x,
            center.y + currentHeight,
            center.z + z
        );
    }

    private void UpdateCurrentSpeed()
    {
        currentSpeed = Vector3.Distance(
            transform.position,
            previousPosition
        ) / Mathf.Max(Time.deltaTime, 0.0001f);
    }

    private void LockDroneRotation()
    {
        if (!lockRotation)
        {
            return;
        }

        transform.rotation = fixedRotation;
    }
}