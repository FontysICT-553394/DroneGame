using System.Collections;
using UnityEngine;

public class RocketSequence : MonoBehaviour
{
    [Header("Rocket Attack Prefab")]
    [SerializeField] private GameObject rocketAttackPrefab;

    [Header("Sequence Settings")]
    [SerializeField] private float[] yOffsets;
    [SerializeField] private float delayBetweenRockets = 0.5f;

    private void Start()
    {
        StartCoroutine(SpawnRocketSequence());
    }

    private IEnumerator SpawnRocketSequence()
    {
        for (int i = 0; i < yOffsets.Length; i++)
        {
            if (GameManager.Instance != null && !GameManager.Instance.GameRunning)
            {
                yield break;
            }

            Vector3 spawnPosition = new Vector3(
                transform.position.x,
                transform.position.y + yOffsets[i],
                transform.position.z
            );

            Instantiate(
                rocketAttackPrefab,
                spawnPosition,
                rocketAttackPrefab.transform.rotation
            );

            yield return new WaitForSeconds(delayBetweenRockets);
        }

        Destroy(gameObject);
    }
}