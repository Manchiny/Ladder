using Assets.Scripts.Hands;
using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class YouAreTiredWindow : AbstractWindow
    {
        [SerializeField] private TextMeshProUGUI _releaseHandText;
        [SerializeField] private TextMeshProUGUI _youAreTiredText;

        private const string ReleaseHandLocalizationKey = "releaseHand";
        private const string YouAreTiredLocalizationKey = "youAreTired";

        public override string LockKey => "YouAreTiredWindow";

        private Stamina _stamina;
        private IDisposable _checkEnergyDispose;

        public static YouAreTiredWindow Show(Stamina stamina) =>
                        Game.Windows.ScreenChange<YouAreTiredWindow>(true, w => w.Init(stamina));

        private void OnDestroy()
        {
            if (_checkEnergyDispose != null)
                _checkEnergyDispose.Dispose();
        }

        protected void Init(Stamina stamina)
        {
            SetText();

            new ScalePongAnimation(_releaseHandText.transform as RectTransform);

            _stamina = stamina;
            _checkEnergyDispose = _stamina.CurrentEnergy.ObserveEveryValueChanged(value => value.Value).Subscribe(OnStaminaValueChanged).AddTo(this);
        }

        protected override void SetText()
        {
            _releaseHandText.text = ReleaseHandLocalizationKey.Localize();
            _youAreTiredText.text = YouAreTiredLocalizationKey.Localize();
        }

        private void OnStaminaValueChanged(float value)
        {
            if (_stamina.IsLowEnergy(out float factor) == false)
                Close();
        }
    }
}
