using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Face Cube Settings")]
    public float faceSize = 2.4f;
    public float faceThickness = 0.08f;
    public float halfCubeSize = 1.2f;

    [Header("Generated Faces")]
    public List<GridCell> cells = new List<GridCell>();

    private void Awake()
    {
        CreateFiveFaceCube();
    }

    private void CreateFiveFaceCube()
    {
        cells.Clear();

        CreateFace(
            0,
            "Voor",
            new Vector3(0f, 0f, halfCubeSize),
            new Vector3(faceSize, faceSize, faceThickness),
            new Color(0f, 0.25f, 1f)
        );

        CreateFace(
            1,
            "Achter",
            new Vector3(0f, 0f, -halfCubeSize),
            new Vector3(faceSize, faceSize, faceThickness),
            new Color(0f, 0.9f, 0.15f)
        );

        CreateFace(
            2,
            "Links",
            new Vector3(-halfCubeSize, 0f, 0f),
            new Vector3(faceThickness, faceSize, faceSize),
            new Color(1f, 0.45f, 0f)
        );

        CreateFace(
            3,
            "Rechts",
            new Vector3(halfCubeSize, 0f, 0f),
            new Vector3(faceThickness, faceSize, faceSize),
            new Color(1f, 0f, 0f)
        );

        CreateFace(
            4,
            "Boven",
            new Vector3(0f, halfCubeSize, 0f),
            new Vector3(faceSize, faceThickness, faceSize),
            new Color(0.7f, 0f, 1f)
        );
    }

    private void CreateFace(int index, string faceName, Vector3 position, Vector3 scale, Color color)
    {
        GameObject faceObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

        faceObject.name = "Face_" + faceName;
        faceObject.transform.SetParent(transform);
        faceObject.transform.localPosition = position;
        faceObject.transform.localRotation = Quaternion.identity;
        faceObject.transform.localScale = scale;

        GridCell cell = faceObject.AddComponent<GridCell>();
        cell.index = index;
        cell.faceName = faceName;
        cell.faceRenderer = faceObject.GetComponent<Renderer>();
        cell.SetBaseColor(color);

        cells.Add(cell);
    }

    public List<GridCell> GetCellsFacingPosition(Vector3 viewerPosition)
    {
        return cells;
    }
}