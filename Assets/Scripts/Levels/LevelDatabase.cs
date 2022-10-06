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
                _levels[i].Init();
        }

        public LevelConfiguration GetLevelConfiguration(int id)
        {
            if (_levels.Count > id)
                return _levels[id];

            int random = Random.Range(GameConstants.MinRandomLevelId, _levels.Count - 1);
            return _levels[random];
        }
    }
}
