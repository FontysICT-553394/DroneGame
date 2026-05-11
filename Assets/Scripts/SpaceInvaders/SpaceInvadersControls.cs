using UnityEngine;

public class SpaceInvadersControls : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject droneObject;
    [SerializeField] private float shootCooldown = 2f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //player2D.GetComponent<SpriteRenderer>().enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            Shoot();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = new Vector3(droneObject.transform.position.x, droneObject.transform.position.y, transform.position.z);
        transform.position = pos;
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, (Vector2) transform.position + Vector2.up * 50, transform.rotation);
        bullet.GetComponent<Bullet>().doesBulletGoDown = false;
        bullet.transform.localScale = Vector3.one * 800f;
    }
}
