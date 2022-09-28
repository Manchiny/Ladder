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

    public override void OnHandCollided(HandChecker checker)
    {
        base.OnHandCollided(checker);

        if (CanBeTaked)
            _animator.speed = 0;
    }

    public override void OnHandExit(HandChecker checker)
    {
        base.OnHandExit(checker);

        if(CanBeTaked == false)
             _animator.speed = 1;
    }

    private Phase GetPhase()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(IncriasingAnimationName))
            return Phase.Increasing;

        return Phase.Decreasing;
    }
}
