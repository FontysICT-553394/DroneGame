using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DroneMovementRacingDrone : MonoBehaviour
{
    private Vector2 movementInput;
    private Vector2 delayedInput;
    private Rigidbody rb;

    [SerializeField] private float moveSpeed = 5f;      // zijwaartse snelheid (A/D)
    [SerializeField] private float verticalSpeed = 5f;  // hoogte snelheid (W/S)
    [SerializeField] private float forwardSpeed = 0f;   // constante voorwaartse snelheid (standaard uit)

    [Header("Drone feel")]
    [SerializeField] private float inputLatency = 4f;     // lager = meer vertraging
    [SerializeField] private float acceleration = 8f;     // lager = trager op snelheid
    [SerializeField] private float dragWhenNoInput = 3f;  // hoger = sneller afremmen
    [SerializeField] private float tiltAmount = 25f;      // Hoeveel de drone kantelt
    [SerializeField] private float tiltSpeed = 5f;        // Hoe snel de drone kantelt

    [Header("Double Tap Dash")]
    [SerializeField] private float dashSpeed = 20f;       // Snelheid van de dash
    [SerializeField] private float dashDuration = 0.2f;   // Duur van de dash
    private bool isDashing = false;
    private float dashTimer;
    private Vector3 dashDirection;
    
    [Header("Battery / Dash resources")]
    [SerializeField] private int batteryMax = 4;
    [SerializeField] private int batteryCurrent = 0;
    [SerializeField] private int dashCost = 1;
    [SerializeField] private float rechargeAccumulator;
    [SerializeField] private Image batteryFillImage; // sleep hier alleen de filled Image in
    [SerializeField] private Color batteryFullColor = new Color(0.25f, 0.8f, 0.25f, 1f);
    [SerializeField] private Color batteryMediumColor = new Color(1f, 0.65f, 0.2f, 1f);
    [SerializeField] private Color batteryLowColor = new Color(1f, 0.2f, 0.2f, 1f);

    [Header("Damage / hit handling")]
    [SerializeField] private int maxDamageStacks = 3;
    [SerializeField] private float damageDuration = 2.25f;
    [SerializeField] private float damageRecoveryDelay = 1.25f;
    [SerializeField] private float knockbackImpulse = 3.5f;
    [SerializeField] private float damageControlPenalty = 0.2f;
    [SerializeField] private float damageDriftAmount = 0.25f;
    [SerializeField] private float damageWobbleAmount = 0.18f;

    private int damageStacks;
    private float damageTimer;
    private float damageRecoveryTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        UpdateBatteryUI();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 newMovementInput = context.ReadValue<Vector2>();
        movementInput = newMovementInput;
    }

    // InputSystem actions: bind Q to OnDashLeft and E to OnDashRight
    public void OnDashLeft(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            TryStartDash(Vector3.left);
        }
    }

    public void OnDashRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            TryStartDash(Vector3.right);
        }
    }

    private void TryStartDash(Vector3 localDir)
    {
        if (isDashing) return;
        if (batteryCurrent < dashCost) return; // not enough battery

        isDashing = true;
        dashTimer = dashDuration;
        dashDirection = localDir.normalized;
        batteryCurrent = Mathf.Max(0, batteryCurrent - dashCost);
        UpdateBatteryUI();
    }

    private void FixedUpdate()
    {
        UpdateDamageTimers();

        if (isDashing)
        {
            Dash();
        }
        else
        {
            HandleMovement();
        }

        HandleTilt();
    }

    private void UpdateBatteryUI()
    {
        if (batteryFillImage == null)
        {
            return;
        }

        batteryMax = Mathf.Max(1, batteryMax);
        batteryCurrent = Mathf.Clamp(batteryCurrent, 0, batteryMax);

        batteryFillImage.fillAmount = (float)batteryCurrent / batteryMax;

        if (batteryCurrent >= batteryMax)
        {
            batteryFillImage.color = batteryFullColor;
        }
        else if (batteryCurrent >= 2)
        {
            batteryFillImage.color = batteryMediumColor;
        }
        else if (batteryCurrent >= 1)
        {
            batteryFillImage.color = batteryLowColor;
        }
        else
        {
            batteryFillImage.color = batteryLowColor;
        }
    }

    private void HandleMovement()
    {
        float controlMultiplier = GetControlMultiplier();

        // Input reageert niet instant, maar loopt erachteraan
        delayedInput = Vector2.Lerp(
            delayedInput,
            movementInput,
            inputLatency * Time.fixedDeltaTime * controlMultiplier
        );

        // X = zijwaarts (A/D), Y = hoogte (W/S). Geen automatische voorwaartse snelheid.
        Vector3 localTargetVelocity = new Vector3(
            delayedInput.x * moveSpeed,
            delayedInput.y * verticalSpeed,
            0f
        );

        if (damageStacks > 0)
        {
            float wobbleX = Mathf.Sin(Time.time * 13f) * damageWobbleAmount * damageStacks;
            float wobbleY = Mathf.Cos(Time.time * 11f) * damageWobbleAmount * damageStacks * 0.5f;
            localTargetVelocity.x += wobbleX + (Time.frameCount % 2 == 0 ? damageDriftAmount * damageStacks * 0.1f : -damageDriftAmount * damageStacks * 0.1f);
            localTargetVelocity.y += wobbleY;
        }

        // Maak het lokaal (ten opzichte van de drone-oriëntatie)
        Vector3 worldTargetVelocity = transform.TransformDirection(localTargetVelocity);

        // Drone accelereert naar de target velocity i.p.v. direct teleport-speed
        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            worldTargetVelocity,
            acceleration * controlMultiplier * Time.fixedDeltaTime
        );

        // Als je geen input geeft, rem alle bewegingen af naar stilstand
        if (Mathf.Abs(movementInput.x) < 0.01f && Mathf.Abs(movementInput.y) < 0.01f)
        {
            rb.linearVelocity = Vector3.Lerp(
                rb.linearVelocity,
                Vector3.zero,
                dragWhenNoInput * controlMultiplier * Time.fixedDeltaTime
            );
        }
    }

    private void Dash()
    {
        if (dashTimer > 0)
        {
            // Tijdens de dash alleen zijwaartse dash (geen automatische voorwaartse component)
            Vector3 dashVel = transform.TransformDirection(new Vector3(dashDirection.x * dashSpeed, 0f, 0f));
            rb.linearVelocity = dashVel;
            dashTimer -= Time.fixedDeltaTime;
        }
        else
        {
            isDashing = false;
            // Laat HandleMovement de controle terugnemen in FixedUpdate
        }
    }

    private void HandleTilt()
    {
        float controlMultiplier = GetControlMultiplier();

        // Kantel de drone op basis van de zijwaartse input
        Quaternion targetRotation = Quaternion.Euler(
            0,
            rb.rotation.eulerAngles.y,
            -delayedInput.x * tiltAmount
        );

        rb.rotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            tiltSpeed * controlMultiplier * Time.fixedDeltaTime
        );
    }

    public void ApplyObstacleHit(Vector3 hitPoint)
    {
        damageStacks = Mathf.Min(maxDamageStacks, damageStacks + 1);
        damageTimer = damageDuration;
        damageRecoveryTimer = damageRecoveryDelay;

        Vector3 pushDirection = transform.position - hitPoint;
        pushDirection.y = 0f;

        if (pushDirection.sqrMagnitude < 0.001f)
        {
            pushDirection = -transform.forward;
            pushDirection.y = 0f;
        }

        rb.AddForce((pushDirection.normalized + Vector3.up * 0.15f) * knockbackImpulse, ForceMode.Impulse);
    }

    public void AddBatteryCharge(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        batteryCurrent = Mathf.Clamp(batteryCurrent + amount, 0, batteryMax);
        UpdateBatteryUI();
    }

    private void UpdateDamageTimers()
    {
        if (damageStacks <= 0)
        {
            return;
        }

        if (damageTimer > 0f)
        {
            damageTimer -= Time.fixedDeltaTime;
            damageRecoveryTimer = damageRecoveryDelay;
            return;
        }

        damageRecoveryTimer -= Time.fixedDeltaTime;
        if (damageRecoveryTimer <= 0f)
        {
            damageStacks = Mathf.Max(0, damageStacks - 1);
            damageTimer = damageStacks > 0 ? damageDuration * 0.5f : 0f;
            damageRecoveryTimer = damageRecoveryDelay;
        }
    }

    private float GetControlMultiplier()
    {
        if (damageStacks <= 0)
        {
            return 1f;
        }

        return Mathf.Clamp(1f - damageStacks * damageControlPenalty, 0.35f, 1f);
    }
}