using UnityEngine;
using static Ladder;

public class LadderStepDynamic : MonoBehaviour
{
    [SerializeField] private LadderStepHalfDynamic _leftPrefab;
    [SerializeField] private LadderStepHalfDynamic _rightPrefab;

    public LadderStepHalfDynamic GetPrefab(LadderSide side)
    {
        if (side == LadderSide.Left)
            return _leftPrefab;
        else
            return _rightPrefab;
    }
}
