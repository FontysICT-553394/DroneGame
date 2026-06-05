using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < value; i++)
            {
                GameManager.Instance.AddCoin();
            }

            Destroy(gameObject);
        }
    }
}