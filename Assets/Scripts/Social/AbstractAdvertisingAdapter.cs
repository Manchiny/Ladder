using RSG;
using System;
using UnityEngine;

namespace Assets.Scripts.Social.Adverts
{
    public abstract class AbstractAdvertisingAdapter
    {
        private Promise _interstitialPromise;

        private event Action Rewarded;
        private event Action RevardedAdsOpened;
        private event Action RewardedAdsClosed;
        private event Action RewardAdsError;
        public abstract string Tag { get; }

        public Promise ShowInterstitial(AbstractSocialAdapter social)
        {
            if (social.IsInited == false)
            {
                Promise promise = new Promise();
                promise.Reject(null);

                return promise;
            }

            if (_interstitialPromise != null && _interstitialPromise.CurState == PromiseState.Pending)
                return _interstitialPromise;
            else
            {
                _interstitialPromise = new Promise();
                ShowInterstitial(OnInterstitialOpen, OnInterstitilaClose, OnInterstitiaError, OnIntersitialOffline);

                return _interstitialPromise;
            }
        }

        public void ShowRewardedVideo(AbstractSocialAdapter social, Action onOpen, Action onRewarded, Action onClose, Action onError)
        {
            if (social.IsInited == false)
                return;

            RevardedAdsOpened = onOpen;
            Rewarded = onRewarded;
            RewardedAdsClosed = onClose;
            RewardAdsError = onError;

            ShowRewarded(OnRewardedOpen, OnRewarded, OnRewardedClose, OnRewardedError);         
        }

        protected abstract void ShowInterstitial(Action onOpen, Action<bool> onClose, Action<string> onError, Action onOffline);
        protected abstract void ShowRewarded(Action onOpen, Action onRewarded, Action onClose, Action<string> onError);

        private void OnInterstitilaClose(bool close)
        {
            if (_interstitialPromise != null && _interstitialPromise.CurState == PromiseState.Pending)
                _interstitialPromise.ResolveOnce();
        }

        private void OnInterstitiaError(string errorMessage)
        {
            if (_interstitialPromise != null && _interstitialPromise.CurState == PromiseState.Pending)
                _interstitialPromise.ResolveOnce();
            

            Debug.LogError($"[{Tag}]: {errorMessage}");
        }

        private void OnInterstitialOpen()
        {
            Debug.LogError($"[{Tag}]: opened interstitial video");
        }

        private void OnIntersitialOffline()
        {

        }

        protected void OnRewardedOpen()
        {
            Debug.LogError($"[{Tag}]: opened rewarded video");

            if (RevardedAdsOpened != null)
            {
                RevardedAdsOpened?.Invoke();
                RevardedAdsOpened = null;
            }
        }

        protected void OnRewarded()
        {
            Debug.LogError($"[{Tag}]: rewarded!");

            if (Rewarded != null)
            {
                Rewarded?.Invoke();
                Rewarded = null;
            }
        }

        private void OnRewardedClose()
        {
            if(RewardedAdsClosed != null)
            {
                RewardedAdsClosed?.Invoke();
                RewardedAdsClosed = null;
            }
        }

        private void OnRewardedError(string errorMessage)
        {
            Debug.LogError($"[{Tag}]: {errorMessage}");

            if(RewardAdsError != null)
            {
                RewardAdsError?.Invoke();
                RewardAdsError = null;
            }
        }

    }
}
