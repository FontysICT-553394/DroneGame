using UnityEngine;

public class DroneRacing_RotateBattery : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);  
    }
}
