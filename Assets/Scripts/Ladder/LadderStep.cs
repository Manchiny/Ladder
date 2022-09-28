using System.Collections.Generic;
using UnityEngine;

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

    public virtual void OnHandCollided(HandChecker checker)
    {
        _checkers.Add(checker);
    }

    public virtual void OnHandExit(HandChecker checker)
    {
        _checkers.Remove(checker);
    }
}
