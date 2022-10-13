using Assets.Scripts.Boosts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Boosts.Boost;
using static Assets.Scripts.Saves.Saver;

namespace Assets.Scripts
{
    public class UserData
    {
        private SavingData _data = new();

        public UserData(SavingData data)
        {
            if (data != null)
            {
                _data = data;
                SavedLocale = data.SavedLocale;
                NeedSound = data.NeedSound;
                BoostsData = data.BoostsData;
                CurrentLevelId = data.CurrentLevelId;
                Money = data.Money;
            }
        }

        public event Action<int> MoneyChanged;

        public string SavedLocale 
        { 
            get => _data.SavedLocale;
            set => _data.SavedLocale = value;
        }

        public bool NeedSound
        {
            get => _data.NeedSound;
            set => _data.NeedSound = value;
        }

        public List<BoostData> BoostsData
        {
            get => _data.BoostsData;
            private set => _data.BoostsData = value;
        }

        public int CurrentLevelId
        {
            get => _data.CurrentLevelId;
            private set => _data.CurrentLevelId = value;
        }

        public int Money
        {
            get => _data.Money;
            private set => _data.Money = value;
        }

        public SavingData GetData()
        {
            SavingData data = new();

            data.SavedLocale = SavedLocale;
            data.NeedSound = NeedSound;
            data.BoostsData = BoostsData;
            data.CurrentLevelId= CurrentLevelId;
            data.Money = Money;

            return data;
        }

        public void AddMoney(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("Incorrect money to add count");

            Money += count;
            MoneyChanged?.Invoke(Money);
        }

        public void SetCurrentLevelId(int id)
        {
            CurrentLevelId = id;
        }

        public bool BuyBoost(Boost boost)
        {
            if (boost.TryGetNextLevelCost(out int cost))
            {
                if (Money >= cost)
                {
                    Money -= cost;
                    MoneyChanged?.Invoke(Money);

                    var data = BoostsData.Where(data => data.Type == boost.Type).FirstOrDefault();

                    if (data == null)
                    {
                        data = new BoostData(boost.Type, 0);
                        BoostsData.Add(data);
                    }

                    data.Level++;

                    Game.Saver.Save(_data);

                    return true;
                }
            }

            return false;
        }

        public int GetBoostLevel(BoostType type)
        {
            var data = BoostsData.Where(data => data.Type == type).FirstOrDefault();

            if (data != null)
                return data.Level;

            return -1;
        }

        public void ShowData()
        {
            string data = JsonUtility.ToJson(_data);
            Debug.Log(data);
        }
    }
}
