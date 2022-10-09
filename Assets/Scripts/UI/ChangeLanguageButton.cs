using Assets.Scripts.Localization;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(BasicButton))]
    public class ChangeLanguageButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _languageText;

        private BasicButton _button;
        private string _locale;

        private Color _baseColor;

        private bool _isActive;

        private void Awake()
        {
            _button = GetComponent<BasicButton>();
        }

        private void OnDestroy()
        {
            Game.Localization.LanguageChanged -= SetActive;
            _button.RemoveListener(OnButtonClick);
        }

        public void Init(string locale)
        {
            _baseColor = _button.Button.image.color;

            _locale = locale;
            _languageText.text = locale.Localize();

            Game.Instance.GameLocalization.LanguageChanged += SetActive;

            SetActive();
            _button.AddListener(OnButtonClick);
        }

        private void SetActive()
        {
            _isActive = _locale != GameLocalization.CurrentLocale;
            _button.Button.image.color = _isActive ? _baseColor : Color.gray;
        }

        private void OnButtonClick()
        {
            if (!_isActive)
                return;

            Game.Instance.ChangeLocale(_locale);
        }
    }
}
