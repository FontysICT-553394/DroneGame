using UnityEngine;
using UnityEngine.UI;

public class HeartHUD : MonoBehaviour
{
    [SerializeField] private Image[] hearts;

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentHealth;
        }
    }
}
