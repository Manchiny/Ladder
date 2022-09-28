using System.Collections.Generic;
using UnityEngine;
using static Ladder;

public class LadderStep : MonoBehaviour
{
    private HashSet<HandChecker> _checkers = new();

    public int Id { get; private set; }

    public virtual bool CanBeTaked => true;
    public int CheckersCount => _checkers.Count;

    public virtual void Init(int id)
    {
        Id = id;
    }

    public void OnHandCollided(HandChecker checker)
    {
        _checkers.Add(checker);
        UpdateState();
    }

    public void OnHandExit(HandChecker checker)
    {
        _checkers.Remove(checker);
        UpdateState();
    }

    protected virtual void UpdateState() { }

    public virtual LadderStep GetPrefab(LadderSide side) => this;
}
