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

        protected override void UpdateState(Ladder.LadderSide side, bool handEnter)
        {
            if (handEnter)
                StartExtraExpendEnergy();
            else if(Hand == null)
                StopExtraExpendEnergy();
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
