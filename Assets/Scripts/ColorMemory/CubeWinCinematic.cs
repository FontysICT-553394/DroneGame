using System.Collections;
using TMPro;
using UnityEngine;

public enum WinCinematicType
{
    Random,
    Explosion,
    Spin,
    Wave,
    Pulse,
    ChaosExplosion,
    Slam,
    Flip
}

public class CubeWinCinematic : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public Camera mainCamera;
    public TMP_Text cinematicText;
    public Behaviour orbitCameraController;

    [Header("Cinematic Mode")]
    public WinCinematicType cinematicType = WinCinematicType.Random;

    [Header("Fixed Cinematic Camera")]
    public bool useFixedCinematicCamera = true;
    public Vector3 cinematicCameraOffset = new Vector3(0f, 2.7f, -5.2f);
    public float cinematicLookHeight = 0.45f;
    public float cameraMoveToCinematicTime = 0.35f;
    public float cameraReturnTime = 0.35f;

    [Header("Neon Confetti")]
    public bool useNeonConfetti = true;
    public int confettiBurstCount = 140;
    public float confettiSpawnHeight = 2.2f;
    public float confettiSpawnRadius = 1.6f;
    public float confettiLifetimeMin = 1.2f;
    public float confettiLifetimeMax = 2.1f;
    public float confettiSpeedMin = 1.8f;
    public float confettiSpeedMax = 3.8f;
    public float confettiSizeMin = 0.06f;
    public float confettiSizeMax = 0.14f;
    public float confettiGravity = 0.35f;
    public Color confettiColorA = new Color(0f, 1f, 1f, 1f);
    public Color confettiColorB = new Color(1f, 0f, 1f, 1f);
    public Color confettiColorC = new Color(1f, 0.9f, 0f, 1f);

    [Header("Text")]
    public string[] messages =
    {
        "PERFECT!",
        "RONDE GEHAALD!",
        "NICE!",
        "COMBO!",
        "DRONE MASTER!",
        "INSANE!",
        "CLEAN!",
        "NEXT LEVEL!"
    };

    public float textStartScale = 0.2f;
    public float textPeakScale = 1.45f;
    public float textDuration = 1.25f;

    [Header("Camera")]
    public bool useCameraZoom = true;
    public float zoomInFov = 50f;
    public float normalFov = 68f;
    public float zoomDuration = 0.35f;

    [Header("Explosion")]
    public float explosionDistance = 0.85f;
    public float explodeDuration = 0.55f;
    public float returnDuration = 0.7f;

    [Header("Spin")]
    public float spinDegrees = 360f;
    public float spinDuration = 1.05f;

    [Header("Wave")]
    public float waveDistance = 0.55f;
    public float waveStepDelay = 0.055f;
    public float waveMoveTime = 0.15f;

    [Header("Pulse")]
    public float pulseScale = 1.25f;
    public float pulseDuration = 0.55f;

    [Header("Chaos Explosion")]
    public float chaosDistance = 1.15f;
    public float chaosExplodeDuration = 0.45f;
    public float chaosReturnDuration = 0.85f;
    public float chaosRotationDegrees = 160f;

    [Header("Slam")]
    public float slamDistance = 0.9f;
    public float slamOutDuration = 0.22f;
    public float slamBackDuration = 0.2f;
    public float slamOvershoot = 0.12f;

    [Header("Flip")]
    public float flipDegrees = 360f;
    public float flipDuration = 0.8f;

    private Vector3[] originalPositions;
    private Quaternion[] originalRotations;
    private Vector3[] originalScales;

    private float originalFov;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private Vector3 originalTextScale;

    private ParticleSystem confettiSystem;
    private ParticleSystemRenderer confettiRenderer;
    private GameObject confettiObject;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            originalFov = mainCamera.fieldOfView;
            normalFov = originalFov;

            if (orbitCameraController == null)
            {
                orbitCameraController = mainCamera.GetComponent<OrbitCameraController>();
            }
        }

        if (cinematicText != null)
        {
            originalTextScale = cinematicText.transform.localScale;
            cinematicText.gameObject.SetActive(false);
        }

        if (useNeonConfetti)
        {
            CreateConfettiSystemIfNeeded();
        }
    }

    public IEnumerator PlayCinematic(int round, int score)
    {
        if (gridManager == null || gridManager.cells.Count == 0)
        {
            yield break;
        }

        CacheOriginalTransforms();
        ShowText(round, score);

        if (useFixedCinematicCamera && mainCamera != null)
        {
            if (orbitCameraController != null)
            {
                orbitCameraController.enabled = false;
            }

            yield return StartCoroutine(MoveCameraToCinematicPosition(cameraMoveToCinematicTime));
        }

        if (useNeonConfetti)
        {
            PlayConfettiBurst();
        }

        Coroutine textRoutine = null;

        if (cinematicText != null)
        {
            textRoutine = StartCoroutine(AnimateText());
        }

        if (useCameraZoom && mainCamera != null)
        {
            yield return StartCoroutine(ZoomCamera(zoomInFov, zoomDuration));
        }

        WinCinematicType chosenType = GetChosenCinematicType();

        if (chosenType == WinCinematicType.Explosion)
        {
            yield return StartCoroutine(ExplosionCinematic());
        }
        else if (chosenType == WinCinematicType.Spin)
        {
            yield return StartCoroutine(SpinCinematic());
        }
        else if (chosenType == WinCinematicType.Wave)
        {
            yield return StartCoroutine(WaveCinematic());
        }
        else if (chosenType == WinCinematicType.Pulse)
        {
            yield return StartCoroutine(PulseCinematic());
        }
        else if (chosenType == WinCinematicType.ChaosExplosion)
        {
            yield return StartCoroutine(ChaosExplosionCinematic());
        }
        else if (chosenType == WinCinematicType.Slam)
        {
            yield return StartCoroutine(SlamCinematic());
        }
        else if (chosenType == WinCinematicType.Flip)
        {
            yield return StartCoroutine(FlipCinematic());
        }

        if (useCameraZoom && mainCamera != null)
        {
            yield return StartCoroutine(ZoomCamera(normalFov, zoomDuration));
        }

        if (textRoutine != null)
        {
            StopCoroutine(textRoutine);
        }

        StopConfetti();
        ResetEverything();

        if (useFixedCinematicCamera && mainCamera != null)
        {
            yield return StartCoroutine(ReturnCameraToGameplayPosition(cameraReturnTime));

            if (orbitCameraController != null)
            {
                orbitCameraController.enabled = true;
            }
        }
    }

    private void CreateConfettiSystemIfNeeded()
    {
        if (confettiSystem != null)
        {
            return;
        }

        confettiObject = new GameObject("NeonConfettiSystem");
        confettiObject.transform.SetParent(transform);

        confettiSystem = confettiObject.AddComponent<ParticleSystem>();
        confettiRenderer = confettiObject.GetComponent<ParticleSystemRenderer>();

        var main = confettiSystem.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 2.5f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(confettiLifetimeMin, confettiLifetimeMax);
        main.startSpeed = new ParticleSystem.MinMaxCurve(confettiSpeedMin, confettiSpeedMax);
        main.startSize = new ParticleSystem.MinMaxCurve(confettiSizeMin, confettiSizeMax);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        main.startColor = new ParticleSystem.MinMaxGradient(confettiColorA, confettiColorB);
        main.gravityModifier = confettiGravity;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = confettiBurstCount * 3;

        var emission = confettiSystem.emission;
        emission.enabled = false;

        var shape = confettiSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = confettiSpawnRadius;
        shape.arc = 360f;

        var velocityOverLifetime = confettiSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.35f, 0.35f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-0.15f, 0.1f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.35f, 0.35f);

        var noise = confettiSystem.noise;
        noise.enabled = true;
        noise.strength = 0.35f;
        noise.frequency = 0.6f;
        noise.scrollSpeed = 0.5f;

        var rotationOverLifetime = confettiSystem.rotationOverLifetime;
        rotationOverLifetime.enabled = true;
        rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-220f * Mathf.Deg2Rad, 220f * Mathf.Deg2Rad);

        var colorOverLifetime = confettiSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(confettiColorA, 0f),
                new GradientColorKey(confettiColorB, 0.35f),
                new GradientColorKey(confettiColorC, 0.7f),
                new GradientColorKey(confettiColorA, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.75f),
                new GradientAlphaKey(0f, 1f)
            }
        );

        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        if (confettiRenderer != null)
        {
            confettiRenderer.renderMode = ParticleSystemRenderMode.Billboard;
            confettiRenderer.sortMode = ParticleSystemSortMode.Distance;

            Shader particleShader = Shader.Find("Sprites/Default");
            if (particleShader != null)
            {
                Material particleMaterial = new Material(particleShader);
                particleMaterial.color = Color.white;
                confettiRenderer.material = particleMaterial;
            }
        }

        confettiSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void PlayConfettiBurst()
    {
        CreateConfettiSystemIfNeeded();

        if (confettiSystem == null || gridManager == null)
        {
            return;
        }

        Vector3 center = gridManager.transform.position;
        confettiObject.transform.position = center + Vector3.up * confettiSpawnHeight;
        confettiObject.transform.rotation = Quaternion.Euler(180f, 0f, 0f);

        confettiSystem.Clear();
        confettiSystem.Play();
        confettiSystem.Emit(confettiBurstCount);
    }

    private void StopConfetti()
    {
        if (confettiSystem == null)
        {
            return;
        }

        confettiSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private WinCinematicType GetChosenCinematicType()
    {
        if (cinematicType != WinCinematicType.Random)
        {
            return cinematicType;
        }

        int randomIndex = Random.Range(1, 8);
        return (WinCinematicType)randomIndex;
    }

    private void CacheOriginalTransforms()
    {
        int count = gridManager.cells.Count;

        originalPositions = new Vector3[count];
        originalRotations = new Quaternion[count];
        originalScales = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            GridCell cell = gridManager.cells[i];

            originalPositions[i] = cell.transform.localPosition;
            originalRotations[i] = cell.transform.localRotation;
            originalScales[i] = cell.transform.localScale;
        }

        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
            originalFov = mainCamera.fieldOfView;
        }
    }

    private IEnumerator MoveCameraToCinematicPosition(float duration)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        Vector3 targetPosition = gridManager.transform.position + cinematicCameraOffset;
        Vector3 lookPosition = gridManager.transform.position + Vector3.up * cinematicLookHeight;
        Quaternion targetRotation = Quaternion.LookRotation(lookPosition - targetPosition, Vector3.up);

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float smoothT = SmoothStep(t);

            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, smoothT);

            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
    }

    private IEnumerator ReturnCameraToGameplayPosition(float duration)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float smoothT = SmoothStep(t);

            mainCamera.transform.position = Vector3.Lerp(startPosition, originalCameraPosition, smoothT);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, originalCameraRotation, smoothT);

            yield return null;
        }

        mainCamera.transform.position = originalCameraPosition;
        mainCamera.transform.rotation = originalCameraRotation;
    }

    private IEnumerator ExplosionCinematic()
    {
        Vector3[] explodedPositions = new Vector3[gridManager.cells.Count];

        for (int i = 0; i < gridManager.cells.Count; i++)
        {
            GridCell cell = gridManager.cells[i];
            Vector3 direction = GetFaceDirection(cell);

            explodedPositions[i] = originalPositions[i] + direction * explosionDistance;
            cell.Highlight();
        }

        yield return StartCoroutine(MoveFaces(originalPositions, explodedPositions, explodeDuration));
        yield return new WaitForSeconds(0.12f);
        yield return StartCoroutine(MoveFaces(explodedPositions, originalPositions, returnDuration));
    }

    private IEnumerator SpinCinematic()
    {
        float timer = 0f;

        SetAllFacesHighlight();

        while (timer < spinDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / spinDuration);
            float smoothT = SmoothStep(t);
            float currentRotation = Mathf.Lerp(0f, spinDegrees, smoothT);

            for (int i = 0; i < gridManager.cells.Count; i++)
            {
                GridCell cell = gridManager.cells[i];

                Quaternion rotation = Quaternion.Euler(0f, currentRotation, 0f);

                cell.transform.localPosition = rotation * originalPositions[i];
                cell.transform.localRotation = rotation * originalRotations[i];
            }

            yield return null;
        }
    }

    private IEnumerator WaveCinematic()
    {
        ResetFaceColors();

        for (int i = 0; i < gridManager.cells.Count; i++)
        {
            GridCell cell = gridManager.cells[i];

            Vector3 direction = GetFaceDirection(cell);
            Vector3 startPosition = cell.transform.localPosition;
            Vector3 outPosition = startPosition + direction * waveDistance;

            cell.Highlight();

            yield return StartCoroutine(MoveSingleFace(cell, startPosition, outPosition, waveMoveTime));
            yield return StartCoroutine(MoveSingleFace(cell, outPosition, startPosition, waveMoveTime));

            cell.SetNormal();

            yield return new WaitForSeconds(waveStepDelay);
        }
    }

    private IEnumerator PulseCinematic()
    {
        float timer = 0f;

        SetAllFacesHighlight();

        while (timer < pulseDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / pulseDuration);
            float pulse;

            if (t < 0.5f)
            {
                pulse = Mathf.Lerp(1f, pulseScale, EaseOutBack(t / 0.5f));
            }
            else
            {
                pulse = Mathf.Lerp(pulseScale, 1f, SmoothStep((t - 0.5f) / 0.5f));
            }

            for (int i = 0; i < gridManager.cells.Count; i++)
            {
                GridCell cell = gridManager.cells[i];
                cell.transform.localScale = originalScales[i] * pulse;
            }

            yield return null;
        }
    }

    private IEnumerator ChaosExplosionCinematic()
    {
        Vector3[] chaosPositions = new Vector3[gridManager.cells.Count];
        Vector3[] randomRotations = new Vector3[gridManager.cells.Count];

        for (int i = 0; i < gridManager.cells.Count; i++)
        {
            GridCell cell = gridManager.cells[i];

            Vector3 direction = GetFaceDirection(cell);
            Vector3 randomOffset = Random.insideUnitSphere * 0.25f;

            chaosPositions[i] = originalPositions[i] + direction * chaosDistance + randomOffset;

            randomRotations[i] = new Vector3(
                Random.Range(-chaosRotationDegrees, chaosRotationDegrees),
                Random.Range(-chaosRotationDegrees, chaosRotationDegrees),
                Random.Range(-chaosRotationDegrees, chaosRotationDegrees)
            );

            cell.Highlight();
        }

        float timer = 0f;

        while (timer < chaosExplodeDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / chaosExplodeDuration);
            float smoothT = EaseOutBack(t);

            for (int i = 0; i < gridManager.cells.Count; i++)
            {
                GridCell cell = gridManager.cells[i];

                cell.transform.localPosition = Vector3.Lerp(
                    originalPositions[i],
                    chaosPositions[i],
                    smoothT
                );

                cell.transform.localRotation = Quaternion.Lerp(
                    originalRotations[i],
                    Quaternion.Euler(randomRotations[i]),
                    smoothT
                );
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.12f);

        timer = 0f;

        while (timer < chaosReturnDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / chaosReturnDuration);
            float smoothT = SmoothStep(t);

            for (int i = 0; i < gridManager.cells.Count; i++)
            {
                GridCell cell = gridManager.cells[i];

                cell.transform.localPosition = Vector3.Lerp(
                    chaosPositions[i],
                    originalPositions[i],
                    smoothT
                );

                cell.transform.localRotation = Quaternion.Lerp(
                    Quaternion.Euler(randomRotations[i]),
                    originalRotations[i],
                    smoothT
                );
            }

            yield return null;
        }
    }

    private IEnumerator SlamCinematic()
    {
        Vector3[] outPositions = new Vector3[gridManager.cells.Count];
        Vector3[] overshootPositions = new Vector3[gridManager.cells.Count];

        SetAllFacesHighlight();

        for (int i = 0; i < gridManager.cells.Count; i++)
        {
            GridCell cell = gridManager.cells[i];
            Vector3 direction = GetFaceDirection(cell);

            outPositions[i] = originalPositions[i] + direction * slamDistance;
            overshootPositions[i] = originalPositions[i] - direction * slamOvershoot;
        }

        yield return StartCoroutine(MoveFaces(originalPositions, outPositions, slamOutDuration));
        yield return StartCoroutine(MoveFaces(outPositions, overshootPositions, slamBackDuration));
        yield return StartCoroutine(MoveFaces(overshootPositions, originalPositions, 0.22f));
    }

    private IEnumerator FlipCinematic()
    {
        float timer = 0f;

        SetAllFacesHighlight();

        while (timer < flipDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / flipDuration);
            float smoothT = SmoothStep(t);

            float currentFlip = Mathf.Lerp(0f, flipDegrees, smoothT);

            for (int i = 0; i < gridManager.cells.Count; i++)
            {
                GridCell cell = gridManager.cells[i];

                Vector3 direction = GetFaceDirection(cell);
                Quaternion flipRotation;

                if (Mathf.Abs(direction.y) > 0.5f)
                {
                    flipRotation = Quaternion.Euler(currentFlip, 0f, 0f);
                }
                else if (Mathf.Abs(direction.x) > 0.5f)
                {
                    flipRotation = Quaternion.Euler(0f, 0f, currentFlip);
                }
                else
                {
                    flipRotation = Quaternion.Euler(0f, currentFlip, 0f);
                }

                cell.transform.localRotation = originalRotations[i] * flipRotation;
            }

            yield return null;
        }
    }

    private IEnumerator MoveFaces(Vector3[] fromPositions, Vector3[] toPositions, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float smoothT = SmoothStep(t);

            for (int i = 0; i < gridManager.cells.Count; i++)
            {
                GridCell cell = gridManager.cells[i];

                cell.transform.localPosition = Vector3.Lerp(
                    fromPositions[i],
                    toPositions[i],
                    smoothT
                );
            }

            yield return null;
        }

        for (int i = 0; i < gridManager.cells.Count; i++)
        {
            gridManager.cells[i].transform.localPosition = toPositions[i];
        }
    }

    private IEnumerator MoveSingleFace(GridCell cell, Vector3 from, Vector3 to, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float smoothT = SmoothStep(t);

            cell.transform.localPosition = Vector3.Lerp(from, to, smoothT);

            yield return null;
        }

        cell.transform.localPosition = to;
    }

    private IEnumerator ZoomCamera(float targetFov, float duration)
    {
        if (mainCamera == null)
        {
            yield break;
        }

        float startFov = mainCamera.fieldOfView;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float smoothT = SmoothStep(t);

            mainCamera.fieldOfView = Mathf.Lerp(startFov, targetFov, smoothT);

            yield return null;
        }

        mainCamera.fieldOfView = targetFov;
    }

    private Vector3 GetFaceDirection(GridCell cell)
    {
        if (cell.faceName == "Voor")
        {
            return Vector3.forward;
        }

        if (cell.faceName == "Achter")
        {
            return Vector3.back;
        }

        if (cell.faceName == "Links")
        {
            return Vector3.left;
        }

        if (cell.faceName == "Rechts")
        {
            return Vector3.right;
        }

        if (cell.faceName == "Boven")
        {
            return Vector3.up;
        }

        return cell.transform.localPosition.normalized;
    }

    private void SetAllFacesHighlight()
    {
        foreach (GridCell cell in gridManager.cells)
        {
            if (cell != null)
            {
                cell.Highlight();
            }
        }
    }

    private void ResetFaceColors()
    {
        foreach (GridCell cell in gridManager.cells)
        {
            if (cell != null)
            {
                cell.SetNormal();
            }
        }
    }

    private void ShowText(int round, int score)
    {
        if (cinematicText == null)
        {
            return;
        }

        string message = messages[Random.Range(0, messages.Length)];

        cinematicText.text =
            "<size=90><b>" + message + "</b></size>\n" +
            "<size=42>Score " + score + "  •  Ronde " + round + "</size>";

        cinematicText.gameObject.SetActive(true);
        cinematicText.transform.localScale = originalTextScale * textStartScale;
    }

    private IEnumerator AnimateText()
    {
        float timer = 0f;

        while (timer < textDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / textDuration);
            float scale;

            if (t < 0.35f)
            {
                scale = Mathf.Lerp(textStartScale, textPeakScale, EaseOutBack(t / 0.35f));
            }
            else
            {
                scale = Mathf.Lerp(textPeakScale, 1f, SmoothStep((t - 0.35f) / 0.65f));
            }

            cinematicText.transform.localScale = originalTextScale * scale;

            yield return null;
        }

        cinematicText.transform.localScale = originalTextScale;
    }

    private void ResetEverything()
    {
        for (int i = 0; i < gridManager.cells.Count; i++)
        {
            GridCell cell = gridManager.cells[i];

            cell.transform.localPosition = originalPositions[i];
            cell.transform.localRotation = originalRotations[i];
            cell.transform.localScale = originalScales[i];

            cell.SetNormal();
        }

        if (mainCamera != null)
        {
            mainCamera.fieldOfView = originalFov;
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.transform.rotation = originalCameraRotation;
        }

        if (cinematicText != null)
        {
            cinematicText.gameObject.SetActive(false);
            cinematicText.transform.localScale = originalTextScale;
        }
    }

    private float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}