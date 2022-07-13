using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusterHandler : MonoBehaviour
{
    #region Variables

    public float CDTime = 20f;
    public Image im;
    public bool available;

    public AudioClip useSound;
    public float volume = 0.5f;

    private float lastUseTime;

    #endregion

    #region UnityCallbacks

    public void Awake()
    {
        lastUseTime = Time.time;
    }

    public void FixedUpdate()
    {
        if (available)
            return;

        float fillAmount = (Time.time - lastUseTime) / CDTime;
        im.fillAmount = fillAmount;
        if (fillAmount >= 1)
        {
            available = true;
        }
    }

    #endregion

    #region Functions

    public virtual bool Activate()
    {
        if (!available)
            return false;
        if (!GameBase.instance.gameActive)
            return false;

        AudioSource.PlayClipAtPoint(useSound, Camera.main.transform.position, volume);

        lastUseTime = Time.time;
        available = false;
        return true;
    }

    #endregion

    #region UIFunctions

    public void CallActivate()
    {
        Activate();
    }

    #endregion
}
