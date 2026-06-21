using System.Collections;
using UnityEngine;

public class RocketAttack : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private GameObject rocketPrefab;

    [Header("Timing")]
    [SerializeField] private float warningTime = 1f;

    [Header("Screen Positions")]
    [SerializeField] private float warningViewportX = 0.9f;
    [SerializeField] private float rocketSpawnViewportX = 1.15f;

    [Header("2D Plane")]
    [SerializeField] private float spawnZ = 0f;

    private GameObject currentWarning;

    private void Start()
    {
        StartCoroutine(RocketRoutine());
    }

    private IEnumerator RocketRoutine()
    {
        float targetY = transform.position.y;

        Vector3 warningPosition = GetScreenWorldPosition(warningViewportX, targetY);
        currentWarning = Instantiate(warningPrefab, warningPosition, warningPrefab.transform.rotation);

        yield return new WaitForSeconds(warningTime);

        if (GameManager.Instance != null && !GameManager.Instance.GameRunning)
        {
            Destroy(currentWarning);
            Destroy(gameObject);
            yield break;
        }

        if (currentWarning != null)
        {
            Destroy(currentWarning);
        }

        Vector3 rocketPosition = GetScreenWorldPosition(rocketSpawnViewportX, targetY);
        Instantiate(rocketPrefab, rocketPosition, rocketPrefab.transform.rotation);

        Destroy(gameObject);
    }

    private Vector3 GetScreenWorldPosition(float viewportX, float worldY)
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            return new Vector3(transform.position.x, worldY, spawnZ);
        }

        float distanceFromCamera = Mathf.Abs(spawnZ - cam.transform.position.z);

        Vector3 viewportPosition = new Vector3(viewportX, 0.5f, distanceFromCamera);
        Vector3 worldPosition = cam.ViewportToWorldPoint(viewportPosition);

        return new Vector3(worldPosition.x, worldY, spawnZ);
    }
}