using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private HeartHUD heartHUD;

    [Header("Damage Feedback")]
    [SerializeField] private PlayerDamageFlash damageFlash;

    [Header("Damage Cooldown")]
    [SerializeField] private float invincibleTime = 1f;

    private int currentHealth;
    private bool isInvincible;

    private void Start()
    {
        currentHealth = maxHealth;

        if (heartHUD != null)
        {
            heartHUD.UpdateHearts(currentHealth);
        }
    }

    public void TakeDamage()
    {
        if (GameManager.Instance != null && !GameManager.Instance.GameRunning)
        {
            return;
        }

        if (isInvincible)
        {
            return;
        }

        currentHealth--;

        if (damageFlash != null)
        {
            damageFlash.Flash();
        }

        if (heartHUD != null)
        {
            heartHUD.UpdateHearts(currentHealth);
        }

        Debug.Log("Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(Invincibility());
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
    }

    private void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }

        gameObject.SetActive(false);
    }
}