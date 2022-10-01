using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Boost : ScriptableObject
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private List<BoostConfig> _configs;

    public abstract BoostType Type { get; }

    public Sprite Icon => _icon;

    public enum BoostType
    {
        MoneyBoost,
        StaminaBoost
    }

    public int GetNextLevelCost(UserData user)
    {
        int currentLevel = GetBoostLevel(user);

        if (_configs.Count > currentLevel+1)
            return _configs[currentLevel+1].Cost;
        else
            return _configs.Last().Cost;
    }

    public float CalculateEndValue(float baseValue)
    {
        if (GetBoostLevel(Game.User) < 0)
        {
            Debug.Log("Boost lvl = " + GetBoostLevel(Game.User));
            return baseValue;
        }

        BoostConfig config;

        if (_configs.Count > GetBoostLevel(Game.User))
            config = _configs[GetBoostLevel(Game.User)];
        else
            config = _configs.Last();

        float value = config.Value;

        return Calculate(baseValue, value);
    }

    public int GetBoostLevel(UserData user) => user.GetBoostLevel(Type);
    public void Save(Saver saver) => saver.SaveBoostLevel(Type, GetBoostLevel(Game.User));
    protected abstract float Calculate(float baseValue, float value);

}

[Serializable]
public class BoostConfig
{
    [SerializeField] private int _cost;
    [SerializeField] private float _value;

    public int Cost => _cost;
    public float Value => _value;
}
