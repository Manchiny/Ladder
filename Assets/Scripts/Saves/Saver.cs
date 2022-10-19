using RSG;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Boosts.Boost;

namespace Assets.Scripts.Saves
{
    public abstract class Saver
    {
        public abstract string Tag { get; }

        public abstract void RemoveAllData();

        public void Save(SavingData savingData)
        {
            WriteData(savingData);
            Debug.Log(Tag + ": save progress.");
        }

        public Promise<UserData> LoadUserData()
        {
            Promise<UserData> promie = new();

            Debug.Log(Tag + ": load progress.");

            LoadData()
                .Then(data =>
                {
                    UserData userData = new UserData(data);
                    promie.Resolve(userData);
                });

            return promie;
        }

        protected abstract void WriteData(SavingData savingData);
        protected abstract Promise<SavingData> LoadData();

        [Serializable]
        public class SavingData
        {
            public string SavedLocale;
            public bool NeedSound;
            public int CurrentLevelId;
            public int Money;
            public List<BoostData> BoostsData = new();
        }

        [Serializable]
        public class BoostData
        {
            public BoostType Type;
            public int Level;

            public BoostData(BoostType type, int level)
            {
                Type = type;
                Level = level;
            }
        }
    }
}
