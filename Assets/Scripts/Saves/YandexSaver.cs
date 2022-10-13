using Agava.YandexGames;
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

        protected override SavingData LoadData()
        {
            string dataString = "";
            PlayerAccount.GetPlayerData((data) => dataString = data);

            SavingData userData = JsonUtility.FromJson<SavingData>(dataString);

            return userData;
        }

        protected override void WriteData(SavingData savingData)
        {
            string dataToJson = JsonUtility.ToJson(savingData);
            PlayerAccount.SetPlayerData(dataToJson);
        }
    }
}
