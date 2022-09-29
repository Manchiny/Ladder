using System.Collections;
using UnityEngine;

public class HandsAnimations
{
    private const string IdleKey = "Idle";
    private const string ClaspKey = "Clasp";
    private const string ReleaseKey = "Release";

    private const float CrossFadeDuration = 0.1f;
    private const float FailReleaseDelay = 0.2f;

    private Animator _animator;
    private MonoBehaviour _monoBehaviour;

    private Coroutine _animationCoroutine;

    public HandsAnimations(Animator animator, MonoBehaviour monoBehaviour)
    {
        _animator = animator;
        _monoBehaviour = monoBehaviour;
    }

    public void PlayIdle() => PlayAnimation(IdleKey);
    public void PlayClasp() => PlayAnimation(ClaspKey);
    public void PlayRelease() => PlayAnimation(ReleaseKey);

    public void PlayFail()
    {
        StopCoroutine();
        _animationCoroutine = _monoBehaviour.StartCoroutine(PlayFailAnimation());
    }

    private void PlayAnimation(string key)
    {
        _animator.StopPlayback();
        StopCoroutine();
        
        _animator.CrossFade(key, CrossFadeDuration);
    }

    private IEnumerator PlayFailAnimation()
    {
        yield return new WaitForSeconds(FailReleaseDelay);
        PlayRelease();
    }

    private void StopCoroutine()
    {
        if (_animationCoroutine != null)
            _monoBehaviour.StopCoroutine(_animationCoroutine);
    }
}
