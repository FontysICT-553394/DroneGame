using UnityEngine;

public class SpaceInvadersControls : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject droneObject;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private bool allowHoldToShoot = false;

    private float lastShootTime = -Mathf.Infinity;

    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    void Update()
    {
        bool wantToShoot = allowHoldToShoot ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
        if (wantToShoot)
            Shoot();
    }

    void FixedUpdate()
    {
        Vector3 pos = new Vector3(droneObject.transform.position.x, droneObject.transform.position.y, transform.position.z);
        transform.position = pos;
    }

    private void Shoot()
    {
        if (!CanShoot())
            return;

        lastShootTime = Time.time;
    
        GameObject bullet = Instantiate(bulletPrefab, (Vector2) transform.position + Vector2.up * 65, transform.rotation);
        bullet.GetComponent<Bullet>().doesBulletGoDown = false;
        bullet.transform.localScale = Vector3.one * 800f;
    }

    private bool CanShoot()
    {
        return Time.time >= lastShootTime + shootCooldown;
    }
}
