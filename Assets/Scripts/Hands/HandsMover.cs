using Assets.Scripts.Ladder;
using RSG;
using System;
using UniRx;
using UnityEngine;
using static Assets.Scripts.Ladder.Ladder;

namespace Assets.Scripts.Hands
{
    [RequireComponent(typeof(Stamina))]
    public class HandsMover : MonoBehaviour
    {
        [SerializeField] private Hand _leftHand;
        [SerializeField] private Hand _rightHand;
        [Space]
        [SerializeField] private Ladder.Ladder _ladder;

        private const float FallingPerStepDuration = 0.2f;

        private Hand _downHand;

        private IDisposable _moveDispose;
        private bool _isProcess;

        public event Action Failed;
        public event Action Loosed;
        public event Action Completed;
        public event Action Catched;
        public event Action<LadderStep, Hand> Taked;
        public event Action Stopped;

        public Stamina Stamina { get; private set; }
        public bool IsFalling => _leftHand.IsFalling || _rightHand.IsFalling;

        public float GetAverageValue => (_leftHand.GetHeight + _rightHand.GetHeight) / 2f;
        public bool CanMove => _leftHand.CanMove && _rightHand.CanMove;


        private void OnDisable()
        {
            RemoveSubscribes();
        }

        public void Init(LadderStep firstStep, LadderStep secondStep, bool isReinit)
        {
            if (isReinit)
                RemoveSubscribes();

            if (Stamina == null)
                Stamina = GetComponent<Stamina>();

            Stamina.Init(this);

            _rightHand.Init(firstStep, LadderSide.Right, Stamina);
            _leftHand.Init(secondStep, LadderSide.Left, Stamina);

            AddSubscribes();

            _downHand = _leftHand.GetHeight < _rightHand.GetHeight ? _leftHand : _rightHand;
        }

        public void TryMove()
        {
            if (_moveDispose == null)
            {
                _moveDispose = Observable.EveryUpdate().Subscribe(_ =>
                {
                    if (CanMove == false)
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

            Stopped?.Invoke();
        }

        public void TryCatch()
        {
            if (IsFalling == false || _isProcess)
                return;

            _isProcess = true;

            ValidateDownHand();
            Hand upperHand = GetUpperHand();

            LadderStep downStep = _ladder.GetNearestStep(_downHand.GetHeight, _downHand.LastTakedStep.Id);

            if (downStep != null)
            {
                LadderStep upperStep = _ladder.GetStepById(downStep.Id + 1);

                if (upperStep == null)
                {
                    _isProcess = false;
                    return;
                }

                if (downStep.CanBeTaked(_downHand.Side) && upperStep.CanBeTaked(upperHand.Side))
                    Catch(upperHand, downStep, upperStep);
                else if (downStep.CanBeTaked(upperHand.Side) && upperStep.CanBeTaked(_downHand.Side))
                    Catch(upperHand, upperStep, downStep);

                ValidateDownHand();
            }

            _isProcess = false;
        }

        public void ForceFail(LadderStep step)
        {
            if (IsFalling)
                return;

            StopMovement();

            float duration = FallingDuration();

            _leftHand.FallDown(duration);
            _rightHand.FallDown(duration);

            Failed?.Invoke();
        }

        private void Catch(Hand upperHand, LadderStep downHandStep, LadderStep upperHandStep)
        {
            _downHand.ForceTake(downHandStep);
            upperHand.ForceTake(upperHandStep);

            Catched?.Invoke();
        }

        private void AddSubscribes()
        {
            _leftHand.Taked += OnStepTaked;
            _rightHand.Taked += OnStepTaked;

            _leftHand.Failed += OnFail;
            _rightHand.Failed += OnFail;

            _leftHand.Loosed += OnLoose;
            _rightHand.Loosed += OnLoose;

            Stamina.EnergyOvered += OnEnergyOver;
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

            Stamina.EnergyOvered -= OnEnergyOver;
        }

        private void OnStepTaked(LadderStep step, Hand hand)
        {
            if (step is FinishButtonStep)
            {
                StopMovement();
                Completed?.Invoke();
            }
            else
                Taked?.Invoke(step, hand);

            step.Hand = hand;
            Debug.Log($"Step {step.Id} taked");
        }

        private float FallingDuration() => (GetAverageValue / GameConstants.LadderDeltaStep) * FallingPerStepDuration;

        private void OnFail(Hand hand)
        {
            StopMovement();

            IPromise failAnimation = new Promise();

            if (hand.Side == LadderSide.Left)
                failAnimation = _leftHand.PlayFailAnimation();
            else
                failAnimation = _rightHand.PlayFailAnimation();

            float duration = FallingDuration();

            failAnimation
                .Then(() =>
                {
                    _leftHand.FallDown(duration);
                    _rightHand.FallDown(duration);

                    Failed?.Invoke();
                });
        }

        private void OnEnergyOver()
        {
            if (IsFalling == false && CanMove)
                ForceFail(null);
        }

        private void OnLoose()
        {
            StopMovement();
            Loosed?.Invoke();
        }

        private void ValidateDownHand() => _downHand = _leftHand.GetHeight < _rightHand.GetHeight ? _leftHand : _rightHand;
        private Hand GetUpperHand() => _downHand == _leftHand ? _rightHand : _leftHand;
    }
}
