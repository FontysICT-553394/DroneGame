using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Spawn Area")]
    [SerializeField] private float minY = -3f;
    [SerializeField] private float maxY = 4f;
    [SerializeField] private float spawnX = 15f;
    [SerializeField] private float spawnZ = 0f;

    private float timer;

    void Update()
    {
        if (!GameManager.Instance.GameRunning) return;
        
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject prefab = obstaclePrefabs[randomIndex];

        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnX, randomY, spawnZ);

        Instantiate(prefab, spawnPosition, prefab.transform.rotation);
    }
}
