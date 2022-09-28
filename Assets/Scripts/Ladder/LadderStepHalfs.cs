using UnityEngine;
using static Ladder;

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
