using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPlayer : GAction
{
    GameObject player;
    MimicAgent mimicAgent;

    public override bool PrePerform()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            return false;
        }

        mimicAgent = GetComponent<MimicAgent>();
        if (mimicAgent == null || !mimicAgent.CanSeePlayer(player))
        {
            return false;
        }

        target = player;
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }
}
