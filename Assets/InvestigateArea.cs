using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InvestigateArea : GAction
{
    public override bool PrePerform()
    {
        // check for marker
        if (target == null)
        {
            return false;
        }
        return true;
    }

    public override bool PostPerform()
    {
        
        return true;
    }
}
