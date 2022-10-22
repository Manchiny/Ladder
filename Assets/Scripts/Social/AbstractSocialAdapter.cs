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
    public abstract class AbstractSocialAdapter : MonoBehaviour
    {
        public const string DefaultLeaderBoardName = "LevelValue";
        protected const int LeaderbourdMaxCount = 20;

        private IDisposable _levelIdSubscriprion;

        public abstract string Tag { get; }
        public abstract string Name { get; }
        public bool IsInited { get; private set; }
        public abstract bool IsAuthorized { get; }
        public abstract bool HasPersonalDataPermission { get; }

        public virtual Saver GetSaver => new DefaultSaver();

        private void OnDestroy()
        {
            if (_levelIdSubscriprion != null)
                _levelIdSubscriprion.Dispose();
        }

        public IEnumerator Init()
        {
            Debug.Log($"{Tag} adapter start initializing...");

            DontDestroyOnLoad(gameObject);

            yield return InitSdk(OnSdkInited);
        }

        public void AfterSceneLoaded()
        {
            _levelIdSubscriprion = Game.Instance.CurrentLevelId.ObserveEveryValueChanged(x => x.Value).Subscribe(OnLevelChanged).AddTo(Game.Instance.gameObject);
        }

        public abstract void ConnectProfileToSocial(Action onSucces, Action<string> onError);
        public abstract Promise<List<LeaderboardData>> GetLeaderboardData(string leaderBoardName);
        public abstract void RequestPersonalProfileDataPermission();

        protected abstract IEnumerator InitSdk(Action onSuccessCallback);
        protected abstract void SetLeaderboardValue(string leaderboardName, int value);

        private void OnSdkInited()
        {
            IsInited = true;
            Debug.Log($"{Tag} adapter successful initialized.");
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
