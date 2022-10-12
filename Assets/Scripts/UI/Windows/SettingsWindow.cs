using Assets.Scripts.Localization;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SettingsWindow : AbstractWindow
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private SliderParametrPanel _soundPanlel;
        [Space]
        [SerializeField] private TextMeshProUGUI _languageSettingsText;
        [SerializeField] private RectTransform _languageButtonContainer;
        [SerializeField] private ChangeLanguageButton _languageButtonPrefab;
        [Space]
        [SerializeField] private BasicButton _connectToSocial;

        private const string LanguageLocalizationKey = "language";
        private const string TitleLocalizationKey = "settings";
        private const string SoundLocalizationKey = "sound";

        private UserInput _userInput;
        private bool _userInputActive;

        public override string LockKey => "SettingsWindow";
        public override bool AnimatedClose => true;
        public override bool NeedHideHudOnShow => true;
        protected override bool NeedCloseOnOutOfClick => true;

        public static SettingsWindow Show() =>
                       Game.Windows.ScreenChange<SettingsWindow>(false, w => w.Init());

        protected void Init()
        {
            _userInput = Game.UserInput;
            _userInputActive = _userInput.IsActive;
            _userInput.SetActive(false);

            InitSocialButton();

            SetText();

            foreach (var lang in GameLocalization.AvailableLocals)
            {
                ChangeLanguageButton button = Instantiate(_languageButtonPrefab, _languageButtonContainer);
                button.Init(lang);
            }

            _soundPanlel.Init(Game.Sound.Enabled, SoundLocalizationKey.Localize(), OnSoundSliderClicked);
        }

        protected override void OnClose()
        {
            _userInput.SetActive(_userInputActive);
        }

        protected override void SetText()
        {
            _languageSettingsText.text = LanguageLocalizationKey.Localize() + ":";
            _titleText.text = TitleLocalizationKey.Localize();

            _soundPanlel.SetText(SoundLocalizationKey);
        }

        private void OnSoundSliderClicked(bool enabled)
        {
            Game.Instance.SetSound(enabled);
        }

        private void InitSocialButton()
        {
            if(Game.Social != null)
            {
                if(Game.Social.IsInited && Game.Social.IsAuthorized() == false)
                {
                    _connectToSocial.gameObject.SetActive(true);
                    _connectToSocial.AddListener(() => Game.Social.ConnectProfileToSocial(OnAuthorizationSuccess, OnAuthorizationError));

                    _connectToSocial.Text = "connect_to".Localize() + $" {Game.Social.Name}"; 

                    return;
                }
            }

            _connectToSocial.gameObject.SetActive(false);
        }

        private void OnAuthorizationSuccess()
        {
            _connectToSocial.gameObject.SetActive(false);
        }

        private void OnAuthorizationError(string text)
        {
            _connectToSocial.gameObject.SetActive(false);
            Debug.Log($"Error autorization: {text}");
        }
    }
}
