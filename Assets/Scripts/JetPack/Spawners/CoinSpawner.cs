using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float spawnInterval = 2.5f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnX = 15f;
    [SerializeField] private float minY = -3f;
    [SerializeField] private float maxY = 4f;
    [SerializeField] private float spawnZ = 0f;

    [Header("Coin Line")]
    [SerializeField] private int coinsPerLine = 6;
    [SerializeField] private float coinSpacing = 0.8f;

    private float timer;

    void Update()
    {
        if (!GameManager.Instance.GameRunning) return;
        
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnCoinLine();
            timer = 0f;
        }
    }

    void SpawnCoinLine()
    {
        float randomY = Random.Range(minY, maxY);

        for (int i = 0; i < coinsPerLine; i++)
        {
            Vector3 spawnPosition = new Vector3(
                spawnX + i * coinSpacing,
                randomY,
                spawnZ
            );

            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
