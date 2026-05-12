using UnityEngine;

public class Octopus : SpaceInvader
{
    private float nextShootTime;
    private float shootChance = 0.33f;
    private int score = 10;

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
        float cooldown = 3f;
        nextShootTime = Time.time + cooldown + Random.Range(2f, 4f);
    }
    
    public override void Die()
    {
        GameObject.Find("Player-2D").GetComponent<SpaceInvadersPlayerStats>().AddScore(score);
        base.Die();
    }
}