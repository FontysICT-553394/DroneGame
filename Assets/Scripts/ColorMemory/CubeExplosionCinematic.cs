using System.Collections;
using TMPro;
using UnityEngine;

public class CubeExplosionCinematic : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public Camera mainCamera;
    public TMP_Text cinematicText;

    [Header("Explosion Settings")]
    public float explosionDistance = 0.75f;
    public float explodeDuration = 0.7f;
    public float holdDuration = 0.25f;
    public float returnDuration = 0.85f;

    [Header("Camera Zoom")]
    public bool useCameraZoom = true;
    public float zoomInFov = 48f;
    public float normalFov = 60f;
    public float zoomDuration = 0.55f;

    [Header("Text Settings")]
    public string[] messages =
    {
        "PERFECT!",
        "RONDE GEHAALD!",
        "NICE!",
        "COMBO!",
        "DRONE MASTER!"
    };

    public float textStartScale = 0.25f;
    public float textPeakScale = 1.25f;
    public float textDuration = 1.25f;

    private Vector3[] originalPositions;
    private Vector3[] explodedPositions;

    private float originalFov;
    private Vector3 originalTextScale;

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
        }

        if (cinematicText != null)
        {
            originalTextScale = cinematicText.transform.localScale;
            cinematicText.gameObject.SetActive(false);
        }
    }

    public IEnumerator PlayCinematic(int round, int score)
    {
        if (gridManager == null || gridManager.cells.Count == 0)
        {
            yield break;
        }

        CacheOriginalPositions();
        ShowText(round, score);

        Coroutine textRoutine = null;

        if (cinematicText != null)
        {
            textRoutine = StartCoroutine(AnimateTextSmooth());
        }

        if (useCameraZoom && mainCamera != null)
        {
            yield return StartCoroutine(ZoomCameraSmooth(zoomInFov, zoomDuration));
        }

        yield return StartCoroutine(MoveFacesSmooth(originalPositions, explodedPositions, explodeDuration));

        yield return new WaitForSeconds(holdDuration);

        yield return StartCoroutine(MoveFacesSmooth(explodedPositions, originalPositions, returnDuration));

        if (useCameraZoom && mainCamera != null)
        {
            yield return StartCoroutine(ZoomCameraSmooth(normalFov, zoomDuration));
        }

        if (textRoutine != null)
        {
            StopCoroutine(textRoutine);
        }

        ResetFaces();
        HideText();
    }

    private void CacheOriginalPositions()
    {
        int count = gridManager.cells.Count;

        originalPositions = new Vector3[count];
        explodedPositions = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            GridCell cell = gridManager.cells[i];

            originalPositions[i] = cell.transform.localPosition;

            Vector3 direction = GetExplosionDirection(cell);
            explodedPositions[i] = originalPositions[i] + direction * explosionDistance;
        }
    }

    private Vector3 GetExplosionDirection(GridCell cell)
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

    private IEnumerator MoveFacesSmooth(Vector3[] fromPositions, Vector3[] toPositions, float duration)
    {
        float timer = 0f;

        SetAllFacesHighlight();

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float rawT = Mathf.Clamp01(timer / duration);
            float smoothT = SmoothStep(rawT);

            for (int i = 0; i < gridManager.cells.Count; i++)
            {
                GridCell cell = gridManager.cells[i];

                if (cell == null)
                {
                    continue;
                }

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
            GridCell cell = gridManager.cells[i];

            if (cell != null)
            {
                cell.transform.localPosition = toPositions[i];
            }
        }
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

    private void ResetFaces()
    {
        if (gridManager == null)
        {
            return;
        }

        for (int i = 0; i < gridManager.cells.Count; i++)
        {
            GridCell cell = gridManager.cells[i];

            if (cell == null)
            {
                continue;
            }

            if (originalPositions != null && i < originalPositions.Length)
            {
                cell.transform.localPosition = originalPositions[i];
            }

            cell.SetNormal();
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

    private IEnumerator AnimateTextSmooth()
    {
        float timer = 0f;

        while (timer < textDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / textDuration);
            float scale;

            if (t < 0.35f)
            {
                float popT = SmoothStep(t / 0.35f);
                scale = Mathf.Lerp(textStartScale, textPeakScale, popT);
            }
            else
            {
                float settleT = SmoothStep((t - 0.35f) / 0.65f);
                scale = Mathf.Lerp(textPeakScale, 1f, settleT);
            }

            cinematicText.transform.localScale = originalTextScale * scale;

            yield return null;
        }

        cinematicText.transform.localScale = originalTextScale;
    }

    private void HideText()
    {
        if (cinematicText == null)
        {
            return;
        }

        cinematicText.gameObject.SetActive(false);
        cinematicText.transform.localScale = originalTextScale;
    }

    private IEnumerator ZoomCameraSmooth(float targetFov, float duration)
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

            float rawT = Mathf.Clamp01(timer / duration);
            float smoothT = SmoothStep(rawT);

            mainCamera.fieldOfView = Mathf.Lerp(startFov, targetFov, smoothT);

            yield return null;
        }

        mainCamera.fieldOfView = targetFov;
    }

    private float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }
}