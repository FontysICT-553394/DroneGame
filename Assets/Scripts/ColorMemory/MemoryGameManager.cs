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
    public CubeWinCinematic cubeWinCinematic;
    public TMP_Text scoreText;
    public TMP_Text roundText;
    public GameObject gameOverText;
    public GameObject scoreboardObject;

    [Header("Sequence Settings")]
    public float showTime = 1.2f;
    public float pauseTime = 0.4f;
    public float nextRoundDelay = 0.25f;
    public float correctFlashTime = 0.25f;

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
            scoreboardObject.SetActive(false);
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
        AddRandomFaceToSequence();
        UpdateUI();

        yield return new WaitForSeconds(0.8f);

        yield return StartCoroutine(ShowSequence());

        ResetAllCells();

        currentState = GameState.PlayerTurn;
    }

    private void AddRandomFaceToSequence()
    {
        if (gridManager.cells.Count == 0)
        {
            Debug.LogWarning("Geen vlakken gevonden in GridManager.");
            return;
        }

        List<GridCell> possibleFaces = new List<GridCell>();

        foreach (GridCell cell in gridManager.cells)
        {
            possibleFaces.Add(cell);
        }

        if (sequence.Count > 0 && possibleFaces.Count > 1)
        {
            int lastIndex = sequence[sequence.Count - 1];

            for (int i = possibleFaces.Count - 1; i >= 0; i--)
            {
                if (possibleFaces[i].index == lastIndex)
                {
                    possibleFaces.RemoveAt(i);
                }
            }
        }

        GridCell randomFace = possibleFaces[Random.Range(0, possibleFaces.Count)];
        sequence.Add(randomFace.index);

        Debug.Log("Nieuw vlak toegevoegd aan sequence: " + randomFace.faceName);
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

            Debug.Log("Sequence vlak: " + cell.faceName);

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

        if (playerIndex < 0 || playerIndex >= sequence.Count)
        {
            return;
        }

        int correctIndex = sequence[playerIndex];

        if (selectedIndex == correctIndex)
        {
            StartCoroutine(HandleCorrectSelectionRoutine(selectedIndex));
        }
        else
        {
            StartCoroutine(HandleWrongSelectionRoutine(selectedIndex));
        }
    }

    private IEnumerator HandleCorrectSelectionRoutine(int selectedIndex)
    {
        currentState = GameState.ShowingSequence;

        GridCell selectedCell = GetCellByIndex(selectedIndex);

        if (selectedCell != null)
        {
            selectedCell.SetCorrectFlash();
            Debug.Log("Correct gekozen: " + selectedCell.faceName);
        }

        yield return new WaitForSeconds(correctFlashTime);

        if (selectedCell != null)
        {
            selectedCell.SetNormal();
        }

        playerIndex++;

        if (playerIndex >= sequence.Count)
        {
            score++;
            UpdateUI();

            if (cubeWinCinematic != null)
            {
                yield return StartCoroutine(cubeWinCinematic.PlayCinematic(round, score));
            }
            else
            {
                yield return new WaitForSeconds(nextRoundDelay);
            }

            yield return new WaitForSeconds(nextRoundDelay);

            yield return StartCoroutine(StartNewRound());
        }
        else
        {
            currentState = GameState.PlayerTurn;
        }
    }

    private IEnumerator HandleWrongSelectionRoutine(int selectedIndex)
    {
        currentState = GameState.GameOver;

        GridCell selectedCell = GetCellByIndex(selectedIndex);

        if (selectedCell != null)
        {
            selectedCell.SetWrong();
            Debug.Log("Fout gekozen: " + selectedCell.faceName);
        }

        yield return new WaitForSeconds(0.5f);

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
            scoreboardObject.SetActive(true);

            TMP_Text gameOverLabel = gameOverText.GetComponent<TMP_Text>();

            if (gameOverLabel != null)
            {
                gameOverLabel.text = "Game Over!\nGa naar de voorzijde en druk op spatie om opnieuw te starten";
            }
        }

        ShowRestartCell();
    }

    private void ShowRestartCell()
    {
        GridCell restartCell = GetCellByIndex(restartCellIndex);

        if (restartCell != null)
        {
            restartCell.SetRestart();
            Debug.Log("Restart vlak: " + restartCell.faceName);
        }
        else
        {
            Debug.LogWarning("Restart cell bestaat niet. Controleer Restart Cell Index.");
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