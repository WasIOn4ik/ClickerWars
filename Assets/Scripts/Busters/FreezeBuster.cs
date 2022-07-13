using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeBuster : BusterHandler
{
    public override bool Activate()
    {
        if (!base.Activate())
            return false;

        GameBase.instance.Freezed = true;
        GameBase.instance.freezeTime = Time.time;

        foreach (var en in GameBase.instance.activeEnemies)
        {
            en.Freeze(GameBase.instance.freezeDuration);
        }

        return true;
    }
}
