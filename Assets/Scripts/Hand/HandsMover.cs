using System;
using UniRx;
using UnityEngine;

public class HandsMover : MonoBehaviour
{
    [SerializeField] private Hand _leftHand;
    [SerializeField] private Hand _rightHand;
    [Space]
    [SerializeField] private Ladder _ladder;

    private Hand _downHand;
    private bool _isFalling;

    private IDisposable _moveDispose;

    public float GetAverageValue => (_leftHand.GetHeight + _rightHand.GetHeight) / 2f;
    private bool _canMove => _leftHand.CanMove && _rightHand.CanMove;

    public event Action Failed;
    public event Action Loosed;
    public event Action Completed;
    public event Action Catched;
    public event Action<LadderStep, Hand> Taked;

    private void OnDisable()
    {
        RemoveSubscribes();
    }

    public void Init(LadderStep firstStep, LadderStep secondStep, bool isReinit)
    {
        if (isReinit)
            RemoveSubscribes();

        _isFalling = false;

        _rightHand.Init(firstStep, Ladder.LadderSide.Right);
        _leftHand.Init(secondStep, Ladder.LadderSide.Left);

        AddSubscribes();

        _downHand = _leftHand.GetHeight < _rightHand.GetHeight ? _leftHand : _rightHand;
    }

    public void TryMove()
    {
        if (_moveDispose == null)
        {
            _moveDispose = Observable.EveryUpdate().Subscribe(_ =>
            {
                if (_canMove == false)
                    return;

                ValidateDownHand();
                _downHand.TryMove(_ladder.NextFreeStep(GetUpperHand().LastTakedStep));
            });
        }
    }

    public void StopMovement()
    {
        if (_moveDispose != null)
        {
            _moveDispose.Dispose();
            _moveDispose = null;
        }
    }

    public void TryCatch()
    {
        if (_isFalling == false)
            return;

        ValidateDownHand();
        LadderStep downStep = _ladder.GetNearestStep(_downHand.GetHeight, _downHand.LastTakedStep.Id);

        Hand upperHand = GetUpperHand();

        if (downStep != null)
        {
            LadderStep upperStep = _ladder.GetStepById(downStep.Id + 1);

            if (downStep.CanBeTaked(_downHand.Side) && upperStep.CanBeTaked(upperHand.Side))
            {
                _downHand.ForceTake(downStep);
                upperHand.ForceTake(upperStep);
                Catched?.Invoke();
            }
            else if (downStep.CanBeTaked(upperHand.Side) && upperStep.CanBeTaked(_downHand.Side))
            {
                upperHand.ForceTake(downStep);
                _downHand.ForceTake(upperStep);
            }

            ValidateDownHand();
        }
    }
    private void AddSubscribes()
    {
        _leftHand.Taked += OnStepTaked;
        _rightHand.Taked += OnStepTaked;

        _leftHand.Failed += OnFail;
        _rightHand.Failed += OnFail;

        _leftHand.Loosed += OnLoose;
        _rightHand.Loosed += OnLoose;
    }

    private void RemoveSubscribes()
    {
        StopMovement();

        _leftHand.Taked -= OnStepTaked;
        _rightHand.Taked -= OnStepTaked;

        _leftHand.Failed -= OnFail;
        _rightHand.Failed -= OnFail;

        _leftHand.Loosed -= OnLoose;
        _rightHand.Loosed -= OnLoose;
    }

    private void OnStepTaked(LadderStep step, Hand hand)
    {
        _isFalling = false;
        Debug.Log($"Step {step.Id} taked");

        if (step is FinishButtonStep)
        {
            StopMovement();
            Completed?.Invoke();
        }
        else
            Taked?.Invoke(step, hand);
    }

    private void OnFail(Hand hand)
    {
        StopMovement();

        _leftHand.FallDown();
        _rightHand.FallDown();

        _isFalling = true;

        Failed?.Invoke();
    }

    private void OnLoose()
    {
        _isFalling = false;
        StopMovement();

        Loosed?.Invoke();
    }

    private void ValidateDownHand() => _downHand = _leftHand.GetHeight < _rightHand.GetHeight ? _leftHand : _rightHand;
    private Hand GetUpperHand() => _downHand == _leftHand ? _rightHand : _leftHand;
}
