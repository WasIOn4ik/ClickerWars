using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    #region Variables

    [Header("Submenus")]
    [SerializeField] private GameObject startMenu;
    [SerializeField] private Leadership leadership;
    [SerializeField] private GameObject credits;

    #endregion

    #region UIFunctions

    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnLeadershipClicked()
    {
        startMenu.SetActive(false);
        leadership.gameObject.SetActive(true);
        leadership.Populate();
    }

    public void OnCreditsClicked()
    {
        startMenu.SetActive(false);
        credits.SetActive(true);
    }

    public void OnExitClicked()
    {
        Leadership.Save();
        Application.Quit();
    }

    public void OnBackToMenuClicked()
    {
        startMenu.SetActive(true);
        credits.SetActive(false);
        leadership.gameObject.SetActive(false);
    }

    #endregion

    #region UnityCallbacks

    public void Awake()
    {
        Application.targetFrameRate = 60;
        Leadership.Load();
    }

    #endregion
}
