using KKH.Manager;
using UnityEngine;
using UnityEngine.Advertisements;



namespace KKH.Ads
{
    public class InterstitialAds : BaseBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] private string _adUnitId = "Interstitial_Android";

        private void OnEnable()
        {
            GameManager.Instance.OnGameOver += ShowIntAds;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameOver -= ShowIntAds;
        }


        private void LoadAds()
        {
            Advertisement.Load(_adUnitId, this);
        }

        private void ShowAds()
        {
            Advertisement.Show(_adUnitId, this);
        }
        private void ShowIntAds()
        {
            LoadAds();
            ShowAds();
        }

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            // Optionally execute code if the Ad Unit successfully loads content.
        }

        public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        }

        public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
        }

        public void OnUnityAdsShowStart(string _adUnitId) { }
        public void OnUnityAdsShowClick(string _adUnitId) { }
        public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState) { }
    }

}
