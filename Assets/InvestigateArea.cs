using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InvestigateArea : GAction
{
    public override bool PrePerform()
    {
        // Ensure there's a target marker set
        if (target == null)
        {
            return false;
        }
        return true;
    }

    public override bool PostPerform()
    {
        // Action completed successfully
        return true;
    }
}
