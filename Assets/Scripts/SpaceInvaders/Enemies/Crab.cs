using UnityEngine;

public class Crab : SpaceInvader
{
    private float nextShootTime;
    private float shootChance = 0.25f;
    private int score = 20;

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
        float cooldown = 2f;
        nextShootTime = Time.time + cooldown + Random.Range(1f, 3f);
    }

    public override void Die()
    {
        GameObject.Find("Player-2D").GetComponent<SpaceInvadersPlayerStats>().AddScore(score);
        base.Die();
    }
}