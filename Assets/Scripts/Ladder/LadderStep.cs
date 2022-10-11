using Assets.Scripts.Hands;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Ladder.Ladder;

namespace Assets.Scripts.Ladder
{
    public class LadderStep : MonoBehaviour
    {
        [SerializeField] protected LadderSide _side;

        private HashSet<HandChecker> _checkers = new();
        private Hand _hand;

        public int Id { get; private set; }

        public Hand Hand
        {
            get => _hand;
            set
            {
                if (value != _hand)
                {
                    _hand = value;
                    OnHandSeted(value);
                }
            }
        }

        public virtual bool CanBeTaked(LadderSide side) => _side == LadderSide.Default || _side == side;

        public int CheckersCount => _checkers.Count;
        public float Height => transform.position.y;

        public virtual void Init(int id)
        {
            Id = id;
        }

        public void OnHandCollided(HandChecker checker)
        {
            _checkers.Add(checker);
            UpdateState(checker.Side, true);
        }

        public void OnHandExit(HandChecker checker)
        {
            _checkers.Remove(checker);
            UpdateState(checker.Side, false);
        }

        public virtual LadderStep GetPrefab(LadderSide side) => this;

        protected virtual void UpdateState(LadderSide side, bool handEnter) { }
        protected virtual void OnHandSeted(Hand hand) { }

    }
}
