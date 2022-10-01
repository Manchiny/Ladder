using System.Collections.Generic;
using UnityEngine;
using static Boost;

public class Saver
{
    private const string CurrentLevelKey = "CurrentLevel";
    private const string MoneyKey = "Money";

    private Dictionary<BoostType, string> _boostsKeys = new Dictionary<BoostType, string>
    {
        { BoostType.MoneyBoost, "StaminaBoostLevel" },
        { BoostType.StaminaBoost, "MoneyBoostLevel" }
    };

    public void Save(UserData data)
    {
        PlayerPrefs.SetInt(CurrentLevelKey, data.CurrentLevelId);
        PlayerPrefs.SetInt(MoneyKey, data.Money);

        foreach (var boost in data.BoostLevels)
            SaveBoostLevel(boost.Key, boost.Value);
    }

    public UserData LoadUserData()
    {
        UserData data = new UserData(GetCurrentLevel(), GetMoney());
        data.WriteBoostsData(GetBoostsData());

        return data;
    }

    public void ResetLevel()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, 0);
    }

    public void SaveBoostLevel(BoostType type, int lvl)
    {
        string key = GetBoostSaveKey(type);

        if (key != null)
            PlayerPrefs.SetInt(key, lvl);
    }

    public int GetCurrentLevel() => PlayerPrefs.GetInt(CurrentLevelKey);
    public int GetMoney() => PlayerPrefs.GetInt(MoneyKey);
    public int GetLastSavedBoostLevel(BoostType type)
    {
        string key = GetBoostSaveKey(type);

        if (key != null)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetInt(key);
        }

        return -1;
    }

    public Dictionary<BoostType, int> GetBoostsData()
    {
        Dictionary<BoostType, int> data = new();

        foreach (var boost in _boostsKeys)
        {
            int levelId = GetBoostLevelBySavedKey(boost.Value);
            data.Add(boost.Key, levelId);
        }

        return data;
    }

    private int GetBoostLevelBySavedKey(string key)
    {
        if (PlayerPrefs.HasKey(key) == false)
            return -1;
        else
            return PlayerPrefs.GetInt(key);
    }

    private string GetBoostSaveKey(BoostType type)
    {
        if (_boostsKeys.ContainsKey(type))
            return _boostsKeys[type];
        else
            Debug.LogError($"There is no save key for {type}!");

        return null;

    }
}

