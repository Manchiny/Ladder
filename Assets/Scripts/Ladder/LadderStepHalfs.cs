using UnityEngine;
using static Assets.Scripts.Ladder.Ladder;

namespace Assets.Scripts.Ladder
{
    public class LadderStepHalfs : LadderStep
    {
        [SerializeField] private LadderStep _leftPrefab;
        [SerializeField] private LadderStep _rightPrefab;

        public override LadderStep GetPrefab(LadderSide side)
        {
            if (side == LadderSide.Left)
                return _leftPrefab;
            else
                return _rightPrefab;
        }
    }
}
