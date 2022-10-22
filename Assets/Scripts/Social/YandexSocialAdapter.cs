using Agava.YandexGames;
using Assets.Scripts.Saves;
using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Social
{
    public class YandexSocialAdapter : AbstractSocialAdapter
    {
        public override string Tag => "[YandexSDK]";
        public override string Name => "Yandex";
        public override Saver GetSaver => new YandexSaver();
        public override bool IsAuthorized => IsInited && PlayerAccount.IsAuthorized;
        public override bool HasPersonalDataPermission => PlayerAccount.HasPersonalProfileDataPermission;

        protected override IEnumerator InitSdk(Action onSuccessCallback)
        {
#if YANDEX_GAMES && UNITY_EDITOR
            yield break;
#elif !UNITY_WEBGL || UNITY_EDITOR
            yield break;
#else
            YandexGamesSdk.CallbackLogging = true;
            yield return YandexGamesSdk.Initialize(onSuccessCallback);
#endif
        }

        public override void ConnectProfileToSocial(Action onSucces, Action<string> onError)
        {
            if (IsInited == false)
            {
                onError?.Invoke($"{Tag}: connect to social failed! SDK not inited;");
                return;
            }

            PlayerAccount.Authorize(onSucces, onError);
        }

        public override void RequestPersonalProfileDataPermission()
        {
            PlayerAccount.RequestPersonalProfileDataPermission();
        }

        public void GetProfileData()
        {
            PlayerAccount.GetProfileData((result) =>
            {
                string name = result.publicName;

                if (string.IsNullOrEmpty(name))
                    name = "Anonymous";

                Debug.Log($"My id = {result.uniqueID}, name = {name}");
            });
        }

        public void OnGetDeviceTypeButtonClick()
        {
            Debug.Log($"DeviceType = {Device.Type}");
        }

        public void OnGetEnvironmentButtonClick()
        {
            Debug.Log($"Environment = {JsonUtility.ToJson(YandexGamesSdk.Environment)}");
        }

        public override Promise<List<LeaderboardData>> GetLeaderboardData(string leaderBoardName)
        {
            Promise<List<LeaderboardData>> promise = new();
            List<LeaderboardData> data = new();

            Leaderboard.GetEntries(leaderBoardName, OnSucces, OnError, LeaderbourdMaxCount, LeaderbourdMaxCount, true);

            void OnSucces(LeaderboardGetEntriesResponse result)
            {
                Debug.Log($"User rank = {result.userRank}");

                foreach (var entry in result.entries)
                {
                    string name = entry.player.publicName;

                    if (name.IsNullOrEmpty())
                        name = "Anonymous";

                    int score = entry.score;

                    data.Add(new LeaderboardData(name, score));
                }

                promise.Resolve(data);
            }

            void OnError(string error)
            {
                Debug.Log(Tag + ": error GetLeaderboardData - " + error);
                promise.Resolve(data);
            }

            return promise;
        }

        protected override void SetLeaderboardValue(string leaderboardName, int value)
        {
            Leaderboard.SetScore(leaderboardName, value, OnSucces, OnError);
            //TryGetLeaderboardPlayerEntry(leaderboardName)
            //     .Then(scores =>
            //     {
            //         if (value > scores)
            //             Leaderboard.SetScore(leaderboardName, value);
            //     })
            //     .Catch(_ =>
            //     {
            //         Leaderboard.SetScore(leaderboardName, value);
            //     });

            void OnSucces()
            {
                Debug.Log($"{Tag}: player's leaderboard data succesfully updated!");
            }

            void OnError(string error)
            {
                Debug.Log($"{Tag}: player's leaderboard data update failed - {error}");
            }
        }

        protected Promise<int> TryGetLeaderboardPlayerEntry(string leaderBoardName)
        {
            Promise<int> promise = new();

            int scores = -1;

            Leaderboard.GetPlayerEntry(leaderBoardName, OnSucces, OnError);

            void OnSucces(LeaderboardEntryResponse responce)
            {
                if (responce == null)
                    promise.Resolve(scores);
                else
                    promise.Resolve(responce.score);
            }

            void OnError(string error)
            {
                promise.Reject(null);
                Debug.Log(Tag + $" can't get Leaderboard player entry: {error}");
            }

            return promise;
        }
    }
}
