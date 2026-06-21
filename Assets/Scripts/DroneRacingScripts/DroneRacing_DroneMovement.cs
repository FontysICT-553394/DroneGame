using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DroneMovementRacingDrone : MonoBehaviour
{
    private Vector2 movementInput;
    private Vector2 delayedInput;
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float verticalSpeed = 5f;

    [Header("Drone feel")]
    [SerializeField] private float inputLatency = 4f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float dragWhenNoInput = 3f;
    [SerializeField] private float tiltAmount = 25f;
    [SerializeField] private float tiltSpeed = 5f;

    [Header("Double Tap Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    private bool isDashing;
    private float dashTimer;
    private Vector3 dashDirection;

    [Header("Battery / Dash resources")]
    [SerializeField] private int batteryMax = 4;
    [SerializeField] private int batteryCurrent = 0;
    [SerializeField] private int dashCost = 1;
    [SerializeField] private Image batteryFillImage;

    [SerializeField] private Color batteryFullColor = new Color(0.25f, 0.8f, 0.25f, 1f);
    [SerializeField] private Color batteryMediumColor = new Color(1f, 0.65f, 0.2f, 1f);
    [SerializeField] private Color batteryLowColor = new Color(1f, 0.2f, 0.2f, 1f);

    [Header("Damage / Drunk handling")]
    [SerializeField] private int maxObstacleHits = 3;

    [Tooltip("How long the drone feels unstable after each hit.")]
    [SerializeField] private float drunkDuration = 3f;

    [Tooltip("How strong the drunk movement is. Higher = more unstable.")]
    [SerializeField] private float drunkStrengthPerHit = 0.75f;

    [Tooltip("How fast the drunk movement changes direction.")]
    [SerializeField] private float drunkInputNoiseSpeed = 12f;

    [Tooltip("Small side push on hit. Keep low to avoid crazy movement.")]
    [SerializeField] private float hitSidePush = 2.5f;

    [Tooltip("How much velocity remains after hit. Lower = impact feels heavier.")]
    [SerializeField] private float hitVelocityDamping = 0.35f;

    [Tooltip("Extra impulsive drift after hit. Higher = more obvious side pull.")]
    [SerializeField] private float drunkVelocityPushPerHit = 2.5f;

    [Tooltip("How long the hit push keeps pulling after impact.")]
    [SerializeField] private float drunkPushDuration = 0.45f;

    private Vector2 drunkPushDirection;
    private float drunkPushTimer;

    [Header("Heart UI")]
    [Tooltip("Sleep hier je 3 heart UI images in.")]
    [SerializeField] private RectTransform[] heartImages;

    [Tooltip("Hoe ver een heart naar beneden zakt als je een hit krijgt.")]
    [SerializeField] private float heartDropDistance = 80f;

    [Tooltip("Snelheid waarmee heart naar beneden beweegt.")]
    [SerializeField] private float heartDropSpeed = 12f;

    [Tooltip("Als true wordt een verloren heart ook transparanter.")]
    [SerializeField] private bool fadeLostHearts = true;

    [SerializeField] private float lostHeartAlpha = 0.35f;

    private Vector2[] heartStartPositions;

    [Header("Death / Crash Fall")]
    [SerializeField] private float crashFallGravityMultiplier = 2.5f;
    [SerializeField] private float crashForwardVelocity = 1.5f;
    [SerializeField] private float crashSideVelocity = 1.5f;
    [SerializeField] private float crashTorque = 3f;

    [Tooltip("Fallback: als de drone de vloer niet detecteert, komt death screen alsnog na deze tijd.")]
    [SerializeField] private float deathScreenFallbackDelay = 3f;

    [Tooltip("Extra delay nadat de vloer geraakt is voordat death screen opent.")]
    [SerializeField] private float deathScreenAfterFloorDelay = 0.35f;

    [Tooltip("Welke tag telt als vloer.")]
    [SerializeField] private string floorTag = "Ground";

    [SerializeField] private bool pauseTimeWhenDeathScreenOpens = true;

    [Header("Death UI")]
    [Tooltip("Sleep hier je death screen panel in. Deze wordt automatisch aangezet.")]
    [SerializeField] private GameObject deathScreenRoot;

    [Header("Events")]
    [Tooltip("Gebruik dit om tracks/spawners te pauzeren wanneer de drone doodgaat.")]
    [SerializeField] private UnityEvent onDroneDisabled;

    [Tooltip("Extra event als je death screen via een aparte manager wilt tonen.")]
    [SerializeField] private UnityEvent onDeathScreenRequested;

    private int obstacleHitsTaken;
    private float drunkTimer;
    private float drunkSeedX;
    private float drunkSeedY;

    private bool isDead;
    private bool hasHitFloorAfterDeath;
    private bool deathScreenTriggered;
    private float normalGravityY = -9.81f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        normalGravityY = Physics.gravity.y;

        drunkSeedX = Random.Range(0f, 1000f);
        drunkSeedY = Random.Range(0f, 1000f);

        CacheHeartStartPositions();
        UpdateHeartUIInstant();

        if (deathScreenRoot != null)
            deathScreenRoot.SetActive(false);

        SyncRuntimeSettings();
        UpdateBatteryUI();
    }

    private void Update()
    {
        UpdateHeartUIAnimated();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isDead)
            return;

        movementInput = context.ReadValue<Vector2>();
    }

    public void OnDashLeft(InputAction.CallbackContext context)
    {
        if (isDead)
            return;

        if (context.phase == InputActionPhase.Performed)
        {
            TryStartDash(Vector3.left);
        }
    }

    public void OnDashRight(InputAction.CallbackContext context)
    {
        if (isDead)
            return;

        if (context.phase == InputActionPhase.Performed)
        {
            TryStartDash(Vector3.right);
        }
    }

    private void TryStartDash(Vector3 localDir)
    {
        if (!DroneRacingRuntimeSettings.BoostEnabled)
            return;

        if (isDashing)
            return;

        if (batteryCurrent < dashCost)
            return;

        isDashing = true;
        dashTimer = dashDuration;
        dashDirection = localDir.normalized;

        batteryCurrent = Mathf.Max(0, batteryCurrent - dashCost);
        UpdateBatteryUI();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            HandleCrashFall();
            return;
        }

        SyncRuntimeSettings();
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

    private void HandleMovement()
    {
        float controlMultiplier = GetControlMultiplier();

        Vector2 finalInput = movementInput;

        if (DroneRacingRuntimeSettings.DamageEnabled && drunkTimer > 0f)
        {
            float hitRatio = Mathf.Clamp01((float)obstacleHitsTaken / maxObstacleHits);
            float drunkStrength = drunkStrengthPerHit * Mathf.Lerp(0.65f, 1.4f, hitRatio);

            float noiseX = Mathf.PerlinNoise(drunkSeedX, Time.time * drunkInputNoiseSpeed) * 2f - 1f;
            float noiseY = Mathf.PerlinNoise(drunkSeedY, Time.time * drunkInputNoiseSpeed) * 2f - 1f;

            Vector2 drunkOffset = new Vector2(noiseX, noiseY) * drunkStrength;
            finalInput += drunkOffset;
        }

        if (drunkPushTimer > 0f)
        {
            float pushStrength = drunkPushTimer / drunkPushDuration;
            finalInput += drunkPushDirection * pushStrength;
            drunkPushTimer -= Time.fixedDeltaTime;
        }

        finalInput = Vector2.ClampMagnitude(finalInput, 1.4f);

        delayedInput = Vector2.Lerp(
            delayedInput,
            finalInput,
            inputLatency * Time.fixedDeltaTime * controlMultiplier
        );

        Vector3 localTargetVelocity = new Vector3(
            delayedInput.x * moveSpeed,
            delayedInput.y * verticalSpeed,
            0f
        );

        Vector3 worldTargetVelocity = transform.TransformDirection(localTargetVelocity);

        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            worldTargetVelocity,
            acceleration * controlMultiplier * Time.fixedDeltaTime
        );

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
        if (dashTimer > 0f)
        {
            Vector3 dashVel = transform.TransformDirection(
                new Vector3(dashDirection.x * dashSpeed, 0f, 0f)
            );

            rb.linearVelocity = dashVel;
            dashTimer -= Time.fixedDeltaTime;
        }
        else
        {
            isDashing = false;
        }
    }

    private void HandleTilt()
    {
        float controlMultiplier = GetControlMultiplier();

        Quaternion targetRotation = Quaternion.Euler(
            0f,
            rb.rotation.eulerAngles.y,
            -delayedInput.x * tiltAmount
        );

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                tiltSpeed * controlMultiplier * Time.fixedDeltaTime
            )
        );
    }

    public void ApplyObstacleHit(Vector3 hitPoint)
    {
        if (!DroneRacingRuntimeSettings.DamageEnabled || isDead)
            return;

        maxObstacleHits = Mathf.Max(1, maxObstacleHits);

        obstacleHitsTaken = Mathf.Clamp(obstacleHitsTaken + 1, 0, maxObstacleHits);
        UpdateHeartUIInstant();

        Debug.Log($"Drone hit obstacle. Hits: {obstacleHitsTaken}/{maxObstacleHits}");

        if (obstacleHitsTaken >= maxObstacleHits)
        {
            Die(hitPoint);
            return;
        }

        drunkTimer = drunkDuration;

        Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);
        float sideDirection = localHitPoint.x >= 0f ? -1f : 1f;

        if (Mathf.Abs(localHitPoint.x) < 0.001f)
        {
            sideDirection = Random.value < 0.5f ? -1f : 1f;
        }

        float hitRatio = Mathf.Clamp01((float)obstacleHitsTaken / maxObstacleHits);

        drunkPushDirection = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-0.7f, 0.7f)
        ).normalized;

        drunkPushTimer = drunkPushDuration;

        rb.linearVelocity *= hitVelocityDamping;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(transform.right * sideDirection * hitSidePush, ForceMode.VelocityChange);

        rb.AddForce(
            transform.TransformDirection(new Vector3(
                drunkPushDirection.x,
                drunkPushDirection.y,
                0f
            )) * drunkVelocityPushPerHit * Mathf.Lerp(0.8f, 1.5f, hitRatio),
            ForceMode.VelocityChange
        );
    }

    public void AddBatteryCharge(int amount)
    {
        if (amount <= 0 || !DroneRacingRuntimeSettings.BoostEnabled || isDead)
            return;

        batteryCurrent = Mathf.Clamp(batteryCurrent + amount, 0, batteryMax);
        UpdateBatteryUI();
    }

    private void UpdateDamageTimers()
    {
        if (!DroneRacingRuntimeSettings.DamageEnabled)
        {
            drunkTimer = 0f;
            drunkPushTimer = 0f;
            return;
        }

        if (drunkTimer > 0f)
        {
            drunkTimer -= Time.fixedDeltaTime;
        }
    }

    private float GetControlMultiplier()
    {
        if (!DroneRacingRuntimeSettings.DamageEnabled)
            return 1f;

        if (obstacleHitsTaken <= 0)
            return 1f;

        float penalty = obstacleHitsTaken * 0.15f;
        return Mathf.Clamp(1f - penalty, 0.55f, 1f);
    }

    private void Die(Vector3 hitPoint)
    {
        if (isDead)
            return;

        isDead = true;
        hasHitFloorAfterDeath = false;
        deathScreenTriggered = false;

        movementInput = Vector2.zero;
        delayedInput = Vector2.zero;
        isDashing = false;
        dashTimer = 0f;
        drunkPushTimer = 0f;

        onDroneDisabled?.Invoke();

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.angularVelocity = Vector3.zero;

        Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);
        float sideDirection = localHitPoint.x >= 0f ? -1f : 1f;

        if (Mathf.Abs(localHitPoint.x) < 0.001f)
        {
            sideDirection = Random.value < 0.5f ? -1f : 1f;
        }

        Vector3 crashVelocity =
            transform.forward * crashForwardVelocity +
            transform.right * sideDirection * crashSideVelocity;

        rb.linearVelocity = crashVelocity;

        Vector3 randomTorque = new Vector3(
            Random.Range(-crashTorque, crashTorque),
            Random.Range(-crashTorque, crashTorque),
            Random.Range(-crashTorque, crashTorque)
        );

        rb.AddTorque(randomTorque, ForceMode.VelocityChange);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Drone disabled. Tracks should pause now. Drone is falling.");

        StartCoroutine(ShowDeathScreenAfterFloorOrDelay());
    }

    private void HandleCrashFall()
    {
        if (rb == null)
            return;

        Vector3 extraGravity = Vector3.down * Mathf.Abs(normalGravityY) * Mathf.Max(0f, crashFallGravityMultiplier - 1f);
        rb.AddForce(extraGravity, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead || deathScreenTriggered)
            return;

        bool hitFloorByTag = !string.IsNullOrWhiteSpace(floorTag) && collision.gameObject.CompareTag(floorTag);
        bool hitFloorByNormal = false;

        if (collision.contactCount > 0)
        {
            ContactPoint contact = collision.GetContact(0);
            hitFloorByNormal = contact.normal.y > 0.45f;
        }

        if (hitFloorByTag || hitFloorByNormal)
        {
            hasHitFloorAfterDeath = true;
            Debug.Log("Drone hit floor after death.");
        }
    }

    private IEnumerator ShowDeathScreenAfterFloorOrDelay()
    {
        float timer = 0f;

        while (!hasHitFloorAfterDeath && timer < deathScreenFallbackDelay)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (hasHitFloorAfterDeath && deathScreenAfterFloorDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(deathScreenAfterFloorDelay);
        }

        ShowDeathScreen();
    }

    private void ShowDeathScreen()
    {
        if (deathScreenTriggered)
            return;

        deathScreenTriggered = true;

        if (pauseTimeWhenDeathScreenOpens)
        {
            Time.timeScale = 0f;
        }

        if (deathScreenRoot != null)
        {
            deathScreenRoot.SetActive(true);
        }

        Debug.Log("Death screen requested.");

        onDeathScreenRequested?.Invoke();
    }

    private void CacheHeartStartPositions()
    {
        if (heartImages == null || heartImages.Length == 0)
            return;

        heartStartPositions = new Vector2[heartImages.Length];

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
                continue;

            heartStartPositions[i] = heartImages[i].anchoredPosition;
        }
    }

    private void UpdateHeartUIInstant()
    {
        if (heartImages == null || heartImages.Length == 0)
            return;

        if (heartStartPositions == null || heartStartPositions.Length != heartImages.Length)
        {
            CacheHeartStartPositions();
        }

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
                continue;

            bool heartLost = i < obstacleHitsTaken;

            Vector2 targetPosition = heartStartPositions[i];

            if (heartLost)
            {
                targetPosition += Vector2.down * heartDropDistance;
            }

            heartImages[i].anchoredPosition = targetPosition;

            if (fadeLostHearts)
            {
                Image image = heartImages[i].GetComponent<Image>();

                if (image != null)
                {
                    Color color = image.color;
                    color.a = heartLost ? lostHeartAlpha : 1f;
                    image.color = color;
                }
            }
        }
    }

    private void UpdateHeartUIAnimated()
    {
        if (heartImages == null || heartImages.Length == 0)
            return;

        if (heartStartPositions == null || heartStartPositions.Length != heartImages.Length)
        {
            CacheHeartStartPositions();
        }

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
                continue;

            bool heartLost = i < obstacleHitsTaken;

            Vector2 targetPosition = heartStartPositions[i];

            if (heartLost)
            {
                targetPosition += Vector2.down * heartDropDistance;
            }

            heartImages[i].anchoredPosition = Vector2.Lerp(
                heartImages[i].anchoredPosition,
                targetPosition,
                heartDropSpeed * Time.unscaledDeltaTime
            );

            if (fadeLostHearts)
            {
                Image image = heartImages[i].GetComponent<Image>();

                if (image != null)
                {
                    Color color = image.color;
                    color.a = Mathf.Lerp(
                        color.a,
                        heartLost ? lostHeartAlpha : 1f,
                        heartDropSpeed * Time.unscaledDeltaTime
                    );

                    image.color = color;
                }
            }
        }
    }

    private void UpdateBatteryUI()
    {
        if (batteryFillImage == null)
            return;

        batteryMax = Mathf.Max(1, batteryMax);

        if (DroneRacingRuntimeSettings.BoostEnabled)
        {
            batteryCurrent = Mathf.Clamp(batteryCurrent, 0, batteryMax);
            batteryFillImage.fillAmount = (float)batteryCurrent / batteryMax;
        }
        else
        {
            batteryFillImage.fillAmount = 0f;
            batteryFillImage.color = batteryLowColor;
            return;
        }

        if (batteryCurrent >= batteryMax)
        {
            batteryFillImage.color = batteryFullColor;
        }
        else if (batteryCurrent >= 2)
        {
            batteryFillImage.color = batteryMediumColor;
        }
        else
        {
            batteryFillImage.color = batteryLowColor;
        }
    }

    private void SyncRuntimeSettings()
    {
        moveSpeed = Mathf.Max(0f, DroneRacingRuntimeSettings.DroneSpeed);
        batteryMax = Mathf.Max(1, DroneRacingRuntimeSettings.BoostCharges);

        if (DroneRacingRuntimeSettings.BoostEnabled)
        {
            batteryCurrent = Mathf.Clamp(batteryCurrent, 0, batteryMax);
        }

        UpdateBatteryUI();
    }

    public int GetObstacleHitsTaken()
    {
        return obstacleHitsTaken;
    }

    public int GetMaxObstacleHits()
    {
        return maxObstacleHits;
    }

    public bool IsDead()
    {
        return isDead;
    }
}