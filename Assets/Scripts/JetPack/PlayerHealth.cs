using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;

    public void TakeDamage()
    {
        health--;

        Debug.Log("Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.Instance.GameOver();
        gameObject.SetActive(false);
    }
}