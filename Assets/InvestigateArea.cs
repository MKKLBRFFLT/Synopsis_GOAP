using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InvestigateArea : GAction
{
    public override bool PrePerform()
    {
        if (target == null)
        {
            Debug.Log("No target for InvestigateArea");
            return false;
        }
        return true;
    }

    public override bool PostPerform()
    {
        Debug.Log("Investigation complete");
        return true;
    }

    public override bool IsAchievable()
    {
        return true; // InvestigateArea er altid achievable hvis markeren allerede er sat
    }
}
