using Assets.Scripts.Hands;
using DG.Tweening;
using RSG;
using UnityEngine;

namespace Assets.Scripts.Ladder
{
    public class SpikyLadderStep : LadderStep
    {
        [SerializeField] private Spike _spike;

        private const float SpikeShowHideDuration = 0.2f;
        private const float ActiveStatePause = 1f;
        private const float InactiveStatePause = 2.5f;

        private bool _isSpikeActive;

        public override void Init(int id)
        {
            base.Init(id);
            StartAnimation();
        }

        protected override void OnHandSeted(Hand hand) 
        {
            if (hand != null && _isSpikeActive)
            {
                Game.Hands.ForceFail(this);
            }
        }

        private void StartAnimation()
        {
            if (gameObject == null)
                return;

            if (Hand != null)
                Game.Hands.ForceFail(this);

            PlayShow()
                .Then(() => Utils.WaitSeconds(ActiveStatePause))
                .Then(() =>
                {
                    if (Hand != null)
                        Game.Hands.ForceFail(this);

                    PlayHide();
                })
                .Then(() => Utils.WaitSeconds(InactiveStatePause))
                .Then(() => StartAnimation());
        }

        private Promise PlayShow()
        {
            Promise promise = new Promise();
            _isSpikeActive = true;

            _spike.PlayShowAnimation(SpikeShowHideDuration).OnComplete(() => promise.Resolve());

            return promise;
        }

        private Promise PlayHide()
        {
            Promise promise = new Promise();
            _isSpikeActive = false;

            _spike.PlayHideAnimation(SpikeShowHideDuration).OnComplete(() => promise.Resolve());

            return promise;
        }
    }
}
