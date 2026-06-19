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
    [SerializeField] private GameObject planePrefab;
    [SerializeField] private GameObject bannerPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float planeSpawnDistance = 30f;
    [SerializeField, Range(0f, 1f)] private float bannerSpawnChance = 0.25f;
    [SerializeField] private bool debugPlaneSpawning = false;

    private readonly List<Transform> pendingPlaneNodes = new List<Transform>();
    private bool loggedMissingPlanePrefab;
    private bool loggedMissingPlayer;
    private bool loggedNoPlaneNodes;
    private float nextPlaneDebugTime;
    private bool loggedMissingPlaneTag;
    private bool warnedNoPlaneNodesWithPlanePrefab;

    private static List<Transform> GetTaggedNodesFromRow(GameObject row, string nodeTag)
    {
        if (row == null)
            return new List<Transform>();

        Transform rowTransform = row.transform;

        return rowTransform
            .GetComponentsInChildren<Transform>()
            .Where(t => t != rowTransform && SafeCompareTag(t, nodeTag))
            .ToList();
    }

    private static bool SafeCompareTag(Transform t, string tag)
    {
        if (t == null || string.IsNullOrEmpty(tag))
            return false;

        try
        {
            return t.CompareTag(tag);
        }
        catch (UnityException)
        {
            return false;
        }
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
        if (airObstaclePosition != null && SafeCompareTag(airObstaclePosition, "PlaneObstacleNode"))
        {
            if (planePrefab == null)
                return;

            // Queue plane spawn until player is nearby. Mark node occupied so batteries don't spawn here.
            if (!pendingPlaneNodes.Contains(airObstaclePosition))
            {
                pendingPlaneNodes.Add(airObstaclePosition);
                nodeStatus[airObstaclePosition] = ObstacleStatus.Occupied;
            }

            return;
        }

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
            if (Random.value > bannerSpawnChance)
                continue;

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
            // Skip nodes reserved for planes 
            if (SafeCompareTag(node, "PlaneObstacleNode"))
                continue;

            GameObject newBattery = Instantiate(
                batteryPrefab,
                node.position + new Vector3(0f, batteryHeightOffset, 0f),
                batteryPrefab.transform.rotation
            );

            newBattery.transform.SetParent(node);

            nodeStatus[node] = ObstacleStatus.Occupied;
        }
    }

    private void Update()
    {
        if (playerTransform == null)
            TryResolvePlayerTransform();

        if (pendingPlaneNodes.Count == 0)
        {
            if (debugPlaneSpawning)
            {
                WarnIfTagMissingOnce("PlaneObstacleNode", ref loggedMissingPlaneTag);

                if (!loggedNoPlaneNodes)
                {
                    Debug.LogWarning($"[GenerateObstacles] No PlaneObstacleNodes queued yet on '{name}'.");
                    loggedNoPlaneNodes = true;
                }
            }

            return;
        }

        if (planePrefab == null)
        {
            if (debugPlaneSpawning && !loggedMissingPlanePrefab)
            {
                Debug.LogWarning("[GenerateObstacles] planePrefab is not assigned.");
                loggedMissingPlanePrefab = true;
            }

            return;
        }

        if (playerTransform == null)
        {
            if (debugPlaneSpawning && !loggedMissingPlayer)
            {
                Debug.LogWarning("[GenerateObstacles] playerTransform is null (assign it or ensure Player tag / DroneMovementRacingDrone exists).");
                loggedMissingPlayer = true;
            }

            return;
        }

        if (debugPlaneSpawning && Time.time >= nextPlaneDebugTime)
        {
            float minDist = float.PositiveInfinity;

            for (int j = 0; j < pendingPlaneNodes.Count; j++)
            {
                Transform n = pendingPlaneNodes[j];
                if (n == null) continue;

                float d = Vector3.Distance(playerTransform.position, n.position);
                if (d < minDist) minDist = d;
            }

            Debug.Log($"[GenerateObstacles] Pending planes: {pendingPlaneNodes.Count}, closestDist: {minDist:0.00}, spawnDist: {planeSpawnDistance:0.00}");
            nextPlaneDebugTime = Time.time + 1f;
        }

        for (int i = pendingPlaneNodes.Count - 1; i >= 0; i--)
        {
            Transform node = pendingPlaneNodes[i];

            if (node == null)
            {
                pendingPlaneNodes.RemoveAt(i);
                continue;
            }

            float dist = Vector3.Distance(playerTransform.position, node.position);

            if (dist <= planeSpawnDistance)
            {
                GameObject newPlane = Instantiate(
                    planePrefab,
                    node.position,
                    planePrefab.transform.rotation
                );

                newPlane.transform.SetParent(node);
                AlignBottomToNode(newPlane, node.position.y + 2f);

                pendingPlaneNodes.RemoveAt(i);
            }
        }
    }

    private void Awake()
    {
        TryResolvePlayerTransform();

        if (debugPlaneSpawning)
            WarnIfTagMissingOnce("PlaneObstacleNode", ref loggedMissingPlaneTag);
    }

    private static void WarnIfTagMissingOnce(string tag, ref bool alreadyLogged)
    {
        if (alreadyLogged)
            return;

        try
        {
            // This throws UnityException if the tag isn't defined.
            GameObject.FindWithTag(tag);
        }
        catch (UnityException)
        {
            Debug.LogWarning($"[GenerateObstacles] Tag '{tag}' is not defined in the Tag Manager.");
            alreadyLogged = true;
        }
    }

    private void TryResolvePlayerTransform()
    {
        if (playerTransform != null)
            return;

        try
        {
            GameObject playerObj = GameObject.FindWithTag("Player");

            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                return;
            }
        }
        catch (UnityException)
        {
            // Tag "Player" might not exist; ignore.
        }

        DroneMovementRacingDrone drone = FindAnyObjectByType<DroneMovementRacingDrone>();

        if (drone != null)
            playerTransform = drone.transform;
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

        if (!warnedNoPlaneNodesWithPlanePrefab && planePrefab != null && pendingPlaneNodes.Count == 0)
        {
            bool tagDefined = true;

            try
            {
                GameObject.FindWithTag("PlaneObstacleNode");
            }
            catch (UnityException)
            {
                tagDefined = false;
            }

            if (!tagDefined)
            {
                Debug.LogWarning("[GenerateObstacles] planePrefab is assigned, but tag 'PlaneObstacleNode' is NOT defined. Create the tag and assign it to your plane spawn nodes.");
            }
            else
            {
                Debug.LogWarning("[GenerateObstacles] planePrefab is assigned, but 0 PlaneObstacleNode nodes were found under AirObstacleRow. Check that your nodes have tag 'PlaneObstacleNode' and are children of an AirObstacleRow.");
            }

            warnedNoPlaneNodesWithPlanePrefab = true;
        }

        if (debugPlaneSpawning)
            Debug.Log($"[GenerateObstacles] '{name}' rows: air={airObstacleRows.Count}, ground={obstacleRows.Count}, pendingPlanes={pendingPlaneNodes.Count}");

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
            .Where(t => SafeCompareTag(t, "ObstacleRow"))
            .Select(t => t.gameObject)
            .ToList();

        airObstacleRows = trackSegment
            .GetComponentsInChildren<Transform>()
            .Where(t => SafeCompareTag(t, "AirObstacleRow"))
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
            List<Transform> airNodes = GetTaggedNodesFromRow(airObstacleRow, "AirObstacleNode");
            List<Transform> planeNodes = GetTaggedNodesFromRow(airObstacleRow, "PlaneObstacleNode");

            // Only air obstacle nodes participate in air obstacle randomization.
            airObstacleNodesByRow[airObstacleRow] = airNodes;

            foreach (Transform node in airNodes)
            {
                nodeStatus[node] = ObstacleStatus.Empty;
            }

            // Plane nodes are always queued to spawn later (when player is nearby).
            foreach (Transform planeNode in planeNodes)
            {
                if (planeNode == null)
                    continue;

                nodeStatus[planeNode] = ObstacleStatus.Occupied;

                if (!pendingPlaneNodes.Contains(planeNode))
                    pendingPlaneNodes.Add(planeNode);
            }

            if (debugPlaneSpawning)
                Debug.Log($"[GenerateObstacles] AirRow '{airObstacleRow.name}': airNodes={airNodes.Count}, planeNodes={planeNodes.Count}");
        }
    }
}