using Assets.Scripts.Boosts;
using Assets.Scripts.Levels;
using System;
using System.Collections.Generic;
using static Assets.Scripts.Boosts.Boost;

namespace Assets.Scripts
{
    public class UserData
    {
        private Dictionary<BoostType, int> _boostsLevels = new Dictionary<BoostType, int>();

        public int CurrentLevelId { get; private set; }
        public int Money { get; private set; }

        public IReadOnlyDictionary<BoostType, int> BoostLevels => _boostsLevels;

        public event Action<int> MoneyChanged;

        public UserData(int levelId, int money)
        {
            CurrentLevelId = levelId;
            Money = money;
        }

        public void AddMoney(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("Incorrect money to add count");

            Money += count;
            MoneyChanged?.Invoke(Money);
        }

        public bool BuyBoost(Boost boost)
        {
            if (boost.TryGetNextLevelCost(out int cost))
            {
                if (Money >= cost)
                {
                    Money -= cost;
                    MoneyChanged?.Invoke(Money);

                    if (_boostsLevels.ContainsKey(boost.Type) == false)
                        _boostsLevels.Add(boost.Type, 0);

                    _boostsLevels[boost.Type]++;

                    Game.Saver.Save(this);

                    return true;
                }
            }

            return false;
        }

        public void SetCurrentLevelId(int id)
        {
            CurrentLevelId = id;
        }

        public void WriteBoostsData(Dictionary<BoostType, int> boostsLevels)
        {
            _boostsLevels = boostsLevels;
        }

        public int GetBoostLevel(BoostType type)
        {
            if (_boostsLevels.ContainsKey(type))
                return _boostsLevels[type];

            return -1;
        }
    }
}
