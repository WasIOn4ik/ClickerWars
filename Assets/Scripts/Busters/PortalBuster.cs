using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBuster : BusterHandler
{
    public override bool Activate()
    {
        if (!base.Activate())
            return false;

        foreach (var en in GameBase.instance.activeEnemies)
        {
            if (!en.GetBoss())
                en.transform.position = Vector3.zero;
        }

        return true;
    }
}
