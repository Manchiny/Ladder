using Agava.YandexGames;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Social
{
    public class YandexSocialAdapter : AbstractSocialAdapter
    {
        [SerializeField]
        private Text _authorizationStatusText;

        [SerializeField]
        private Text _personalProfileDataPermissionStatusText;

        [SerializeField]
        private InputField _playerDataTextField;

        public override string Tag => "YandexSDK";

        public override void InitSdk(Action onSuccessCallback)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return;
#endif
            YandexGamesSdk.Initialize(onSuccessCallback);
        }

        public void OnAuthorizeButtonClick()
        {
            PlayerAccount.Authorize();
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

        public void OnSetLeaderboardScoreButtonClick()
        {
            Leaderboard.SetScore("PlaytestBoard", UnityEngine.Random.Range(1, 100));
        }

        public void OnGetLeaderboardEntriesButtonClick()
        {
            Leaderboard.GetEntries("PlaytestBoard", (result) =>
            {
                Debug.Log($"My rank = {result.userRank}");
                foreach (var entry in result.entries)
                {
                    string name = entry.player.publicName;
                    if (string.IsNullOrEmpty(name))
                        name = "Anonymous";
                    Debug.Log(name + " " + entry.score);
                }
            });
        }

        public void OnGetLeaderboardPlayerEntryButtonClick()
        {
            Leaderboard.GetPlayerEntry("PlaytestBoard", (result) =>
            {
                if (result == null)
                    Debug.Log("Player is not present in the leaderboard.");
                else
                    Debug.Log($"My rank = {result.rank}, score = {result.score}");
            });
        }

        public void OnSetPlayerDataButtonClick()
        {
            PlayerAccount.SetPlayerData(_playerDataTextField.text);
        }

        public void OnGetPlayerDataButtonClick()
        {
            PlayerAccount.GetPlayerData((data) => _playerDataTextField.text = data);
        }

        public void OnGetDeviceTypeButtonClick()
        {
            Debug.Log($"DeviceType = {Device.Type}");
        }
        public void OnGetEnvironmentButtonClick()
        {
            Debug.Log($"Environment = {JsonUtility.ToJson(YandexGamesSdk.Environment)}");
        }
    }
}
