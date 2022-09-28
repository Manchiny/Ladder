using UnityEngine;

public class LadderStepHalfDynamic : LadderStep
{
    [SerializeField] private Animator _animator;

    public override void OnTaked()
    {
        _animator.speed = 0;
    }

    public override void OnRelease()
    {
        _animator.speed = 1;
    }
}
