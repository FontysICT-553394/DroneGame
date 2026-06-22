using System.Collections;
using UnityEngine;

public class RocketWarningObstacle : MonoBehaviour
{
    [Header("Warning")]
    [SerializeField] private GameObject warningObject;

    [Header("Rocket")]
    [SerializeField] private GameObject rocketPrefab;

    [Header("Timing")]
    [SerializeField] private float warningTime = 1.2f;

    [Header("Rocket Spawn Offset")]
    [SerializeField] private float rocketSpawnOffsetX = 7f;

    private void Start()
    {
        if (warningObject != null)
        {
            warningObject.SetActive(true);
        }

        StartCoroutine(SpawnRocketAfterWarning());
    }

    private IEnumerator SpawnRocketAfterWarning()
    {
        yield return new WaitForSeconds(warningTime);

        Vector3 rocketSpawnPosition = transform.position + new Vector3(rocketSpawnOffsetX, 0f, 0f);

        if (rocketPrefab != null)
        {
            Instantiate(
                rocketPrefab,
                rocketSpawnPosition,
                rocketPrefab.transform.rotation
            );
        }

        Destroy(gameObject);
    }
}