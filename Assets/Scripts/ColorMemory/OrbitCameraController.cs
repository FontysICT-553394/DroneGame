using UnityEngine;

public class OrbitCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform drone;
    public Transform lookTarget;

    [Header("Camera Distance")]
    public float distanceBehindDrone = 2.8f;
    public float heightAboveDrone = 0.9f;

    [Header("Look Settings")]
    public float lookTargetHeightOffset = 0f;

    [Header("Smoothing")]
    public float positionSmoothTime = 0.25f;
    public float rotationSmoothTime = 0.2f;

    private Vector3 positionVelocity;
    private Vector3 rotationVelocity;

    private void LateUpdate()
    {
        if (drone == null || lookTarget == null)
        {
            return;
        }

        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    private void UpdateCameraPosition()
    {
        Vector3 directionFromCenterToDrone = drone.position - lookTarget.position;

        if (directionFromCenterToDrone.sqrMagnitude < 0.001f)
        {
            return;
        }

        directionFromCenterToDrone.Normalize();

        Vector3 targetPosition =
            drone.position +
            directionFromCenterToDrone * distanceBehindDrone +
            Vector3.up * heightAboveDrone;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref positionVelocity,
            positionSmoothTime
        );
    }

    private void UpdateCameraRotation()
    {
        Vector3 targetLookPosition = lookTarget.position + Vector3.up * lookTargetHeightOffset;
        Vector3 directionToTarget = targetLookPosition - transform.position;

        if (directionToTarget.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(
            directionToTarget.normalized,
            Vector3.up
        );

        Vector3 currentEuler = transform.rotation.eulerAngles;
        Vector3 targetEuler = targetRotation.eulerAngles;

        Vector3 smoothEuler = new Vector3(
            Mathf.SmoothDampAngle(currentEuler.x, targetEuler.x, ref rotationVelocity.x, rotationSmoothTime),
            Mathf.SmoothDampAngle(currentEuler.y, targetEuler.y, ref rotationVelocity.y, rotationSmoothTime),
            Mathf.SmoothDampAngle(currentEuler.z, targetEuler.z, ref rotationVelocity.z, rotationSmoothTime)
        );

        transform.rotation = Quaternion.Euler(smoothEuler);
    }
}