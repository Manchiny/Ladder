using DG.Tweening;
using System;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private const float MoveDuration = 0.5f;

    private bool _isProcess;

    private float _needHeight;
    private LadderStep _targetStep;

    private HandsAnimations _animations;

    public bool CanMove => _isProcess == false;
    public float GetHeight => transform.position.y;

    public event Action<LadderStep> Taked;

    public void Init(LadderStep initStep)
    {
        MoveForce(initStep);

        _animations = new HandsAnimations(_animator);
        _animations.PlayIdle();
    }

    public bool TryMove(LadderStep step)
    {
        if (_isProcess)
            return false;

        _needHeight = step.transform.position.y;
        _targetStep = step;

        MoveUp();

        return true;
    }

    private void MoveForce(LadderStep step)
    {
        Vector3 position = transform.position;
        position.y = step.transform.position.y;

        transform.position = position;
    }

    private void MoveUp()
    {
        _isProcess = true;

        _animations.PlayRelease();

        transform.DOMoveY(_needHeight, MoveDuration)
             .SetLink(gameObject)
             .SetEase(Ease.Linear)
             .OnComplete(() =>
             {
                 Taked?.Invoke(_targetStep);

                 _animations.PlayClasp();

                 _isProcess = false;
             });
    }
}
