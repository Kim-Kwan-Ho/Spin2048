using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;


public class BannerAds : BaseBehaviour
{
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.TOP_CENTER;
    [SerializeField] private string _adUnitId = "Banner_Android";

    private void Start()
    {
        Advertisement.Banner.SetPosition(_bannerPosition);
        StartCoroutine(CoLoadBanner());
    }
    private IEnumerator CoLoadBanner()
    {
        yield return new WaitUntil(() => Advertisement.isInitialized);
        LoadBanner();
    }
    private void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, options);
        ShowBanner();
    }

    private void ShowBanner()
    {
        Advertisement.Banner.Show(_adUnitId);
    }
    private void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
    }

    // Implement code to execute when the load errorCallback event triggers:
    private void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
    }




}