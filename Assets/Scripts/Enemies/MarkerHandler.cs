using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerHandler : MonoBehaviour
{
    #region Variables

    public bool bAvailable;

    #endregion

    #region Functions

    public void Lock()
    {
        bAvailable = false;
        gameObject.SetActive(true);
    }

    public void Unlock()
    {
        bAvailable = true;
        gameObject.SetActive(false);
    }

    #endregion
}
