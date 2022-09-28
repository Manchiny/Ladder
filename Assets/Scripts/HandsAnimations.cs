using UnityEngine;

public class HandsAnimations
{
    private const string IdleKey = "Idle";
    private const string ClaspKey = "Clasp";
    private const string ReleaseKey = "Release";

    private const float CrossFadeDuration = 0.1f;

    private Animator _animator;

    public HandsAnimations(Animator animator)
    {
        _animator = animator;
    }

    public void PlayIdle() => PlayAnimation(IdleKey);
    public void PlayClasp() => PlayAnimation(ClaspKey);
    public void PlayRelease() => PlayAnimation(ReleaseKey);

    private void PlayAnimation(string key)
    {
        _animator.StopPlayback();
        _animator.CrossFade(key, CrossFadeDuration);
    }
}
