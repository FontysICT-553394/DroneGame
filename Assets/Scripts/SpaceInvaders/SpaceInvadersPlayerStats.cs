using System;
using TMPro;
using UnityEngine;

public class SpaceInvadersPlayerStats : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] public int score = 0;
    

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            GameObject.Find("UIManager").GetComponent<SpaceInvaderUIManager>().GameOver(score);
        else
            GameObject.Find("UIManager").GetComponent<SpaceInvaderUIManager>().UpdateHealth(health);
    }

    public void AddScore(int score)
    {
        this.score += score;
    }
}
