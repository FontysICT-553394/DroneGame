using UnityEngine;

public class OrbitCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform drone;
    public Transform lookTarget;

    [Header("Camera Distance")]
    public float distanceBehindDrone = 3.5f;
    public float heightAboveDrone = 1.2f;

    [Header("Look Settings")]
    public float lookTargetHeightOffset = 0.5f;

    [Header("Smoothing")]
    public float positionSmoothTime = 0.25f;
    public float rotationSmoothTime = 0.2f;

    [Header("Start Behaviour")]
    public bool snapToPositionOnStart = true;

    private Vector3 positionVelocity;
    private Vector3 rotationVelocity;

    private void Start()
    {
        if (snapToPositionOnStart)
        {
            SnapCameraToTargetPosition();
        }
    }

    private void LateUpdate()
    {
        if (drone == null || lookTarget == null)
        {
            return;
        }

        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    private void SnapCameraToTargetPosition()
    {
        if (drone == null || lookTarget == null)
        {
            return;
        }

        Vector3 targetPosition = CalculateTargetPosition();
        Quaternion targetRotation = CalculateTargetRotation(targetPosition);

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        positionVelocity = Vector3.zero;
        rotationVelocity = Vector3.zero;
    }

    private void UpdateCameraPosition()
    {
        Vector3 targetPosition = CalculateTargetPosition();

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref positionVelocity,
            positionSmoothTime
        );
    }

    private void UpdateCameraRotation()
    {
        Quaternion targetRotation = CalculateTargetRotation(transform.position);

        Vector3 currentEuler = transform.rotation.eulerAngles;
        Vector3 targetEuler = targetRotation.eulerAngles;

        Vector3 smoothEuler = new Vector3(
            Mathf.SmoothDampAngle(
                currentEuler.x,
                targetEuler.x,
                ref rotationVelocity.x,
                rotationSmoothTime
            ),
            Mathf.SmoothDampAngle(
                currentEuler.y,
                targetEuler.y,
                ref rotationVelocity.y,
                rotationSmoothTime
            ),
            Mathf.SmoothDampAngle(
                currentEuler.z,
                targetEuler.z,
                ref rotationVelocity.z,
                rotationSmoothTime
            )
        );

        transform.rotation = Quaternion.Euler(smoothEuler);
    }

    private Vector3 CalculateTargetPosition()
    {
        Vector3 directionFromCenterToDrone = drone.position - lookTarget.position;

        if (directionFromCenterToDrone.sqrMagnitude < 0.001f)
        {
            directionFromCenterToDrone = Vector3.forward;
        }

        directionFromCenterToDrone.Normalize();

        Vector3 targetPosition =
            drone.position +
            directionFromCenterToDrone * distanceBehindDrone +
            Vector3.up * heightAboveDrone;

        return targetPosition;
    }

    private Quaternion CalculateTargetRotation(Vector3 cameraPosition)
    {
        Vector3 targetLookPosition = lookTarget.position + Vector3.up * lookTargetHeightOffset;
        Vector3 directionToTarget = targetLookPosition - cameraPosition;

        if (directionToTarget.sqrMagnitude < 0.001f)
        {
            directionToTarget = Vector3.forward;
        }

        return Quaternion.LookRotation(directionToTarget.normalized, Vector3.up);
    }
}