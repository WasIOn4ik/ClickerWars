using GoogleMobileAds.Api;
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

    private BannerView bannerView;

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
        MobileAds.Initialize(initStatus => { });
        RequestBanner();
    }
    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        AdSize adSize = new AdSize(728, 90);
        this.bannerView = new BannerView(adUnitId, adSize, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    #endregion
}
