using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HoldAndReleaseWindow : AbstractWindow
    {
        [SerializeField] private TextMeshProUGUI _infoText;

        public override string LockKey => "HoldOndReleaseWindow";
        private const string HoldAndReleaseLocalizationKey = "holdAndRelease";

        public static HoldAndReleaseWindow Show() =>
                        Game.Windows.ScreenChange<HoldAndReleaseWindow>(true, w => w.Init());

        protected void Init()
        {
            _infoText.text = HoldAndReleaseLocalizationKey.Localize();
            new ScalePongAnimation(_infoText.transform as RectTransform);
        }
    }
}
