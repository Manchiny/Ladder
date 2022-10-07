using RSG;
using System;
using UnityEngine;

namespace Assets.Scripts.Social.Adverts
{
    public abstract class AbstractAdvertisingAdapter
    {
        private Promise _interstitialPromise;
        private int _showInterstitialAfterLevelCounter;

        private event Action Rewarded;
        private event Action RevardedAdsOpened;
        private event Action RewardedAdsClosed;
        private event Action RewardAdsError;

        public abstract string Tag { get; }
        public bool NeedShowInterstitial => _showInterstitialAfterLevelCounter <= 0;

        ~AbstractAdvertisingAdapter()
        {
            Game.Instance.LevelCompleted -= OnLevelComplete;
        }

        public void Init()
        {
            _showInterstitialAfterLevelCounter = GameConstants.LevelsCountBetweenInterstitialShow;
            Game.Instance.LevelCompleted += OnLevelComplete;
        }

        public Promise TryShowInterstitial(AbstractSocialAdapter social)
        {
            Debug.Log($"[{Tag}] Try show Interstitial... ");

            if (social.IsInited == false || NeedShowInterstitial == false)
            {
                Promise promise = new Promise();
                promise.Resolve();

                Debug.Log($"[{Tag}] Interstitial rejected: social adapetr inite = {social.IsInited}, need show interstitial = {NeedShowInterstitial}, show interstitial counter = {_showInterstitialAfterLevelCounter}");

                return promise;
            }

            if (_interstitialPromise != null && _interstitialPromise.CurState == PromiseState.Pending)
            {
                Debug.Log($"[{Tag}] Interstitia must been already opend");
                return _interstitialPromise;
            }
            else
            {
                Debug.Log($"[{Tag}] start show Interstitial... ");

                _interstitialPromise = new Promise();
                ShowInterstitial(OnInterstitialOpen, OnInterstitilaClose, OnInterstitiaError, OnIntersitialOffline);
                return _interstitialPromise;
            }
        }

        public void TryShowRewardedVideo(AbstractSocialAdapter social, Action onOpen, Action onRewarded, Action onClose, Action onError)
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
            _showInterstitialAfterLevelCounter = GameConstants.LevelsCountBetweenInterstitialShow;
            Debug.LogError($"[{Tag}]: opened interstitial video");
        }

        private void OnIntersitialOffline()
        {
            Debug.LogError($"[{Tag}]: offline");
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

        private void OnLevelComplete()
        {
            _showInterstitialAfterLevelCounter--;
        }
    }
}
