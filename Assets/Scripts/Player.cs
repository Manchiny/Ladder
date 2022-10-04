using Assets.Scripts.Boosts;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class Player
    {
        private UserData _user;
        private HashSet<Boost> _boosts = new();

        public Player(UserData data)
        {
            _user = data;

            foreach (var pair in data.BoostLevels)
            {
                if (pair.Value >= 0)
                {
                    Boost boost = Game.BoostsDatabase.GetBoost(pair.Key);

                    if (boost != null)
                        _boosts.Add(boost);
                }
            }
        }

        public float CalculateEndValueWithBoosts<T>(float baseValue) where T : Boost
        {
            List<Boost> boosts = new();

            foreach (var boost in _boosts)
            {
                if (boost is T)
                    boosts.Add(boost);
            }

            if (boosts.Count == 0)
                return baseValue;

            float summ = 0;

            foreach (var boost in boosts)
                summ += boost.CalculateEndValue(baseValue);

            return summ;
        }

        public bool TryBuyBoost<T>(T boost) where T : Boost
        {
            if (_user.BuyBoost(boost))
            {
                _boosts.Add(boost);
                return true;
            }

            return false;
        }
    }
}
