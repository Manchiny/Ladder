using Agava.YandexGames;
using Assets.Scripts.Saves;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Social
{
    public class YandexSocialAdapter : AbstractSocialAdapter
    {
        public override string Tag => "[YandexSDK]";
        public override string Name => "Yandex";
        public override Saver GetSaver => new YandexSaver();

        protected override void InitSdk(Action onSuccessCallback)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return;
#endif
            YandexGamesSdk.Initialize(onSuccessCallback);
        }

        public override void ConnectProfileToSocial(Action onSucces, Action<string> onError)
        {
            if(IsInited == false)
            {
                onError?.Invoke($"{Tag}: connect to social failed! SDK not inited;");
                return;
            }

            PlayerAccount.Authorize(onSucces, onError);
        }

        public void OnRequestPersonalProfileDataPermissionButtonClick()
        {
            PlayerAccount.RequestPersonalProfileDataPermission();
        }

        public void OnGetProfileDataButtonClick()
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

        public override bool IsAuthorized() => PlayerAccount.IsAuthorized;

        protected override void SetLeaderboardValue(string leaderboardName, int value)
        {
            Leaderboard.SetScore(leaderboardName, value);
        }

        public override List<LeaderboardData> GetLeaderboardData()
        {
            List<LeaderboardData> data = new();

            Leaderboard.GetEntries(LeaderBoardName, OnSucces, OnError, LeaderbourdMaxCount, LeaderbourdMaxCount, true);

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
            }

            void OnError(string error)
            {
                Debug.Log(Tag + ": error GetLeaderboardData - " + error);
            }

            return data;
        }

        protected void GetLeaderboardPlayerEntry()
        {
            Leaderboard.GetPlayerEntry("PlaytestBoard", (result) =>
            {
                if (result == null)
                    Debug.Log("Player is not present in the leaderboard.");
                else
                    Debug.Log($"My rank = {result.rank}, score = {result.score}");
            });
        }
    }
}
