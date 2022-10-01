using System.Collections.Generic;
using UnityEngine;
using static Boost;

[CreateAssetMenu(fileName = "BoostsDatabase", menuName = "Boosts/BoostsDatabase")]
public class BoostsDatabase : ScriptableObject
{
    [SerializeField] private List<Boost> _boosts;

    public Boost GetBoost(BoostType type) 
    {
        foreach (var boost in _boosts)
        {
            if (boost.Type == type)
                return boost;
        }

        Debug.LogError($"There is not boost {type} in Database!");
        return null;
    }
}
