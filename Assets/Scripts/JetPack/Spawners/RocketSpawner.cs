using System.Collections;
using UnityEngine;

public class RocketSpawner : MonoBehaviour
{
    [Header("Rocket Settings")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private GameObject warningPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float warningTime = 1.2f;

    [Header("Positions")]
    [SerializeField] private float spawnX = 15f;
    [SerializeField] private float warningX = 8f;
    [SerializeField] private float minY = -3f;
    [SerializeField] private float maxY = 4f;
    [SerializeField] private float spawnZ = 0f;

    private float timer;

    void Update()
    {
        if (!GameManager.Instance.GameRunning) return;
        
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            StartCoroutine(SpawnRocketWithWarning());
            timer = 0f;
        }
    }

    IEnumerator SpawnRocketWithWarning()
    {
        float randomY = Random.Range(minY, maxY);

        Vector3 warningPosition = new Vector3(warningX, randomY, spawnZ);
        GameObject warning = Instantiate(warningPrefab, warningPosition, Quaternion.identity);

        yield return new WaitForSeconds(warningTime);

        if (warning != null)
        {
            Destroy(warning);
        }

        Vector3 rocketPosition = new Vector3(spawnX, randomY, spawnZ);
        Instantiate(rocketPrefab, rocketPosition, Quaternion.identity);
    }
}
