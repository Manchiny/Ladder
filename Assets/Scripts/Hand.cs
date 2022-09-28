using DG.Tweening;
using System;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private const float MoveUpDuration = 0.5f;
    private const float MoveDownStepDuration = 0.2f;

    private bool _isProcess;

    private float _needHeight;
    private LadderStep _targetStep;

    private HandsAnimations _animations;

    public bool CanMove => _isProcess == false && IsFailed == false;
    public bool IsFailed { get; private set; }
    public float GetHeight => transform.position.y;

    public event Action<LadderStep> Taked;
    public event Action<Hand> Failed;

    public void Init(LadderStep initStep)
    {
        MoveForce(initStep);

        _animations = new HandsAnimations(_animator, this);
        _animations.PlayIdle();

        IsFailed = false;
    }

    public void MoveDown()
    {
        if (_needHeight != 0)
        {
            _animations.PlayFail();

            float duration = (_needHeight / GameConstants.LadderDeltaStep) * MoveDownStepDuration;

            transform.DOMoveY(0, duration)
                 .SetLink(gameObject)
                 .SetEase(Ease.Linear);
        }
    }

    public bool TryMove(LadderStep step)
    {
        if (CanMove == false || step == null)
            return false;

        _needHeight = step.transform.position.y;
        _targetStep = step;

        if (step is FinishButtonStep)
            PressFinishButton();
        else
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

        transform.DOMoveY(_needHeight, MoveUpDuration)
             .SetLink(gameObject)
             .SetEase(Ease.Linear)
             .OnComplete(() =>
             {
                 TryTake(_targetStep);
             });
    }

    private void TryTake(LadderStep step)
    {
        _animations.PlayClasp();

        if (step.CanBeTaked)
        {
            Taked?.Invoke(_targetStep);
        }
        else
        {
            IsFailed = true;
            Failed?.Invoke(this);
        }

        _isProcess = false;
    }

    private void PressFinishButton()
    {
        _isProcess = true;
        _animations.PlayRelease();

        transform.DOMove(_targetStep.transform.position, MoveUpDuration)
             .SetLink(gameObject)
             .SetEase(Ease.Linear)
             .OnComplete(() =>
             {
                 Taked?.Invoke(_targetStep);
                 _isProcess = false;
             });
    }
}
