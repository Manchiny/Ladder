using RSG;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Boosts.Boost;

namespace Assets.Scripts.Saves
{
    public class DefaultSaver : Saver
    {
        private const string CurrentLevelKey = "CurrentLevel";
        private const string MoneyKey = "Money";
        private const string LocaleKey = "Locale";
        private const string SoundKey = "Sound";

        private Dictionary<BoostType, string> _boostsKeys = new Dictionary<BoostType, string>
        {
            { BoostType.MoneyBoost, "StaminaBoostLevel" },
            { BoostType.StaminaBoost, "MoneyBoostLevel" }
        };

        public override string Tag => "[DefaultSaver]";

        public override void RemoveAllData()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Player prefs removed!");
        }

        protected override Promise<SavingData> LoadData()
        {
            Promise<SavingData> promise = new();

            SavingData data = new SavingData();

            data.Money = GetMoney();
            data.CurrentLevelId = GetCurrentLevel();
            data.BoostsData = GetBoostsData();
            data.SavedLocale = GetSavedLocale();
            data.NeedSound = GetNeedSound();

            promise.Resolve(data);
            return promise;
        }

        protected override void WriteData(SavingData savingData)
        {
            PlayerPrefs.SetInt(CurrentLevelKey, savingData.CurrentLevelId);
            PlayerPrefs.SetInt(MoneyKey, savingData.Money);
            PlayerPrefs.SetInt(SoundKey, BoolToInt(savingData.NeedSound));

            if (savingData.SavedLocale.IsNullOrEmpty() == false)
                PlayerPrefs.SetString(LocaleKey, savingData.SavedLocale);

            foreach (var boost in savingData.BoostsData)
                SaveBoostLevel(boost.Type, boost.Level);
        }

        private List<BoostData> GetBoostsData()
        {
            List<BoostData> data = new();

            foreach (var boost in _boostsKeys)
            {
                int levelId = GetBoostLevelBySavedKey(boost.Value);
                data.Add(new BoostData(boost.Key, levelId));
            }

            return data;
        }

        private void SaveBoostLevel(BoostType type, int lvl)
        {
            string key = GetBoostSaveKey(type);

            if (key != null)
                PlayerPrefs.SetInt(key, lvl);
        }

        private string GetBoostSaveKey(BoostType type)
        {
            if (_boostsKeys.ContainsKey(type))
                return _boostsKeys[type];
            else
                Debug.LogError($"There is no save key for {type}!");

            return null;
        }

        private int GetBoostLevelBySavedKey(string key)
        {
            if (PlayerPrefs.HasKey(key) == false)
                return -1;
            else
                return PlayerPrefs.GetInt(key);
        }

        private bool GetNeedSound()
        {
            if (PlayerPrefs.HasKey(SoundKey))
                return IntToBool(PlayerPrefs.GetInt(SoundKey));

            return true;
        }

        private int GetCurrentLevel() => PlayerPrefs.GetInt(CurrentLevelKey);
        private int GetMoney() => PlayerPrefs.GetInt(MoneyKey);
        private string GetSavedLocale() => PlayerPrefs.HasKey(LocaleKey) ? PlayerPrefs.GetString(LocaleKey) : null;

        private int BoolToInt(bool value) => value == true ? 1 : 0;
        private bool IntToBool(int value) => value == 1 ? true : false;
    }
}

