using Assets.Scripts.Hands;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.LevelLadder.Ladder;

namespace Assets.Scripts.LevelLadder
{
    public class LadderStep : MonoBehaviour
    {
        [SerializeField] protected LadderSide _side;

        private HashSet<HandChecker> _checkers = new();

        public int Id { get; private set; }

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
            UpdateState(checker.Side);
        }

        public void OnHandExit(HandChecker checker)
        {
            _checkers.Remove(checker);
            UpdateState(checker.Side);
        }

        protected virtual void UpdateState(LadderSide side) { }

        public virtual LadderStep GetPrefab(LadderSide side) => this;
    }
}
