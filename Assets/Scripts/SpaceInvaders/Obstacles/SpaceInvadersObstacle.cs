using System;
using UnityEngine;

public class SpaceInvadersObstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<SpaceInvadersPlayerStats>().TakeDamage(3);
            Debug.Log(other.gameObject.name);
        }
    }
}
