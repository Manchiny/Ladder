using Agava.YandexGames;
using System;


namespace Assets.Scripts.Social.Adverts
{
    public class YandexAdvertisingAdapter : AbstractAdvertisingAdapter
    {
        public override string Tag => "YandexAdverts";

        protected override void ShowInterstitial(Action onOpen, Action<bool> onClose, Action<string> onError, Action onOffline)
        {
            InterstitialAd.Show(onOpen, onClose, onError, onOffline);
        }

        protected override void ShowRewarded(Action onOpen, Action onRewarded, Action onClose, Action<string> onError)
        {
            VideoAd.Show(onOpen, onRewarded, onClose, onError);
        }
    }
}
