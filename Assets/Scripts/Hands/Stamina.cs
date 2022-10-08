using Assets.Scripts.Boosts;
using System;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.Hands
{
    public class Stamina : MonoBehaviour
    {
        public const float MaxEnergyValue = 20f;
        public const float LowEnergyPercent = 40f;
        public const float MaxFactorToFail = 0.99f;

        public const float RecoveryPerSecond = 3.5f;
        public const float BaseEnergyСonsumptionPerStep = 2f;

        private HandsMover _hands;

        private bool _inited;
        private bool _touched;

        public event Action EnergyOvered;

        public ReactiveProperty<float> CurrentEnergy { get; private set; }

        private void Update()
        {
            if (_inited == false || _touched)
                return;

            if (_hands.CanMove || _hands.IsFalling)
                RecoverEnergy(RecoveryPerSecond * Time.deltaTime);
        }

        private void OnDisable()
        {
            Game.UserInput.Touched -= SetTouchedTrue;
            Game.UserInput.Untouched -= SetTouchedFalse;
        }

        public void Init(HandsMover hands)
        {
            if (_inited == false)
            {

                _hands = hands;
                CurrentEnergy = new();

                Game.UserInput.Touched += SetTouchedTrue;
                Game.UserInput.Untouched += SetTouchedFalse;

                _inited = true;
            }

            CurrentEnergy.Value = MaxEnergyValue;
        }

        /// <summary>
        /// В диапозоне от 0 до 1 возвращает степень усталости. Где 0 - только начали уставать, 1 - максимальная усталость.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public bool IsLowEnergy(out float factor)
        {
            float valuePercent = CurrentEnergy.Value * 100f / MaxEnergyValue;
            factor = 0;

            if (valuePercent <= LowEnergyPercent)
            {
                factor = 1 - valuePercent / LowEnergyPercent;
                return true;
            }

            return false;
        }

        public void ForceExpendEnergy(float value)
        {
            float energyCount = Game.Player.CalculateEndValueWithBoosts<StaminaBoost>(value);
            ExpandEnergy(energyCount);
        }

        public void ForceExpendEnergyForMove()
        {
            float energyCount = Game.Player.CalculateEndValueWithBoosts<StaminaBoost>(BaseEnergyСonsumptionPerStep);
            ExpandEnergy(energyCount);
        }

        private void SetTouchedTrue() => _touched = true;
        private void SetTouchedFalse() => _touched = false;

        private void RecoverEnergy(float energyCount)
        {
            if (energyCount == MaxEnergyValue)
                return;

            if (CurrentEnergy.Value + energyCount < MaxEnergyValue)
                CurrentEnergy.Value += energyCount;
            else
                CurrentEnergy.Value = MaxEnergyValue;
        }

        private void ExpandEnergy(float energyCount)
        {
            if (energyCount <= 0)
                return;

            if (CurrentEnergy.Value - energyCount >= 0)
                CurrentEnergy.Value -= energyCount;
            else
            {
                CurrentEnergy.Value = 0;
                EnergyOvered?.Invoke();
            }
        }
    }
}
