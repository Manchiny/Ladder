using UnityEngine;

public class LadderStepHalfDynamic : LadderStep
{
    [SerializeField] private Animator _animator;

    private const string IncriasingAnimationName = "Increasing";

    public override bool CanBeTaked => CheckersCount > 1 || (CheckersCount > 0 && GetPhase() == Phase.Increasing);

    private enum Phase
    {
        Decreasing,
        Increasing
    }

    protected override void UpdateState()
    {
        _animator.speed = CanBeTaked ? 0 : 1;
    }

    private Phase GetPhase()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(IncriasingAnimationName))
            return Phase.Increasing;

        return Phase.Decreasing;
    }
}
