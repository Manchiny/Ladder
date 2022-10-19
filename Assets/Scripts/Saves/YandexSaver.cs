using Agava.YandexGames;
using RSG;
using UnityEngine;

namespace Assets.Scripts.Saves
{
    public class YandexSaver : Saver
    {
        public override string Tag => "[YandexSaver]";

        public override void RemoveAllData()
        {    
            Save(new SavingData());
        }

        protected override Promise<SavingData> LoadData()
        {
            Promise<SavingData> promise = new();

            PlayerAccount.GetPlayerData(OnSuccess, OnError);
            return promise;

            void OnSuccess(string data)
            {
                string dataString = data;
                SavingData userData = JsonUtility.FromJson<SavingData>(dataString);

                promise.Resolve(userData);
            }

            void OnError(string error)
            {
                Debug.LogError(Tag + " get player data error: " + error);
                SavingData data = new SavingData();
                promise.Resolve(data);
            }
        }

        protected override void WriteData(SavingData savingData)
        {
            string dataToJson = JsonUtility.ToJson(savingData);
            PlayerAccount.SetPlayerData(dataToJson);
        }
    }
}
