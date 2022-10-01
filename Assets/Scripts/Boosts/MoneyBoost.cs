using UnityEngine;

[CreateAssetMenu(fileName = "MoneyBoost", menuName = "Boosts/MoneyBoost")]
public class MoneyBoost : Boost
{
    public override BoostType Type => BoostType.MoneyBoost;

    protected override float Calculate(float baseValue, float value)
    {
        Debug.Log("Money boost value: " + value);
        return baseValue * value;
    }

}
