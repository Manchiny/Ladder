using System;

public class UserData
{
    public int CurrentLevelId { get; private set; }
    public int Money { get; private set; }

    public UserData(int levelId, int money)
    {
        CurrentLevelId = levelId;
        Money = money;
    }

    public void AddMoney(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException("Incorrect money to add count");

        Money += count;
    }

    public void SetCurrentLevelId(LevelConfiguration level)
    {
        CurrentLevelId = level.Id;
    }
}
