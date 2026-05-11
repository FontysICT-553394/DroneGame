using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public abstract class SpaceInvader : MonoBehaviour
{
    [SerializeField] private int health = 1;
    [SerializeField] private int damage = 1;
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float moveDelay = 2f;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private List<GameObject> bulletPrefabs = new();

    public bool CanShoot()
    {
        float yOffset = 50f;
        Vector2 origin = (Vector2)transform.position + Vector2.down * yOffset;
        Vector2 direction = Vector2.down;
        float maxDistance = 750f;

        Debug.DrawRay(origin, direction * maxDistance, Color.red, 10f);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance);

        return hit.collider == null || !hit.collider.CompareTag("Enemy");
    }
    
    public virtual bool Shoot()
    {
        if (CanShoot())
        {
            int random = new Random().Next(3);
            GameObject bullet = Instantiate(bulletPrefabs[random], (Vector2) transform.position + Vector2.down * 50, transform.rotation);
            bullet.transform.localScale = Vector3.one * 800f;
        }

        return false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }
    
    public virtual IEnumerator MoveRoutine()
    {
        // Adjust the delay between moves as needed
        WaitForSeconds wait = new WaitForSeconds(moveDelay);

        while (true)
        {
            // Move Right 3 times
            for (int i = 0; i < 3; i++)
            {
                transform.Translate(Vector2.right * moveSpeed);
                yield return wait;
            }

            // Move Down
            transform.Translate(Vector2.down * moveSpeed);
            yield return wait;

            // Move Left 3 times
            for (int i = 0; i < 3; i++)
            {
                transform.Translate(Vector2.left * moveSpeed);
                yield return wait;
            }

            // Move Down
            transform.Translate(Vector2.down * moveSpeed);
            yield return wait;
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
