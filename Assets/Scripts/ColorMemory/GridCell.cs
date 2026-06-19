using UnityEngine;

public enum CellVisualState
{
    Normal,
    Hover,
    Highlight,
    CorrectFlash,
    Wrong,
    Restart,
    RestartHover
}

public class GridCell : MonoBehaviour
{
    public int index;
    public string faceName;

    [Header("Renderer")]
    public Renderer faceRenderer;

    private Collider faceCollider;
    private CellVisualState currentState;

    private Color baseColor = Color.gray;
    private Color wrongColor = new Color(1f, 0f, 0f);
    private Color restartColor = new Color(1f, 0f, 1f);
    private Color correctFlashColor = Color.white;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;

        if (faceRenderer == null)
        {
            faceRenderer = GetComponent<Renderer>();
        }

        faceCollider = GetComponent<Collider>();

        SetNormal();
    }

    public void SetBaseColor(Color color)
    {
        baseColor = color;
        SetNormal();
    }

    public Color GetBaseColor()
    {
        return baseColor;
    }

    public void SetNormal()
    {
        currentState = CellVisualState.Normal;
        transform.localScale = originalScale;
        SetColor(baseColor, 0.15f);
    }

    public void SetHover()
    {
        if (currentState == CellVisualState.Highlight ||
            currentState == CellVisualState.CorrectFlash ||
            currentState == CellVisualState.Wrong ||
            currentState == CellVisualState.Restart ||
            currentState == CellVisualState.RestartHover)
        {
            return;
        }

        currentState = CellVisualState.Hover;

        // Niet groter maken bij selecteren/hover.
        transform.localScale = originalScale;

        Color brighterColor = MakeBrighter(baseColor, 1.6f);
        SetColor(brighterColor, 0.75f);
    }

    public void Highlight()
    {
        currentState = CellVisualState.Highlight;

        // Alleen bij de sequence groter maken.
        transform.localScale = originalScale * 1.1f;

        Color brighterColor = MakeBrighter(baseColor, 2.2f);
        SetColor(brighterColor, 1.2f);
    }

    public void SetCorrectFlash()
    {
        currentState = CellVisualState.CorrectFlash;

        // Niet groter maken bij correcte selectie.
        transform.localScale = originalScale;

        SetColor(correctFlashColor, 1.2f);
    }

    public void SetWrong()
    {
        currentState = CellVisualState.Wrong;

        // Niet groter maken bij fout.
        transform.localScale = originalScale;

        SetColor(wrongColor, 1f);
    }

    public void SetRestart()
    {
        currentState = CellVisualState.Restart;

        // Niet groter maken bij restart.
        transform.localScale = originalScale;

        SetColor(restartColor, 1f);
    }

    public void SetRestartHover()
    {
        currentState = CellVisualState.RestartHover;

        // Niet groter maken bij restart hover.
        transform.localScale = originalScale;

        Color brighterRestart = MakeBrighter(restartColor, 1.6f);
        SetColor(brighterRestart, 1.2f);
    }

    public bool IsTemporaryState()
    {
        return currentState == CellVisualState.Hover;
    }

    public float GetDistanceToPoint(Vector3 point)
    {
        if (faceCollider == null)
        {
            return Vector3.Distance(transform.position, point);
        }

        Vector3 closestPoint = faceCollider.ClosestPoint(point);
        return Vector3.Distance(point, closestPoint);
    }

    private Color MakeBrighter(Color color, float multiplier)
    {
        return new Color(
            Mathf.Clamp01(color.r * multiplier),
            Mathf.Clamp01(color.g * multiplier),
            Mathf.Clamp01(color.b * multiplier),
            color.a
        );
    }

    private void SetColor(Color color, float emissionStrength)
    {
        if (faceRenderer == null)
        {
            return;
        }

        faceRenderer.material.color = color;

        if (faceRenderer.material.HasProperty("_BaseColor"))
        {
            faceRenderer.material.SetColor("_BaseColor", color);
        }

        if (faceRenderer.material.HasProperty("_EmissionColor"))
        {
            faceRenderer.material.EnableKeyword("_EMISSION");
            faceRenderer.material.SetColor("_EmissionColor", color * emissionStrength);
        }
    }
}