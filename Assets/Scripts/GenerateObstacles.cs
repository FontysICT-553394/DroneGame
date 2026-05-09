
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateObstacles : MonoBehaviour
{
    private List<GameObject> obstacleRows = new List<GameObject>();
    private List<GameObject> airObstacleRows = new List<GameObject>();
    [SerializeField] private List<GameObject> ObstaclePrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> AirObstaclePrefabs = new List<GameObject>();
    [SerializeField] private RacingTrackGenerator racingTrackGenerator;



    private void PlaceObstacle(Transform obstaclePosition)
    {
        GameObject randomObstaclePrefab = ObstaclePrefabs[Random.Range(0, ObstaclePrefabs.Count)];
        GameObject newObstacle = Instantiate(randomObstaclePrefab, obstaclePosition.position, randomObstaclePrefab.transform.rotation);
        newObstacle.transform.SetParent(obstaclePosition);
    }

    private void PlaceAirObstacle(Transform airObstaclePosition)
    {
        GameObject randomAirObstaclePrefab = AirObstaclePrefabs[Random.Range(0, AirObstaclePrefabs.Count)];
        GameObject newAirObstacle = Instantiate(randomAirObstaclePrefab, airObstaclePosition.position, randomAirObstaclePrefab.transform.rotation);
        newAirObstacle.transform.SetParent(airObstaclePosition);
    }

    public void GetAllObstacleSegments(GameObject trackSegment)
    {
        obstacleRows = trackSegment
        .GetComponentsInChildren<Transform>()
        .Where(t => t.CompareTag("ObstacleRow"))
        .Select(t => t.gameObject)
        .ToList();

        airObstacleRows = trackSegment
        .GetComponentsInChildren<Transform>()
        .Where(t => t.CompareTag("AirObstacleRow"))
        .Select(t => t.gameObject)
        .ToList();

        foreach (var airObstacleRow in airObstacleRows)
        {
            List<Transform> airObstaclePositions = airObstacleRow
            .GetComponentsInChildren<Transform>()
            .Where(t =>
                t != airObstacleRow.transform &&
                t.CompareTag("AirObstacleNode") &&
                t.childCount == 0
            )
            .ToList();

            int randomAirObstacleAmount = Random.Range(1, 3);

            for (int i = 0; i < randomAirObstacleAmount; i++)
            {
                if (airObstaclePositions.Count == 0)
                break;

            int randomIndex = Random.Range(0, airObstaclePositions.Count);
            Transform randomAirObstaclePosition = airObstaclePositions[randomIndex];

            PlaceAirObstacle(randomAirObstaclePosition);

            airObstaclePositions.RemoveAt(randomIndex);
            }
        }

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
