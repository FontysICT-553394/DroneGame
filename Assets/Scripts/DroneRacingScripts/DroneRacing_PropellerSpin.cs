using System.Collections.Generic;
using UnityEngine;

public class SpinAllPropellors : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 1000f;

    private readonly List<Transform> propellors = new();

    void Start()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name.ToLower().Contains("propellor"))
            {
                propellors.Add(child);
            }
        }

        //Debug.Log($"Found {propellors.Count} propellors");
    }

    void Update()
    {
        foreach (Transform propellor in propellors)
        {
            propellor.Rotate(Vector3.up * spinSpeed * Time.deltaTime, Space.Self);
        }
    }
}