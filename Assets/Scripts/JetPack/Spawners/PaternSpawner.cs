using UnityEngine;

public class PatternSpawner : MonoBehaviour
{
    [Header("Pattern Settings")]
    [SerializeField] private GameObject[] patternPrefabs;
    [SerializeField] private float spawnInterval = 3f;

    [Header("Spawn Position")]
    [SerializeField] private float spawnX = 15f;
    [SerializeField] private float spawnY = 0f;
    [SerializeField] private float spawnZ = 0f;

    private float timer;

    void Update()
    {
        if (!GameManager.Instance.GameRunning) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnPattern();
            timer = 0f;
        }
    }

    void SpawnPattern()
    {
        if (patternPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, patternPrefabs.Length);
        GameObject prefab = patternPrefabs[randomIndex];

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

        Instantiate(prefab, spawnPosition, prefab.transform.rotation);
    }
}
