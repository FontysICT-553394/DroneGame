using UnityEngine;

public class DroneSelector : MonoBehaviour
{
    [Header("References")]
    public MemoryGameManager memoryGameManager;
    public GridManager gridManager;
    public DroneController droneController;

    [Header("Normal Face Selection")]
    public float selectDistance = 1.15f;
    public float topFaceSelectDistance = 1.7f;

    [Header("Manual Selection")]
    public KeyCode selectKey = KeyCode.Space;
    public bool allowSpaceSelection = true;

    [Header("Game Over Restart Selection")]
    public float restartDistance = 1.4f;

    [Header("Start Selection Safety")]
    public float requiredMoveDistanceBeforeSelect = 0.25f;

    private GridCell currentHoverCell;
    private GridCell currentRestartCell;

    private bool wasPlayerTurn = false;

    // Alleen bij de eerste spelerbeurt nodig.
    private bool hasUnlockedSelectionOnce = false;
    private bool hasMovedBeforeFirstSelection = false;
    private Vector3 firstPlayerTurnStartPosition;

    private void Awake()
    {
        if (droneController == null)
        {
            droneController = GetComponent<DroneController>();
        }
    }

    private void Update()
    {
        if (memoryGameManager.IsGameOver())
        {
            wasPlayerTurn = false;
            ClearCurrentHover();
            HandleRestartHover();
            HandleRestartSpaceSelection();
            return;
        }

        ClearRestartHover();

        if (memoryGameManager.IsPlayerTurn())
        {
            if (!wasPlayerTurn)
            {
                BeginPlayerTurn();
            }

            if (!hasUnlockedSelectionOnce)
            {
                CheckIfDroneMovedEnoughAtStart();

                if (!hasMovedBeforeFirstSelection)
                {
                    ClearCurrentHover();
                    return;
                }

                hasUnlockedSelectionOnce = true;
                Debug.Log("Eerste beweging gedaan. Selecteren met spatie is nu actief.");
            }

            HandleFaceHover();
            HandleSpaceSelection();
        }
        else
        {
            wasPlayerTurn = false;
            ClearCurrentHover();
        }
    }

    private void BeginPlayerTurn()
    {
        wasPlayerTurn = true;
        ClearCurrentHover();

        if (!hasUnlockedSelectionOnce)
        {
            hasMovedBeforeFirstSelection = false;
            firstPlayerTurnStartPosition = transform.position;

            Debug.Log("Eerste spelerbeurt gestart. Beweeg eerst voordat je kunt selecteren.");
        }
    }

    private void CheckIfDroneMovedEnoughAtStart()
    {
        float movedDistance = Vector3.Distance(transform.position, firstPlayerTurnStartPosition);

        if (movedDistance >= requiredMoveDistanceBeforeSelect)
        {
            hasMovedBeforeFirstSelection = true;
        }
    }

    private void HandleFaceHover()
    {
        GridCell closestFace = GetClosestFace();

        if (closestFace == null)
        {
            ClearCurrentHover();
            return;
        }

        float distance = closestFace.GetDistanceToPoint(transform.position);
        float allowedDistance = GetAllowedSelectDistance(closestFace);

        if (distance > allowedDistance)
        {
            ClearCurrentHover();
            return;
        }

        if (currentHoverCell != closestFace)
        {
            ClearCurrentHover();

            currentHoverCell = closestFace;
            currentHoverCell.SetHover();

            Debug.Log("Hover vlak: " + currentHoverCell.faceName);
        }
    }

    private void HandleSpaceSelection()
    {
        if (!allowSpaceSelection)
        {
            return;
        }

        if (!Input.GetKeyDown(selectKey))
        {
            return;
        }

        if (currentHoverCell == null)
        {
            Debug.Log("Geen vlak om te selecteren.");
            return;
        }

        Debug.Log("Geselecteerd met spatie: " + currentHoverCell.faceName);
        memoryGameManager.SelectCell(currentHoverCell.index);
    }

    private void HandleRestartHover()
    {
        int restartCellIndex = memoryGameManager.GetRestartCellIndex();
        GridCell restartFace = GetCellByIndex(restartCellIndex);

        if (restartFace == null)
        {
            Debug.LogWarning("Restart vlak niet gevonden. Controleer Restart Cell Index.");
            return;
        }

        currentRestartCell = restartFace;

        float distance = restartFace.GetDistanceToPoint(transform.position);

        if (distance <= restartDistance)
        {
            memoryGameManager.SetRestartCellHover();
        }
        else
        {
            memoryGameManager.SetRestartCellNormal();
        }
    }

    private void HandleRestartSpaceSelection()
    {
        if (!allowSpaceSelection)
        {
            return;
        }

        if (!Input.GetKeyDown(selectKey))
        {
            return;
        }

        if (currentRestartCell == null)
        {
            Debug.Log("Geen restart vlak gevonden.");
            return;
        }

        float distance = currentRestartCell.GetDistanceToPoint(transform.position);

        if (distance > restartDistance)
        {
            Debug.Log("Je bent niet dicht genoeg bij het restart-vlak.");
            return;
        }

        Debug.Log("Restart met spatie via vlak: " + currentRestartCell.faceName);
        memoryGameManager.RestartGame();
    }

    private GridCell GetClosestFace()
    {
        GridCell closestFace = null;
        float closestDistance = float.MaxValue;

        foreach (GridCell cell in gridManager.cells)
        {
            float distance = cell.GetDistanceToPoint(transform.position);

            if (cell.faceName == "Boven")
            {
                distance *= 0.75f;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestFace = cell;
            }
        }

        return closestFace;
    }

    private float GetAllowedSelectDistance(GridCell cell)
    {
        if (cell.faceName == "Boven")
        {
            return topFaceSelectDistance;
        }

        return selectDistance;
    }

    private GridCell GetCellByIndex(int index)
    {
        foreach (GridCell cell in gridManager.cells)
        {
            if (cell.index == index)
            {
                return cell;
            }
        }

        return null;
    }

    private void ClearCurrentHover()
    {
        if (currentHoverCell != null && currentHoverCell.IsTemporaryState())
        {
            currentHoverCell.SetNormal();
        }

        currentHoverCell = null;
    }

    private void ClearRestartHover()
    {
        currentRestartCell = null;
    }
}