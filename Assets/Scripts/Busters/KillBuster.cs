using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBuster : BusterHandler
{
    public override bool Activate()
    {
        if (!base.Activate())
            return false;

        foreach (var en in GameBase.instance.activeEnemies)
        {
            if (!en.GetBoss())
                en.HandleDeath();
            else
                en.ReceiveDamage((int)(en.GetMaxHP() * 0.2f));
        }
        return true;
    }
}
