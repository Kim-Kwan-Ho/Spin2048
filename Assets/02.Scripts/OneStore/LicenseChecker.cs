using OneStore.Alc;
using UnityEngine;


namespace KKH.OneStore
{
    public class LicenseChecker : MonoBehaviour
    {

        private void Awake()
        {
            OneStoreAppLicenseCheckerImpl _appLicenseCheckerImpl = new OneStoreAppLicenseCheckerImpl("MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC2epppDV2FphjjLutH3w/8eAHHxfUYXJhVIw8KQwtVZOJmizeoHBcFf7+nquuRNuvfQcFzbholkb3xFTsrJUIzat3YmbGIJ7We5QuDMwiZ633Tz4DXQf0SphIh8dWNymO9xyroElEluX4svIBwwQ586Ruaw8HeZWe39B7n2wyrMQIDAQAB");
            _appLicenseCheckerImpl.Initialize(new YourCallback());

        }
        
    }
    class YourCallback : ILicenseCheckCallback
    {
        // ILicenseCheckCallback implementations.
        public void OnGranted(string license, string signature) { }
        public void OnDenied() { }
        public void OnError(int code, string message){}
    }
}

