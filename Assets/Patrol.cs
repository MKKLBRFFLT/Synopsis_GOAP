using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : GAction
{
    private GameObject[] patrolPoints;
    private int currentPointIndex = 0;

    public override bool PrePerform()
    {
        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        if (patrolPoints.Length == 0)
        {
            Debug.Log("No patrol points found");
            return false;
        }

        Debug.Log("Patrol points found: " + patrolPoints.Length);
        target = patrolPoints[currentPointIndex];
        return true;
    }

    public override bool PostPerform()
    {
        Debug.Log("Reached patrol point: " + currentPointIndex);
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        return true;
    }

    public override bool IsAchievable()
    {
        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        return patrolPoints != null && patrolPoints.Length > 0;
    }
}