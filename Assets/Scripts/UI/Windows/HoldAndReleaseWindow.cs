using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HoldAndReleaseWindow : AbstractWindow
    {
        [SerializeField] private TextMeshProUGUI _infoText;

        public override string LockKey => "HoldOndReleaseWindow";
        private const string HoldAndReleaseLocalizationKey = "holdAndRelease";

        private void OnDestroy()
        {
            Game.Localization.LanguageChanged -= SetText;
        }

        public static HoldAndReleaseWindow Show() =>
                        Game.Windows.ScreenChange<HoldAndReleaseWindow>(true, w => w.Init());

        protected void Init()
        {
            SetText();
            Game.Localization.LanguageChanged += SetText;

            new ScalePongAnimation(_infoText.transform as RectTransform);
        }

        private void SetText()
        {
            _infoText.text = HoldAndReleaseLocalizationKey.Localize();
        }
    }
}
