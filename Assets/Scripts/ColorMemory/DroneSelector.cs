using UnityEngine;

public class DroneSelector : MonoBehaviour
{
    [Header("References")]
    public MemoryGameManager memoryGameManager;
    public GridManager gridManager;
    public DroneController droneController;

    [Header("Cell Selection")]
    public float selectDistance = 1.45f;
    public float selectHoverTime = 1.1f;
    public float maxSelectionSpeed = 0.65f;

    [Header("Restart Selection")]
    public float restartDistance = 1.45f;
    public float restartHoverTime = 1.5f;
    public float maxRestartSpeed = 0.65f;

    private GridCell currentHoverCell;
    private float cellHoverTimer = 0f;
    private float restartHoverTimer = 0f;
    private bool hasSelectedCurrentCell = false;
    private bool isHoveringRestartCell = false;

    private void Awake()
    {
        if (droneController == null)
        {
            droneController = GetComponent<DroneController>();
        }
    }

    private void Update()
    {
        if (memoryGameManager.IsPlayerTurn())
        {
            HandleCellHoverSelection();
        }
        else
        {
            ClearCurrentHover();
        }

        if (memoryGameManager.IsGameOver())
        {
            HandleRestartHoverSelection();
        }
        else
        {
            restartHoverTimer = 0f;
            isHoveringRestartCell = false;
        }
    }

    private void HandleCellHoverSelection()
    {
        GridCell closestCell = GetClosestCell();

        if (closestCell == null)
        {
            ClearCurrentHover();
            return;
        }

        float distance = Vector3.Distance(transform.position, closestCell.transform.position);
        float currentSpeed = GetDroneSpeed();

        if (distance > selectDistance)
        {
            ClearCurrentHover();
            return;
        }

        if (currentHoverCell != closestCell)
        {
            ClearCurrentHover();

            currentHoverCell = closestCell;
            currentHoverCell.SetHover();

            cellHoverTimer = 0f;
            hasSelectedCurrentCell = false;
        }

        if (currentSpeed > maxSelectionSpeed)
        {
            cellHoverTimer = 0f;
            return;
        }

        cellHoverTimer += Time.deltaTime;

        if (cellHoverTimer >= selectHoverTime && !hasSelectedCurrentCell)
        {
            hasSelectedCurrentCell = true;
            memoryGameManager.SelectCell(currentHoverCell.index);
        }
    }

    private void HandleRestartHoverSelection()
    {
        int restartCellIndex = memoryGameManager.GetRestartCellIndex();

        GridCell restartCell = GetCellByIndex(restartCellIndex);

        if (restartCell == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, restartCell.transform.position);
        float currentSpeed = GetDroneSpeed();

        if (distance <= restartDistance && currentSpeed <= maxRestartSpeed)
        {
            if (!isHoveringRestartCell)
            {
                memoryGameManager.SetRestartCellHover();
                isHoveringRestartCell = true;
            }

            restartHoverTimer += Time.deltaTime;

            if (restartHoverTimer >= restartHoverTime)
            {
                memoryGameManager.RestartGame();
            }
        }
        else
        {
            if (isHoveringRestartCell)
            {
                memoryGameManager.SetRestartCellNormal();
            }

            isHoveringRestartCell = false;
            restartHoverTimer = 0f;
        }
    }

    private GridCell GetClosestCell()
    {
        GridCell closestCell = null;
        float closestDistance = float.MaxValue;

        foreach (GridCell cell in gridManager.cells)
        {
            float distance = Vector3.Distance(transform.position, cell.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCell = cell;
            }
        }

        return closestCell;
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

    private float GetDroneSpeed()
    {
        if (droneController == null)
        {
            return 0f;
        }

        return droneController.CurrentSpeed;
    }

    private void ClearCurrentHover()
    {
        if (currentHoverCell != null && currentHoverCell.IsTemporaryState())
        {
            currentHoverCell.SetNormal();
        }

        currentHoverCell = null;
        cellHoverTimer = 0f;
        hasSelectedCurrentCell = false;
    }
}