using System;
using UnityEngine;

public class HandsMover : MonoBehaviour
{
    [SerializeField] private Hand _leftHand;
    [SerializeField] private Hand _rightHand;
    [Space]
    [SerializeField] private Ladder _ladder;

    private Hand _downHand;

    public float GetAverageValue => (_leftHand.GetHeight + _rightHand.GetHeight) / 2f;
    private bool _canMove => _leftHand.CanMove && _rightHand.CanMove;

    public event Action<LadderStep> StepTaked;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) == true)
            TryMove();
    }

    private void OnDisable()
    {
        _leftHand.Taked -= OnStepTaked;
        _rightHand.Taked -= OnStepTaked;
    }

    public void Init(LadderStep firstStep, LadderStep secondStep)
    {

        _rightHand.Init(firstStep);
        _leftHand.Init(secondStep);

        _leftHand.Taked += OnStepTaked;
        _rightHand.Taked += OnStepTaked;

        _downHand = _leftHand.GetHeight < _rightHand.GetHeight ? _leftHand : _rightHand;
    }

    public float GetHeight()
    {
        var hand = _downHand == _leftHand ? _rightHand : _leftHand;
        return hand.transform.position.y;
    }

    private void OnStepTaked(LadderStep step)
    {
        StepTaked?.Invoke(step);
    }

    private void TryMove()
    {
        if (_canMove == false)
            return;

        if(_downHand.TryMove(_ladder.NextFreeStep))
            SetDownHand();
    }

    private void SetDownHand()
    {
        _downHand = _downHand == _leftHand ? _rightHand : _leftHand;
    }
}
