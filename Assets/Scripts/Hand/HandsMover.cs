using System;
using UnityEngine;

public class HandsMover : MonoBehaviour
{
    [SerializeField] private Hand _leftHand;
    [SerializeField] private Hand _rightHand;
    [Space]
    [SerializeField] private Ladder _ladder;

    private Hand _downHand;
    private bool _isFalling;

    public float GetAverageValue => (_leftHand.GetHeight + _rightHand.GetHeight) / 2f;
    private bool _canMove => _leftHand.CanMove && _rightHand.CanMove;

    public event Action Failed;
    public event Action Loosed;


    private void Update()
    {
        if (Input.GetKey(KeyCode.W) == true)
            TryMove();

        if (Input.GetKey(KeyCode.Space) == true && _isFalling)
            TryCatch();
    }

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
        _leftHand.Taked -= OnStepTaked;
        _rightHand.Taked -= OnStepTaked;

        _leftHand.Failed -= OnFail;
        _rightHand.Failed -= OnFail;

        _leftHand.Loosed -= OnLoose;
        _rightHand.Loosed -= OnLoose;
    }

    private void OnStepTaked(LadderStep step)
    {
        _isFalling = false;
        Debug.Log($"Step {step.Id} taked");
    }

    private void OnFail(Hand hand)
    {
        _leftHand.FallDown();
        _rightHand.FallDown();

        _isFalling = true;

        Failed?.Invoke();
    }

    private void OnLoose()
    {
        _isFalling = false;
        Loosed?.Invoke();
    }

    private void TryMove()
    {
        if (_canMove == false)
            return;

        Debug.Log($"Try move to step {GetUpperHand().LastTakedStep.Id}");

        ValidateDownHand();
        _downHand.TryMove(_ladder.NextFreeStep(GetUpperHand().LastTakedStep));
    }

    private void TryCatch()
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
            }
            else if (downStep.CanBeTaked(upperHand.Side) && upperStep.CanBeTaked(_downHand.Side))
            {
                upperHand.ForceTake(downStep);
                _downHand.ForceTake(upperStep);
            }

            ValidateDownHand();
        }
    }

    private void ValidateDownHand() => _downHand = _leftHand.GetHeight < _rightHand.GetHeight ? _leftHand : _rightHand;
    private Hand GetUpperHand() => _downHand == _leftHand ? _rightHand : _leftHand;
}
