using System;
using UnityEngine;

public class Crab : SpaceInvader
{
    void Start()
    {
        Shoot();
        StartCoroutine(MoveRoutine());
    }
}
