using UnityEngine;

namespace Assets.Scripts.Boosts
{
    [CreateAssetMenu(fileName = "MoneyBoost", menuName = "Boosts/MoneyBoost")]
    public class MoneyBoost : Boost
    {
        public override BoostType Type => BoostType.MoneyBoost;
        public override string Name => "INCOME";

        protected override float Calculate(float baseValue, float value)
        {
            Debug.Log("Money boost value: " + value);
            return baseValue * value;
        }
    }
}
