using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BoxCollider2D hitbox; 
    public float speed = 25f; 
    public int damage = 1;
    public float stepInterval = 0.8f;
    public bool doesBulletGoDown = true;
    
    private float timer = 0f;
    
    private void Awake()
    {
        timer = 0f;
    }

    public void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
    
        while (timer >= stepInterval)
        {
            timer -= stepInterval;
            if(doesBulletGoDown)
                transform.position += Vector3.down * speed;
            else
                transform.position += Vector3.up * speed;
        }
        
        // Check for collision with enemies, ignoring Z-axis
        Vector2 position = transform.position; 
        Vector2 size = hitbox.size;
        
        Collider2D[] hits = Physics2D.OverlapBoxAll(position, size, 0f);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                if (hit.CompareTag("Enemy"))
                {
                    hit.gameObject.GetComponent<SpaceInvader>().TakeDamage(damage);
                    Destroy(gameObject);
                }
                else if (hit.CompareTag("Player"))
                {
                    hit.gameObject.GetComponent<SpaceInvadersPlayerStats>().TakeDamage(damage);
                    Destroy(gameObject);
                }
            }
        }
    }
}
