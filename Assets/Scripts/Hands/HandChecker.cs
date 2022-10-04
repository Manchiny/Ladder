using UnityEngine;
using static Assets.Scripts.Ladder.Ladder;

namespace Assets.Scripts.Hands
{
    public class HandChecker : MonoBehaviour
    {
        private Hand _hand;
        public LadderSide Side => _hand.Side;

        private void Awake()
        {
            _hand = GetComponentInParent<Hand>();
        }
    }
}
