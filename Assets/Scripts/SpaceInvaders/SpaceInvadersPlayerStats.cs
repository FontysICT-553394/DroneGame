using System;
using UnityEngine;

public class SpaceInvadersPlayerStats : MonoBehaviour
{
    [SerializeField] private int health = 3;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(this);
        }
    }
}
