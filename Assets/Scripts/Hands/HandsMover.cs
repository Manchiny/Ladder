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
        private LadderStep _lastFailStep;

        private IDisposable _moveDispose;


        public event Action Failed;
        public event Action Loosed;
        public event Action Completed;
        public event Action Catched;
        public event Action<LadderStep, Hand> Taked;
        public event Action Stopped;

        public Stamina Stamina { get; private set; }
        public bool IsFalling { get; private set; }

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

            IsFalling = false;
            _lastFailStep = null;

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
            if (IsFalling == false)
                return;

            ValidateDownHand();
            LadderStep downStep = _ladder.GetNearestStep(_downHand.GetHeight, _downHand.LastTakedStep.Id);

            Hand upperHand = GetUpperHand();

            if (downStep != null)
            {
                LadderStep upperStep = _ladder.GetStepById(downStep.Id + 1);

                if (downStep.CanBeTaked(_downHand.Side) && upperStep.CanBeTaked(upperHand.Side))
                {
                    _lastFailStep = null;
                    _downHand.ForceTake(downStep);
                    upperHand.ForceTake(upperStep);
                    Catched?.Invoke();
                }
                else if (downStep.CanBeTaked(upperHand.Side) && upperStep.CanBeTaked(_downHand.Side))
                {
                    _lastFailStep = null;
                    upperHand.ForceTake(downStep);
                    _downHand.ForceTake(upperStep);
                }

                ValidateDownHand();
            }
        }

        public void ForceFail(LadderStep step)
        {
            //if (step !=null && _lastFailStep != null && _lastFailStep == step)
            //    return;

            if (IsFalling)
                return;

            _lastFailStep = step;

            StopMovement();
            IsFalling = true;

            float duration = FallingDuration();

            _leftHand.FallDown(duration);
            _rightHand.FallDown(duration);

            Failed?.Invoke();
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
            IsFalling = false;
            Debug.Log($"Step {step.Id} taked");
            _lastFailStep = null;

            if (step is FinishButtonStep)
            {
                StopMovement();
                Completed?.Invoke();
            }
            else
                Taked?.Invoke(step, hand);

            step.Hand = hand;
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

            IsFalling = true;
        }

        private void OnEnergyOver()
        {
            if (IsFalling == false && CanMove)
                ForceFail(null);
        }

        private void OnLoose()
        {
            IsFalling = false;
            StopMovement();

            Loosed?.Invoke();
        }

        private void ValidateDownHand() => _downHand = _leftHand.GetHeight < _rightHand.GetHeight ? _leftHand : _rightHand;
        private Hand GetUpperHand() => _downHand == _leftHand ? _rightHand : _leftHand;
    }
}
