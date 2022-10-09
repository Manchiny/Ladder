using Assets.Scripts.Localization;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SettingsWindow : AbstractWindow
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _soundText;
        [Space]
        [SerializeField] private TextMeshProUGUI _languageSettingsText;
        [SerializeField] private RectTransform _languageButtonContainer;
        [SerializeField] private ChangeLanguageButton _languageButtonPrefab;

        private const string LanguageLocalizationKey = "language";
        private const string TitleLocalizationKey = "settings";

        private UserInput _userInput;
        private bool _userInputActive;

        public override string LockKey => "SettingsWindow";
        protected override bool NeedCloseOnOutOfClick => true;

        public static SettingsWindow Show() =>
                       Game.Windows.ScreenChange<SettingsWindow>(false, w => w.Init());

        protected void Init()
        {
            _userInput = Game.UserInput;
            _userInputActive = _userInput.IsActive;
            _userInput.SetActive(false);

            SetText();

            AnimatedClose = true;
            NeedHideHudOnShow = true;

            foreach (var lang in GameLocalization.AvailableLocals)
            {
                ChangeLanguageButton button = Instantiate(_languageButtonPrefab, _languageButtonContainer);
                button.Init(lang);
            }
        }

        protected override void OnClose()
        {
            _userInput.SetActive(_userInputActive);
        }

        protected override void SetText()
        {
            _languageSettingsText.text = LanguageLocalizationKey.Localize() + ":";
            _titleText.text = TitleLocalizationKey.Localize();
        }
    }
}
