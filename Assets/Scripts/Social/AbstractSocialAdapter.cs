#pragma warning disable

using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Social
{
    public abstract class AbstractSocialAdapter
    {
        private Promise _initPromise;

        public abstract string Tag { get; }
        public abstract string Name { get; }
        public bool IsInited { get; private set; }

        public Promise Init()
        {
            if (_initPromise != null && _initPromise.CurState == PromiseState.Pending)
                return _initPromise;

            Debug.Log($"[{Tag}] adapter start initializing...");

            _initPromise = new Promise();

            InitSdk(OnSdkInited);

            return _initPromise;
        }

        protected abstract void InitSdk(Action onSuccessCallback);
        public abstract void ConnectProfileToSocial(Action onSucces, Action<string> onError);
        public abstract bool IsAuthorized();

        private void OnSdkInited()
        {
            IsInited = true;
            Debug.Log($"[{Tag}] adapter successful initialized.");

            _initPromise.Resolve();
        }
    }
}
