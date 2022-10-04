using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Boost : ScriptableObject
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private List<BoostConfig> _configs;
    [SerializeField] private Color _viewColor;
    [SerializeField] private ParticleSystem _buyEffect;

    public abstract BoostType Type { get; }
    public abstract string Name { get; }

    public Sprite Icon => _icon;
    public Color ViewColor => _viewColor;
    public ParticleSystem BuyEffect => _buyEffect;
    public bool NeedShow => HasBoostLevel(GetBoostLevel(Game.User)) && Game.CurrentLevelId >= GameConstants.MinLevelToShowBoostsShop;

    public enum BoostType
    {
        MoneyBoost,
        StaminaBoost
    }

    public bool HasBoostLevel(int levelId) => _configs.Count > levelId;

    public bool TryGetNextLevelCost(out int cost)
    {
        int currentLevel = GetBoostLevel(Game.User);
        cost = 0;

        if (HasBoostLevel(currentLevel + 1))
        {
            cost = _configs[currentLevel + 1].Cost;
            return true;
        }
        else
            return false;
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
