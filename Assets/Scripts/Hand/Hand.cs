using DG.Tweening;
using System;
using UnityEngine;
using static Ladder;

public class Hand : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private const float MoveUpDuration = 0.5f;
    private const float ForceMoveStepDuration = 0.2f;

    private bool _isProcess;
    private Tween _fallingTween;

    private LadderStep _targetStep;
    private HandsAnimations _animations;

    private Vector3 _defaultPosition;

    public LadderSide Side { get; private set; }
    public bool IsFalling { get; private set; }
    public LadderStep LastTakedStep { get; private set; }
    public bool CanMove => _isProcess == false && IsFalling == false;
    public float GetHeight => transform.position.y;

    public event Action<LadderStep> Taked;
    public event Action<Hand> Failed;
    public event Action Loosed;

    private void Start()
    {
        _defaultPosition = transform.localPosition;
    }

    public void Init(LadderStep initStep, LadderSide side)
    {
        transform.localPosition = _defaultPosition;

        Side = side;

        _animations = new HandsAnimations(_animator, this);
        ForceTake(initStep);

        IsFalling = false;
    }

    public void FallDown()
    {
        if (IsFalling)
            _animations.PlayFail();
        else
            _animations.PlayRelease();

        float duration = (_targetStep.Height / GameConstants.LadderDeltaStep) * ForceMoveStepDuration;

        _fallingTween = transform.DOMoveY(0, duration)
                                 .SetLink(gameObject)
                                 .SetEase(Ease.Linear)
                                 .OnComplete(() => Loosed?.Invoke());
    }

    public bool TryMove(LadderStep step)
    {
        if (CanMove == false || step == null)
            return false;

        _targetStep = step;

        if (step is FinishButtonStep)
            PressFinishButton(step);
        else
            MoveUpStep(step);

        return true;
    }

    public bool ForceTake(LadderStep step)
    {
        if (_isProcess)
            return false;

        _isProcess = true;

        _animations.PlayClasp();

        _targetStep = step;
        _fallingTween.Kill();

        transform.DOMoveY(step.Height, ForceMoveStepDuration)
             .SetLink(gameObject)
             .SetEase(Ease.Linear)
             .OnComplete(() => Take(step));

        return true;
    }

    private void MoveUpStep(LadderStep step)
    {
        _isProcess = true;
        _animations.PlayRelease();

        transform.DOMoveY(step.Height, MoveUpDuration)
             .SetLink(gameObject)
             .SetEase(Ease.Linear)
             .OnComplete(() =>
             {
                 TryTake(step);
                 _isProcess = false;
             });
    }

    private bool TryTake(LadderStep step)
    {
        _animations.PlayClasp();

        if (step.CanBeTaked(Side))
        {
            Take(step);
            return true;
        }
        else
        {
            IsFalling = true;
            Failed?.Invoke(this);

            return false;
        }
    }

    private void PressFinishButton(LadderStep step)
    {
        _isProcess = true;
        _animations.PlayRelease();

        transform.DOMove(step.transform.position, MoveUpDuration)
             .SetLink(gameObject)
             .SetEase(Ease.Linear)
             .OnComplete(() =>
             {
                 Take(step);
             });
    }

    private void Take(LadderStep step)
    {
        LastTakedStep = step;
        IsFalling = false;
        _isProcess = false;

        Taked?.Invoke(step);
    }
}
