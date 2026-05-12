using UnityEngine;

public class Squid : SpaceInvader
{
    private float nextShootTime;
    private float shootChance = 0.33f;
    private int score = 30;

    void Start()
    {
        SetNextShootTime();
        StartCoroutine(MoveRoutine());
    }

    public override void Update()
    {
        base.Update();

        if (Time.time >= nextShootTime)
        {
            if (Random.value <= shootChance)
            {
                Shoot();
            }
            SetNextShootTime();
        }
    }

    private void SetNextShootTime()
    {
        float cooldown = 1f;
        nextShootTime = Time.time + cooldown + Random.Range(0.5f, 2f);
    }
    
    public override void Die()
    {
        GameObject.Find("Player-2D").GetComponent<SpaceInvadersPlayerStats>().AddScore(score);
        base.Die();
    }
}