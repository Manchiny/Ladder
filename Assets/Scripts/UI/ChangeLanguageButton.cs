using Assets.Scripts.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class ChangeLanguageButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _languageText;

        private Button _button;
        private string _locale;

        private Color _baseColor;

        private bool _isActive;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _baseColor = _button.image.color;
        }

        private void OnDestroy()
        {
            Game.Localization.LanguageChanged -= SetActive;
            _button.onClick.RemoveListener(OnButtonClick);
        }

        public void Init(string locale)
        {
            _locale = locale;
            _languageText.text = locale.Localize();

            Game.Instance.GameLocalization.LanguageChanged += SetActive;

            SetActive();
            _button.onClick.AddListener(OnButtonClick);
        }

        private void SetActive()
        {
            _isActive = _locale != GameLocalization.CurrentLocale;
            _button.image.color = _isActive ? _baseColor : Color.gray;
        }

        private void OnButtonClick()
        {
            if (!_isActive)
                return;

            Game.Instance.ChangeLocale(_locale);
        }
    }
}
