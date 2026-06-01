using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    ShowingSequence,
    PlayerTurn,
    GameOver
}

public class MemoryGameManager : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public Transform droneTransform;
    public TMP_Text scoreText;
    public TMP_Text roundText;
    public GameObject gameOverText;

    [Header("Sequence Settings")]
    public float showTime = 1f;
    public float pauseTime = 0.35f;
    public float nextRoundDelay = 0.8f;

    [Header("Score")]
    public int score = 0;

    [Header("Round")]
    public int round = 0;

    [Header("Restart")]
    public int restartCellIndex = 0;

    public GameState currentState;

    private List<int> sequence = new List<int>();
    private int playerIndex = 0;

    private void Start()
    {
        currentState = GameState.ShowingSequence;

        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        UpdateUI();
        StartCoroutine(StartNewRound());
    }

    private IEnumerator StartNewRound()
    {
        currentState = GameState.ShowingSequence;

        playerIndex = 0;
        round++;

        ResetAllCells();

        bool addedNewCell = AddRandomVisibleCellToSequence();

        if (!addedNewCell)
        {
            WinGame();
            yield break;
        }

        UpdateUI();

        yield return new WaitForSeconds(0.8f);

        yield return StartCoroutine(ShowSequence());

        ResetAllCells();

        currentState = GameState.PlayerTurn;
    }

    private bool AddRandomVisibleCellToSequence()
    {
        List<GridCell> possibleCells;

        if (droneTransform != null)
        {
            possibleCells = gridManager.GetCellsFacingPosition(droneTransform.position);
        }
        else
        {
            possibleCells = gridManager.cells;
        }

        List<GridCell> availableCells = new List<GridCell>();

        foreach (GridCell cell in possibleCells)
        {
            if (!sequence.Contains(cell.index))
            {
                availableCells.Add(cell);
            }
        }

        if (availableCells.Count == 0)
        {
            foreach (GridCell cell in gridManager.cells)
            {
                if (!sequence.Contains(cell.index))
                {
                    availableCells.Add(cell);
                }
            }
        }

        if (availableCells.Count == 0)
        {
            return false;
        }

        GridCell randomCell = availableCells[Random.Range(0, availableCells.Count)];
        sequence.Add(randomCell.index);

        return true;
    }

    private IEnumerator ShowSequence()
    {
        foreach (int index in sequence)
        {
            GridCell cell = GetCellByIndex(index);

            if (cell == null)
            {
                continue;
            }

            cell.Highlight();
            yield return new WaitForSeconds(showTime);

            cell.SetNormal();
            yield return new WaitForSeconds(pauseTime);
        }
    }

    public void SelectCell(int selectedIndex)
    {
        if (currentState != GameState.PlayerTurn)
        {
            return;
        }

        int correctIndex = sequence[playerIndex];

        if (selectedIndex == correctIndex)
        {
            HandleCorrectSelection(selectedIndex);
        }
        else
        {
            HandleWrongSelection(selectedIndex);
        }
    }

    private void HandleCorrectSelection(int selectedIndex)
    {
        GridCell selectedCell = GetCellByIndex(selectedIndex);

        if (selectedCell != null)
        {
            selectedCell.SetCorrect();
        }

        playerIndex++;

        if (playerIndex >= sequence.Count)
        {
            score++;
            UpdateUI();
            StartCoroutine(NextRound());
        }
    }

    private void HandleWrongSelection(int selectedIndex)
    {
        GridCell selectedCell = GetCellByIndex(selectedIndex);

        if (selectedCell != null)
        {
            selectedCell.SetWrong();
        }

        currentState = GameState.GameOver;

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        ShowRestartCell();

        Debug.Log("Game Over!");
    }

    private void WinGame()
    {
        currentState = GameState.GameOver;

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);

            TMP_Text gameOverLabel = gameOverText.GetComponent<TMP_Text>();

            if (gameOverLabel != null)
            {
                gameOverLabel.text = "Je hebt alle blokjes gehad!\nBlijf stil bij het blauwe blokje om opnieuw te starten";
            }
        }

        ShowRestartCell();

        Debug.Log("You Win!");
    }

    private void ShowRestartCell()
    {
        GridCell restartCell = GetCellByIndex(restartCellIndex);

        if (restartCell != null)
        {
            restartCell.SetRestart();
        }
    }

    public void SetRestartCellHover()
    {
        GridCell restartCell = GetCellByIndex(restartCellIndex);

        if (restartCell != null)
        {
            restartCell.SetRestartHover();
        }
    }

    public void SetRestartCellNormal()
    {
        GridCell restartCell = GetCellByIndex(restartCellIndex);

        if (restartCell != null)
        {
            restartCell.SetRestart();
        }
    }

    private IEnumerator NextRound()
    {
        currentState = GameState.ShowingSequence;

        yield return new WaitForSeconds(nextRoundDelay);

        yield return StartCoroutine(StartNewRound());
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsPlayerTurn()
    {
        return currentState == GameState.PlayerTurn;
    }

    public bool IsGameOver()
    {
        return currentState == GameState.GameOver;
    }

    public int GetRestartCellIndex()
    {
        return restartCellIndex;
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

    private void ResetAllCells()
    {
        foreach (GridCell cell in gridManager.cells)
        {
            cell.SetNormal();
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (roundText != null)
        {
            roundText.text = "Ronde: " + round;
        }
    }
}