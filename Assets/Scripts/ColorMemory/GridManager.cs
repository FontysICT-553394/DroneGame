using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Cube Grid Settings")]
    public GameObject cellPrefab;
    public int cubeSize = 3;
    public float spacing = 0.9f;

    [Header("Generated Cells")]
    public List<GridCell> cells = new List<GridCell>();

    private void Awake()
    {
        CreateRubikCubeGrid();
    }

    private void CreateRubikCubeGrid()
    {
        cells.Clear();

        float offset = (cubeSize - 1) * spacing / 2f;
        int index = 0;

        for (int x = 0; x < cubeSize; x++)
        {
            for (int y = 0; y < cubeSize; y++)
            {
                for (int z = 0; z < cubeSize; z++)
                {
                    bool isOuterCube =
                        x == 0 || x == cubeSize - 1 ||
                        y == 0 || y == cubeSize - 1 ||
                        z == 0 || z == cubeSize - 1;

                    if (!isOuterCube)
                    {
                        continue;
                    }

                    Vector3 position = new Vector3(
                        x * spacing - offset,
                        y * spacing - offset,
                        z * spacing - offset
                    );

                    GameObject cellObject = Instantiate(
                        cellPrefab,
                        position,
                        Quaternion.identity,
                        transform
                    );

                    cellObject.name = "CubeCell_" + index;

                    GridCell cell = cellObject.GetComponent<GridCell>();

                    if (cell == null)
                    {
                        cell = cellObject.AddComponent<GridCell>();
                    }

                    cell.index = index;
                    cell.SetBaseColor(GetRubikColor(x, y, z));

                    cells.Add(cell);
                    index++;
                }
            }
        }
    }

    private Color GetRubikColor(int x, int y, int z)
    {
        if (y == cubeSize - 1)
        {
            return Color.white;
        }

        if (y == 0)
        {
            return Color.yellow;
        }

        if (x == 0)
        {
            return new Color(1f, 0.45f, 0f);
        }

        if (x == cubeSize - 1)
        {
            return Color.red;
        }

        if (z == 0)
        {
            return Color.blue;
        }

        if (z == cubeSize - 1)
        {
            return Color.green;
        }

        return new Color(0.35f, 0.35f, 0.35f);
    }

    public List<GridCell> GetCellsFacingPosition(Vector3 viewerPosition)
    {
        List<GridCell> visibleCells = new List<GridCell>();

        Vector3 viewerDirection = (viewerPosition - transform.position).normalized;

        float absX = Mathf.Abs(viewerDirection.x);
        float absY = Mathf.Abs(viewerDirection.y);
        float absZ = Mathf.Abs(viewerDirection.z);

        Vector3 faceDirection;

        if (absY > absX && absY > absZ)
        {
            faceDirection = viewerDirection.y >= 0f ? Vector3.up : Vector3.down;
        }
        else if (absX > absZ)
        {
            faceDirection = viewerDirection.x >= 0f ? Vector3.right : Vector3.left;
        }
        else
        {
            faceDirection = viewerDirection.z >= 0f ? Vector3.forward : Vector3.back;
        }

        foreach (GridCell cell in cells)
        {
            Vector3 directionToCell = (cell.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(directionToCell, faceDirection);

            if (dot > 0.55f)
            {
                visibleCells.Add(cell);
            }
        }

        if (visibleCells.Count == 0)
        {
            visibleCells.AddRange(cells);
        }

        return visibleCells;
    }
}