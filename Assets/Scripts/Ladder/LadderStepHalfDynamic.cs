using UnityEngine;
using static Ladder;

public class LadderStepHalfDynamic : LadderStep
{
    [SerializeField] private Animator _animator;

    private const string IncriasingAnimationName = "Increasing";

    public override bool CanBeTaked(LadderSide side) => base.CanBeTaked(side) && (CheckersCount > 1 || (CheckersCount > 0 && GetPhase() == Phase.Increasing));

    private enum Phase
    {
        Decreasing,
        Increasing
    }

    protected override void UpdateState(LadderSide side)
    {
        _animator.speed = CanBeTaked(side) ? 0 : 1;
    }

    private Phase GetPhase()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(IncriasingAnimationName))
            return Phase.Increasing;

        return Phase.Decreasing;
    }
}
