using UnityEngine;
using static Assets.Scripts.Ladder.Ladder;

namespace Assets.Scripts.Ladder
{
    public class LadderStepHalfDynamic : LadderStep
    {
        [SerializeField] private Animator _animator;

        private const string IncriasingAnimationName = "Increasing";

        private enum Phase
        {
            Decreasing,
            Increasing
        }

        public override bool CanBeTaked(LadderSide side) => base.CanBeTaked(side) && (CheckersCount > 1 || (CheckersCount > 0 && GetPhase() == Phase.Increasing));

        protected override void UpdateState(LadderSide side, bool handEnter)
        {
            _animator.speed = CanBeTaked(side) ? 0 : 1;
        }

        private Phase GetPhase()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(IncriasingAnimationName))
                return Phase.Increasing;

            return Phase.Decreasing;
        }
    }
}
