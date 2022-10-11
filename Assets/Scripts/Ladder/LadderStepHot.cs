using Assets.Scripts.Hands;
using System;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.Ladder
{
    public class LadderStepHot : LadderStep
    {
        private const float ExtraExpendEnergyValuePerSecond = 3f;

        private IDisposable _extraEnergExpendSubscribtion;

        protected override void OnHandSeted(Hand hand)
        {
            if (hand == null && _extraEnergExpendSubscribtion != null)
                StopExtraExpendEnergy();
            else if (hand != null)
                StartExtraExpendEnergy();
        }

        private void StartExtraExpendEnergy()
        {
            if (_extraEnergExpendSubscribtion == null)
            {
                float expendEnergyValue = Stamina.RecoveryPerSecond + ExtraExpendEnergyValuePerSecond;

                _extraEnergExpendSubscribtion = Observable.EveryUpdate().Subscribe(_ =>
                    {
                        Game.Hands.Stamina.ForceExpendEnergy(expendEnergyValue * Time.deltaTime);
                    }).AddTo(this);
            }
        }

        private void StopExtraExpendEnergy()
        {
            if (_extraEnergExpendSubscribtion != null)
            {
                _extraEnergExpendSubscribtion.Dispose();
                _extraEnergExpendSubscribtion = null;
            }
        }
    }
}
