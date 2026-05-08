
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateObstacles : MonoBehaviour
{
    private List<GameObject> obstacleRows = new List<GameObject>();

    [SerializeField] private GameObject obstacleRockPrefab;
    
    [SerializeField] private RacingTrackGenerator racingTrackGenerator;



    private void PlaceObstacle(Transform obstaclePosition)
    {
        GameObject newObstacle = Instantiate(obstacleRockPrefab, obstaclePosition.position, obstacleRockPrefab.transform.rotation);
        newObstacle.transform.SetParent(obstaclePosition);
    }

    public void GetAllObstacleSegments(GameObject trackSegment)
    {
        obstacleRows = trackSegment
        .GetComponentsInChildren<Transform>()
        .Where(t => t.CompareTag("ObstacleRow"))
        .Select(t => t.gameObject)
        .ToList();

        

        foreach (var obstacleRow in obstacleRows)
        {   
            List<Transform> obstaclePositions = obstacleRow
            .GetComponentsInChildren<Transform>()
            .Where(t =>
                t != obstacleRow.transform &&
                t.CompareTag("ObstacleNode") &&
                t.childCount == 0
            )
            .ToList();

        int randomObstacleAmount = Random.Range(1, 3);

            for (int i = 0; i < randomObstacleAmount; i++)
            {
                if (obstaclePositions.Count == 0)
                break;

            int randomIndex = Random.Range(0, obstaclePositions.Count);
            Transform randomObstaclePosition = obstaclePositions[randomIndex];

            PlaceObstacle(randomObstaclePosition);

            obstaclePositions.RemoveAt(randomIndex);
            }
        }
    }
}
