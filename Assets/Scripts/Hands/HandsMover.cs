using Assets.Scripts.Ladder;
using RSG;
using System;
using System.Collections.Generic;
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

        private IDisposable _moveDispose;
        private bool _isProcess;
        private Promise _catchingPromise;

        public event Action Failed;
        public event Action Loosed;
        public event Action Completed;
        public event Action Catched;
        public event Action<LadderStep, Hand> Taked;
        public event Action Stopped;

        public Hand DownHand { get; private set; }
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

            //if (_catchingPromise != null)
            //    _catchingPromise = null;

            if (Stamina == null)
                Stamina = GetComponent<Stamina>();

            Stamina.Init(this);

            _rightHand.Init(firstStep, LadderSide.Right, Stamina);
            _leftHand.Init(secondStep, LadderSide.Left, Stamina);

            AddSubscribes();

            DownHand = _leftHand.GetHeight < _rightHand.GetHeight ? _leftHand : _rightHand;
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
                    DownHand.TryMove(_ladder.NextFreeStep(GetUpperHand().LastTakedStep));
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

        public bool TryCatch()
        {
            Debug.Log($"Hands mover try catch: IsFalling = {IsFalling}, isProcess = {_isProcess}");

            if (IsFalling == false || _isProcess)
                return false;

            _isProcess = true;

            ValidateDownHand();
            Hand upperHand = GetUpperHand();

            LadderStep downStep = _ladder.GetNearestStep(DownHand.GetHeight, DownHand.LastTakedStep.Id);

            bool result = false;

            if (downStep != null)
            {
                LadderStep upperStep = _ladder.GetStepById(downStep.Id + 1);

                if (upperStep == null)
                {
                    _isProcess = false;
                    return false;
                }

                if (downStep.CanBeTaked(DownHand.Side) && upperStep.CanBeTaked(upperHand.Side))
                {
                    result = true;

                    Catch(upperHand, downStep, upperStep)
                        .Then(() => ValidateDownHand());
                }
                else if (downStep.CanBeTaked(upperHand.Side) && upperStep.CanBeTaked(DownHand.Side))
                {
                    result = true;

                    Catch(upperHand, upperStep, downStep)
                        .Then(() => ValidateDownHand());
                }                
            }

            _isProcess = false;
            return result;
        }

        public void ForceFail(LadderStep step)
        {
            Debug.Log("Force fail: IsFalling: " + IsFalling);
            if (IsFalling)
                return;

            StopMovement();

            float duration = FallingDuration();

            _leftHand.FallDown(duration);
            _rightHand.FallDown(duration);

            Failed?.Invoke();
        }

        public Hand GetAnotherHand(Hand hand) => hand == _rightHand ? _leftHand : _rightHand;

        private Promise Catch(Hand upperHand, LadderStep downHandStep, LadderStep upperHandStep)
        {
            //if (_catchingPromise != null)
            //    return _catchingPromise;

            _catchingPromise = new Promise();
            List<IPromise> sequense = new();

            sequense.Add(DownHand.ForceTake(downHandStep));
            sequense.Add(upperHand.ForceTake(upperHandStep));

            Promise.All(sequense)
                .Then(() =>
                {
                    Catched?.Invoke();
                    _catchingPromise.Resolve();
                });

            return _catchingPromise;
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
            Debug.Log("On fail: IsFalling: " + IsFalling);

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

        private void ValidateDownHand() => DownHand = _leftHand.GetHeight < _rightHand.GetHeight ? _leftHand : _rightHand;
        private Hand GetUpperHand() => DownHand == _leftHand ? _rightHand : _leftHand;
    }
}
