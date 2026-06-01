using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpaceInvadersObstacleManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> obstacles;
    [SerializeField] private List<GameObject> obstacleEndLocation;
    [SerializeField] private float minCooldown = 10f;
    [SerializeField] private float maxCooldown = 20f;
    [SerializeField] private float returnCooldown = 10f;
    [SerializeField] private float moveSpeed = 5f;

    private readonly List<int> obstacleIndicesToMove = new();
    private readonly List<Vector3> obstacleStartPositions = new();
    private readonly Dictionary<int, Coroutine> activeMoves = new();

    private void Start()
    {
        obstacleStartPositions.Clear();
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacleStartPositions.Add(obstacles[i] != null ? obstacles[i].transform.position : Vector3.zero);
        }

        StartCoroutine(MoveObstacles());
    }

    private IEnumerator MoveObstacles()
    {
        while (true)
        {
            int randomCooldown = Random.Range((int)minCooldown, (int)maxCooldown);
            yield return new WaitForSeconds(randomCooldown);

            int pairCount = Mathf.Min(obstacles.Count, obstacleEndLocation.Count);
            if (pairCount == 0)
            {
                continue;
            }

            obstacleIndicesToMove.Clear();

            int obstacleAmount = Random.Range(0, pairCount + 1);
            List<int> availableIndices = new List<int>(pairCount);
            for (int i = 0; i < pairCount; i++)
            {
                availableIndices.Add(i);
            }

            for (int i = 0; i < obstacleAmount; i++)
            {
                int randomIndex = Random.Range(0, availableIndices.Count);
                obstacleIndicesToMove.Add(availableIndices[randomIndex]);
                availableIndices.RemoveAt(randomIndex);
            }

            foreach (int index in obstacleIndicesToMove)
            {
                if (index < 0 || index >= obstacles.Count || index >= obstacleEndLocation.Count)
                {
                    continue;
                }

                GameObject obstacle = obstacles[index];
                GameObject endLocation = obstacleEndLocation[index];
                if (obstacle == null || endLocation == null)
                {
                    continue;
                }

                MoveObstacle(index, obstacle, endLocation.transform.position);
            }
        }
    }

    private void MoveObstacle(int index, GameObject obstacle, Vector3 targetPosition)
    {
        if (index >= obstacleStartPositions.Count)
        {
            return;
        }

        if (activeMoves.TryGetValue(index, out Coroutine running))
        {
            StopCoroutine(running);
        }

        Vector3 startPosition = obstacleStartPositions[index];
        Coroutine routine = StartCoroutine(MoveObstacleRoutine(index, obstacle.transform, startPosition, targetPosition));
        activeMoves[index] = routine;
    }

    private IEnumerator MoveObstacleRoutine(int index, Transform obstacleTransform, Vector3 startPosition, Vector3 targetPosition)
    {
        while (Vector3.Distance(obstacleTransform.position, targetPosition) > 0.01f)
        {
            obstacleTransform.position = Vector3.MoveTowards(
                obstacleTransform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        obstacleTransform.position = targetPosition;

        yield return new WaitForSeconds(returnCooldown);

        while (Vector3.Distance(obstacleTransform.position, startPosition) > 0.01f)
        {
            obstacleTransform.position = Vector3.MoveTowards(
                obstacleTransform.position,
                startPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        obstacleTransform.position = startPosition;
        activeMoves.Remove(index);
    }
}
