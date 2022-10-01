using UnityEngine;

[CreateAssetMenu(fileName = "StaminaBoost", menuName = "Boosts/StaminaBoost")]
public class StaminaBoost : Boost
{
    public override BoostType Type => BoostType.StaminaBoost;

    protected override float Calculate(float baseValue, float value)
    {
        return baseValue * value;
    }
}
