using UnityEngine;

public class Saver
{
    private const string CurrentLevelKey = "CurrentLevel";
    private const string MoneyKey = "Money";

    public void Save(UserData data)
    {
        PlayerPrefs.SetInt(CurrentLevelKey, data.CurrentLevelId);
        PlayerPrefs.SetInt(MoneyKey, data.Money);
    }

    public UserData LoadUserData()
    {
        return new UserData(GetCurrentLevel(), GetMoney());
    }

    public void ResetLevel()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, 0);
    }

    public int GetCurrentLevel() => PlayerPrefs.GetInt(CurrentLevelKey);
    public int GetMoney() => PlayerPrefs.GetInt(MoneyKey);
    
}

