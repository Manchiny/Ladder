using UnityEngine;

namespace Assets.Scripts.Boosts
{
    [CreateAssetMenu(fileName = "StaminaBoost", menuName = "Boosts/StaminaBoost")]
    public class StaminaBoost : Boost
    {
        public override BoostType Type => BoostType.StaminaBoost;
        public override string Name => "STAMINA";

        protected override float Calculate(float baseValue, float value)
        {
            if (value <= 0)
            {
                Debug.LogError($"Stamina boost value must be > 0! Stamina boost level = {GetBoostLevel(Game.User)}");
                return baseValue;
            }

            return baseValue / value;
        }
    }
}
