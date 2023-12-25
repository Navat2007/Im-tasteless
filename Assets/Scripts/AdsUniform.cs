using InstantGamesBridge;
using InstantGamesBridge.Modules.Advertisement;
using UnityEngine;

public class AdsUniform : MonoBehaviour
{
    bool _ignoreDelay = false;
    
    private void Awake()
    {
        EventBus.AdsEvents.OnAdsNeedToShow += ShowFullscreenAds;
        Bridge.advertisement.interstitialStateChanged += OnInterstitialStateChanged;
    }

    private void OnDestroy()
    {
        EventBus.AdsEvents.OnAdsNeedToShow -= ShowFullscreenAds;
        Bridge.advertisement.interstitialStateChanged -= OnInterstitialStateChanged;
    }
    
    private void OnInterstitialStateChanged(InterstitialState state)
    {
        Debug.Log($"Ads state changed: {state}");
        
        switch (state)
        {
            case InterstitialState.Opened:
                EventBus.AdsEvents.OnAdsShown?.Invoke();
                break;
            
            case InterstitialState.Closed:
                EventBus.AdsEvents.OnAdsClose?.Invoke();
                break;
            
            case InterstitialState.Failed:
                EventBus.AdsEvents.OnAdsFailed?.Invoke();
                break;
        }
    }
    
    private void ShowFullscreenAds()
    {
        Debug.Log("ShowFullscreenAds");
        
        Bridge.advertisement.ShowInterstitial(_ignoreDelay);
    }
}