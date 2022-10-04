using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Levels
{
    [CreateAssetMenu]
    public class LevelDatabase : ScriptableObject
    {
        [SerializeField] private List<LevelConfiguration> _levels;

        public void Init()
        {
            for (int i = 0; i < _levels.Count; i++)
                _levels[i].Init(i);
        }

        public LevelConfiguration GetLevelConfiguration(int id) => _levels.Count > id ? _levels[id] : null;
        public bool IsLevelLast(int id) => _levels.Count - 1 == id;
        public LevelConfiguration GetNextLevelConfiguration(LevelConfiguration currentLevel)
        {
            if (currentLevel.Id + 1 < _levels.Count)
                return _levels[currentLevel.Id + 1];
            else
                return _levels[0];
        }
    }
}
