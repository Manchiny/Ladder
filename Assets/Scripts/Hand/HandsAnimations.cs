using DG.Tweening;
using RSG;
using UnityEngine;

public class HandsAnimations
{
    private const string IdleKey = "Idle";
    private const string ClaspKey = "Clasp";
    private const string ReleaseKey = "Release";

    private const float CrossFadeDuration = 0.1f;

    private const float ReturnHandOnFailDuration = 0.18f;
    private const float DeltaHandsHeighOnFail = 0.15f;

    private Animator _animator;
   
    public HandsAnimations(Animator animator)
    {
        _animator = animator;
    }

    public void PlayIdle() => PlayAnimation(IdleKey);
    public void PlayClasp() => PlayAnimation(ClaspKey);
    public void PlayRelease() => PlayAnimation(ReleaseKey);

    public IPromise PlayFail(Transform hand) => ReturnHand(hand);

    private void PlayAnimation(string key)
    {
        _animator.StopPlayback();
        _animator.CrossFade(key, CrossFadeDuration);
    }

    private Promise ReturnHand(Transform hand)
    {
        Promise promise = new Promise();

        float positionY = hand.position.y - GameConstants.LadderDeltaStep + DeltaHandsHeighOnFail;

        hand.DOMoveY(positionY, ReturnHandOnFailDuration)
            .SetEase(Ease.InQuad)
            .SetLink(hand.gameObject)
            .OnComplete(() => promise.Resolve());

        return promise;
    }
}
