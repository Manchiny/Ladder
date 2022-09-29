using UnityEngine;
using static Ladder;

public class HandChecker : MonoBehaviour
{
    private Hand _hand;
    public LadderSide Side => _hand.Side;

    private void Awake()
    {
        _hand = GetComponentInParent<Hand>();
    }
}
