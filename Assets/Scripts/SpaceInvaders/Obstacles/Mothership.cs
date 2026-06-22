using System;
using System.Collections;
using UnityEngine;

public class Mothership : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float despawnTime = 5f;
    
    public Vector2 moveDirection;

    private void Start()
    {
        StartCoroutine(Despawn());
    }
    
    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection.normalized * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<SpaceInvadersPlayerStats>().TakeDamage(3);
        }
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}
