using UnityEngine;

public enum CellVisualState
{
    Normal,
    Hover,
    Highlight,
    Correct,
    Wrong,
    Restart,
    RestartHover
}

public class GridCell : MonoBehaviour
{
    public int index;

    [Header("Renderer van ColorCube")]
    public Renderer colorRenderer;

    [Header("Outline Settings")]
    public bool createOutline = true;
    public Material outlineMaterial;
    public float edgeThickness = 0.06f;
    public float edgeLength = 1.05f;

    private CellVisualState currentState;
    private Color baseColor = new Color(0.35f, 0.35f, 0.35f);

    private Color hoverColor = new Color(0.85f, 0.1f, 1f);
    private Color highlightColor = new Color(1f, 0f, 1f);
    private Color correctColor = new Color(0.1f, 1f, 0.2f);
    private Color wrongColor = new Color(1f, 0.05f, 0.05f);
    private Color restartColor = new Color(0f, 0.25f, 1f);
    private Color restartHoverColor = new Color(0f, 1f, 1f);

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;

        FindColorRenderer();

        if (createOutline)
        {
            CreateOutlineEdges();
        }

        SetNormal();
    }

    private void FindColorRenderer()
    {
        if (colorRenderer != null)
        {
            return;
        }

        Transform colorCube = transform.Find("ColorCube");

        if (colorCube != null)
        {
            colorRenderer = colorCube.GetComponent<Renderer>();
        }

        if (colorRenderer == null)
        {
            colorRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void CreateOutlineEdges()
    {
        if (transform.Find("Edge_Top_Front") != null)
        {
            return;
        }

        CreateEdge("Edge_Top_Front", new Vector3(0, 0.53f, 0.53f), new Vector3(edgeLength, edgeThickness, edgeThickness));
        CreateEdge("Edge_Top_Back", new Vector3(0, 0.53f, -0.53f), new Vector3(edgeLength, edgeThickness, edgeThickness));
        CreateEdge("Edge_Bottom_Front", new Vector3(0, -0.53f, 0.53f), new Vector3(edgeLength, edgeThickness, edgeThickness));
        CreateEdge("Edge_Bottom_Back", new Vector3(0, -0.53f, -0.53f), new Vector3(edgeLength, edgeThickness, edgeThickness));

        CreateEdge("Edge_Left_Front", new Vector3(-0.53f, 0, 0.53f), new Vector3(edgeThickness, edgeLength, edgeThickness));
        CreateEdge("Edge_Left_Back", new Vector3(-0.53f, 0, -0.53f), new Vector3(edgeThickness, edgeLength, edgeThickness));
        CreateEdge("Edge_Right_Front", new Vector3(0.53f, 0, 0.53f), new Vector3(edgeThickness, edgeLength, edgeThickness));
        CreateEdge("Edge_Right_Back", new Vector3(0.53f, 0, -0.53f), new Vector3(edgeThickness, edgeLength, edgeThickness));

        CreateEdge("Edge_Top_Left", new Vector3(-0.53f, 0.53f, 0), new Vector3(edgeThickness, edgeThickness, edgeLength));
        CreateEdge("Edge_Top_Right", new Vector3(0.53f, 0.53f, 0), new Vector3(edgeThickness, edgeThickness, edgeLength));
        CreateEdge("Edge_Bottom_Left", new Vector3(-0.53f, -0.53f, 0), new Vector3(edgeThickness, edgeThickness, edgeLength));
        CreateEdge("Edge_Bottom_Right", new Vector3(0.53f, -0.53f, 0), new Vector3(edgeThickness, edgeThickness, edgeLength));
    }

    private void CreateEdge(string edgeName, Vector3 localPosition, Vector3 localScale)
    {
        GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cube);
        edge.name = edgeName;
        edge.transform.SetParent(transform);
        edge.transform.localPosition = localPosition;
        edge.transform.localRotation = Quaternion.identity;
        edge.transform.localScale = localScale;

        Collider edgeCollider = edge.GetComponent<Collider>();

        if (edgeCollider != null)
        {
            Destroy(edgeCollider);
        }

        Renderer edgeRenderer = edge.GetComponent<Renderer>();

        if (outlineMaterial != null)
        {
            edgeRenderer.material = outlineMaterial;
        }
        else
        {
            edgeRenderer.material.color = Color.black;
        }
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
        SetColor(baseColor);
    }

    public void SetHover()
    {
        if (currentState == CellVisualState.Highlight ||
            currentState == CellVisualState.Correct ||
            currentState == CellVisualState.Wrong ||
            currentState == CellVisualState.Restart ||
            currentState == CellVisualState.RestartHover)
        {
            return;
        }

        currentState = CellVisualState.Hover;
        transform.localScale = originalScale * 1.08f;
        SetColor(hoverColor);
    }

    public void Highlight()
    {
        currentState = CellVisualState.Highlight;
        transform.localScale = originalScale * 1.18f;
        SetColor(highlightColor);
    }

    public void SetCorrect()
    {
        currentState = CellVisualState.Correct;
        transform.localScale = originalScale * 1.12f;
        SetColor(correctColor);
    }

    public void SetWrong()
    {
        currentState = CellVisualState.Wrong;
        transform.localScale = originalScale * 1.12f;
        SetColor(wrongColor);
    }

    public void SetRestart()
    {
        currentState = CellVisualState.Restart;
        transform.localScale = originalScale * 1.12f;
        SetColor(restartColor);
    }

    public void SetRestartHover()
    {
        currentState = CellVisualState.RestartHover;
        transform.localScale = originalScale * 1.18f;
        SetColor(restartHoverColor);
    }

    public bool IsTemporaryState()
    {
        return currentState == CellVisualState.Hover;
    }

    private void SetColor(Color color)
    {
        if (colorRenderer != null)
        {
            colorRenderer.material.color = color;

            if (colorRenderer.material.HasProperty("_EmissionColor"))
            {
                colorRenderer.material.EnableKeyword("_EMISSION");
                colorRenderer.material.SetColor("_EmissionColor", color * 0.35f);
            }
        }
    }
}