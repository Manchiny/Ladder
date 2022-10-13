#pragma warning disable

using Assets.Scripts.Saves;
using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.Social
{
    public abstract class AbstractSocialAdapter
    {
        public const string DefaultLeaderBoardName = "LevelValue";
        protected const int LeaderbourdMaxCount = 20;

        private Promise _initPromise;
        private IDisposable _levelIdSubscriprion;

        public abstract string Tag { get; }
        public abstract string Name { get; }
        public bool IsInited { get; private set; }
        public abstract bool IsAuthorized { get; }

        public virtual Saver GetSaver => new DefaultSaver();

        ~AbstractSocialAdapter()
        {
            if (_levelIdSubscriprion != null)
                _levelIdSubscriprion.Dispose();
        }

        public Promise Init()
        {
            if (_initPromise != null && _initPromise.CurState == PromiseState.Pending)
                return _initPromise;

            Debug.Log($"{Tag} adapter start initializing...");

            _initPromise = new Promise();

            InitSdk(OnSdkInited);

            return _initPromise;
        }

        public abstract void ConnectProfileToSocial(Action onSucces, Action<string> onError);
        public abstract List<LeaderboardData> GetLeaderboardData(string leaderBoardName);
        public abstract void RequestPersonalProfileDataPermission();

        protected abstract void InitSdk(Action onSuccessCallback);
        protected abstract void SetLeaderboardValue(string leaderboardName, int value);

        private void OnSdkInited()
        {
            IsInited = true;
            Debug.Log($"{Tag} adapter successful initialized.");

            _levelIdSubscriprion = Game.Instance.CurrentLevelId.ObserveEveryValueChanged(x => x.Value).Subscribe(OnLevelChanged).AddTo(Game.Instance.gameObject);

            _initPromise.ResolveOnce();
        }

        private void OnLevelChanged(int level)
        {
            SetLeaderboardValue(DefaultLeaderBoardName, level + 1);
        }

        public class LeaderboardData
        {
            public readonly string UserName;
            public readonly int ScoreValue;

            public LeaderboardData(string name, int value)
            {
                UserName = name;
                ScoreValue = value;
            }
        }
    }
}
