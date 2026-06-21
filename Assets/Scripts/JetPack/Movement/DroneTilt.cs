using UnityEngine;

public class DroneTilt : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform droneVisual;

    [Header("Tilt Settings")]
    [SerializeField] private float maxTiltAngle = 20f;
    [SerializeField] private float tiltSpeed = 8f;

    private void Update()
    {
        if (droneVisual == null) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Naar rechts/voren vliegen = naar voren leunen
        // Naar links/achter vliegen = naar achter leunen
        float targetZRotation = -horizontalInput * maxTiltAngle;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZRotation);

        droneVisual.localRotation = Quaternion.Lerp(
            droneVisual.localRotation,
            targetRotation,
            tiltSpeed * Time.deltaTime
        );
    }
}
