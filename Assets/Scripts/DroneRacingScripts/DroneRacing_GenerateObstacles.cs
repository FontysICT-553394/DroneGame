using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum ObstacleStatus
{
    Occupied,
    Empty
}

public class GenerateObstacles : MonoBehaviour
{
    private List<GameObject> obstacleRows = new List<GameObject>();
    private List<GameObject> airObstacleRows = new List<GameObject>();

    private readonly Dictionary<GameObject, List<Transform>> obstacleNodesByRow = new Dictionary<GameObject, List<Transform>>();
    private readonly Dictionary<GameObject, List<Transform>> airObstacleNodesByRow = new Dictionary<GameObject, List<Transform>>();
    private readonly Dictionary<Transform, ObstacleStatus> nodeStatus = new Dictionary<Transform, ObstacleStatus>();

    [SerializeField] private float batteryHeightOffset = 5f;
    [SerializeField] private RacingTrackGenerator racingTrackGenerator;

    [Header("Prefabs")]
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private List<GameObject> ObstaclePrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> AirObstaclePrefabs = new List<GameObject>();
    [SerializeField] private GameObject bannerPrefab;

    private static List<Transform> GetTaggedNodesFromRow(GameObject row, string nodeTag)
    {
        if (row == null)
            return new List<Transform>();

        Transform rowTransform = row.transform;

        return rowTransform
            .GetComponentsInChildren<Transform>()
            .Where(t => t != rowTransform && t.CompareTag(nodeTag))
            .ToList();
    }

    private void PlaceObstacle(Transform obstaclePosition)
    {
        GameObject randomObstaclePrefab = ObstaclePrefabs[Random.Range(0, ObstaclePrefabs.Count)];

        GameObject newObstacle = Instantiate(
            randomObstaclePrefab,
            obstaclePosition.position,
            randomObstaclePrefab.transform.rotation
        );

        newObstacle.transform.SetParent(obstaclePosition);

        AlignBottomToNode(newObstacle, obstaclePosition.position.y);
    }

    private void PlaceAirObstacle(Transform airObstaclePosition)
    {
        GameObject randomAirObstaclePrefab = AirObstaclePrefabs[Random.Range(0, AirObstaclePrefabs.Count)];

        GameObject newAirObstacle = Instantiate(
            randomAirObstaclePrefab,
            airObstaclePosition.position, 
            randomAirObstaclePrefab.transform.rotation
        );

        newAirObstacle.transform.SetParent(airObstaclePosition);

        AlignBottomToNode(newAirObstacle, airObstaclePosition.position.y + 2f);
    }

    private void PlaceBannerOnRow(GameObject airObstacleRow)
    {
        if (bannerPrefab == null) return;

        List<Transform> bannerNodes = GetTaggedNodesFromRow(airObstacleRow, "BannerNode");

        if (bannerNodes.Count == 0) return;

        foreach (Transform bannerNode in bannerNodes)
        {
            GameObject newBanner = Instantiate(
                bannerPrefab,
                bannerNode.position,
                bannerPrefab.transform.rotation
            );

            newBanner.transform.SetParent(bannerNode);
        }
    }

    private void PlaceBatteryOnEmptyNodes()
    {
        List<Transform> emptyNodes = nodeStatus
            .Where(kvp => kvp.Key != null && kvp.Value == ObstacleStatus.Empty)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (Transform node in emptyNodes)
        {
            GameObject newBattery = Instantiate(
                batteryPrefab,
                node.position + new Vector3(0f, batteryHeightOffset, 0f),
                batteryPrefab.transform.rotation
            );

            newBattery.transform.SetParent(node);

            nodeStatus[node] = ObstacleStatus.Occupied;
        }
    }

    private void AlignBottomToNode(GameObject obstacle, float nodeY)
    {
        Renderer[] renderers = obstacle.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        float bottomY = bounds.min.y;
        float offsetY = nodeY - bottomY;

        obstacle.transform.position += new Vector3(0f, offsetY, 0f);
    }

    public HashSet<Transform> ChooseRandomNodes(List<Transform> nodes, int amount)
    {
        HashSet<Transform> chosenNodes = new HashSet<Transform>();

        List<Transform> availableNodes = nodes
            .Where(t =>
                t != null &&
                nodeStatus.ContainsKey(t) &&
                nodeStatus[t] == ObstacleStatus.Empty
            )
            .ToList();

        for (int i = 0; i < amount; i++)
        {
            if (availableNodes.Count == 0)
                break;

            int randomIndex = Random.Range(0, availableNodes.Count);
            Transform randomNode = availableNodes[randomIndex];

            chosenNodes.Add(randomNode);
            nodeStatus[randomNode] = ObstacleStatus.Occupied;

            availableNodes.RemoveAt(randomIndex);
        }

        return chosenNodes;
    }


    public void GenerateObstaclesForTrack(GameObject trackSegment)
    {
        GetAllObstacleSegments(trackSegment);

        foreach (var airObstacleRow in airObstacleRows)
        {
            List<Transform> airNodes = airObstacleNodesByRow[airObstacleRow];

            int maxAirAmount = Mathf.Min(airNodes.Count - 1, 2);
            int airAmount = Random.Range(0, maxAirAmount + 1);

            HashSet<Transform> chosenAirNodes = ChooseRandomNodes(airNodes, airAmount);

            if (chosenAirNodes.Count == 0)
            {
                PlaceBannerOnRow(airObstacleRow);

                foreach (Transform airNode in airNodes)
                {
                    nodeStatus[airNode] = ObstacleStatus.Occupied;
                }
            }

            foreach (Transform chosenAirNode in chosenAirNodes)
            {
                PlaceAirObstacle(chosenAirNode);
            }
        }

        foreach (var obstacleRow in obstacleRows)
        {
            List<Transform> groundNodes = obstacleNodesByRow[obstacleRow];

            int maxGroundAmount = Mathf.Min(groundNodes.Count - 1, 2);
            int groundAmount = Random.Range(1, maxGroundAmount + 1);

            HashSet<Transform> chosenNodes = ChooseRandomNodes(groundNodes, groundAmount);

            foreach (Transform chosenNode in chosenNodes)
            {
                PlaceObstacle(chosenNode);
            }
        }
        
        PlaceBatteryOnEmptyNodes();
    }

    public void GetAllObstacleSegments(GameObject trackSegment)
    {
        obstacleNodesByRow.Clear();
        airObstacleNodesByRow.Clear();
        nodeStatus.Clear();

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

        foreach (GameObject obstacleRow in obstacleRows)
        {
            obstacleNodesByRow[obstacleRow] = GetTaggedNodesFromRow(obstacleRow, "ObstacleNode");

            foreach (Transform node in obstacleNodesByRow[obstacleRow])
            {
                nodeStatus[node] = ObstacleStatus.Empty;
            }
        }

        foreach (GameObject airObstacleRow in airObstacleRows)
        {
            airObstacleNodesByRow[airObstacleRow] = GetTaggedNodesFromRow(airObstacleRow, "AirObstacleNode");

            foreach (Transform node in airObstacleNodesByRow[airObstacleRow])
            {
                nodeStatus[node] = ObstacleStatus.Empty;
            }
        }
    }
}
