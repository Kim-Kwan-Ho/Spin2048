using UnityEngine;
using UnityEngine.Advertisements;


namespace KKH.Ads
{
    public class AdsInitializer : BaseBehaviour, IUnityAdsInitializationListener
    {
        [SerializeField] private string _gameId = "5812206";
        [SerializeField] private bool _testMode;
        protected override void Initialize()
        {
            base.Initialize();
            InitializeAds();
        }

        private void InitializeAds()
        {
            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, false, this);
            }
        }


        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }
    }

}
