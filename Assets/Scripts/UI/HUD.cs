using Assets.Scripts.Hands;
using DG.Tweening;
using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _moneyText;
        [Space]
        [SerializeField] private RectTransform _moneyPanelContent;
        [SerializeField] private FloatingMoneyText _floatingMoneyPrefab;
        [Space]
        [SerializeField] private StaminaView _staminaView;
        [Space]
        [SerializeField] private BasicButton _settingsButton;
        [SerializeField] private BasicButton _leaderboardButton;

        private const string LevelLocalizationKey = "level";

        private const float FadeDuration = 1f;
        private const float MoneyPanelAnimationDuration = 0.075f;
        private const float FloatingMoneyDeltaYStartPosition = 1f;

        private CanvasGroup _canvas;
        private HandsMover _hands;

        protected Tween _showHideAnimation;

        private Tween MoneyAnimationTween;
        private IDisposable _levelChangeDispose;

        private void Awake()
        {
            _canvas = GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            _levelChangeDispose?.Dispose();

            Game.User.MoneyChanged -= OnMoneyChanged;
            Game.Localization.LanguageChanged -= OnLocalizationChanged;

            _settingsButton.RemoveListener(OnSettingsButtonClick);
        }

        public void Init(HandsMover hands, bool isReinit)
        {
            _canvas.alpha = 0;
            _levelChangeDispose = Game.Instance.CurrentLevelId.ObserveEveryValueChanged(x => x.Value).Subscribe(OnLevelChanged).AddTo(this);

            _leaderboardButton.gameObject.SetActive(Game.Social != null && Game.Social.IsInited && Game.Social.IsAuthorized());

            SetMoneyText(Game.User.Money);

            if (isReinit == false)
            {
                _settingsButton.AddListener(OnSettingsButtonClick);
                _leaderboardButton.AddListener(OnLeaderboardButtonClick);

                Game.User.MoneyChanged += OnMoneyChanged;
                Game.Localization.LanguageChanged += OnLocalizationChanged;
            }

            _hands = hands;
            _staminaView.Init(_hands);

            Show();
        }

        public void Show()
        {
            if (_showHideAnimation != null && _showHideAnimation.active)
            {
                _showHideAnimation.Kill();
                _showHideAnimation = null;
            }

            _showHideAnimation = _canvas.DOFade(1f, FadeDuration).SetLink(gameObject);
        }

        public void Hide()
        {
            if (_showHideAnimation != null && _showHideAnimation.active)
            {
                _showHideAnimation.Kill();
                _showHideAnimation = null;
            }

            _showHideAnimation = _canvas.DOFade(0f, FadeDuration).SetLink(gameObject);
        }

        public void ShowFloatingMoney(int count, Hand hand)
        {
            FloatingMoneyText floatingMoney = Instantiate(_floatingMoneyPrefab, transform);

            Vector3 position = hand.transform.position;
            position.y += FloatingMoneyDeltaYStartPosition;

            floatingMoney.transform.position = Camera.main.WorldToScreenPoint(position);
            floatingMoney.Init(count);
        }

        private void OnLevelChanged(int level)
        {
            _levelText.text = LevelLocalizationKey.Localize() + $" {level + 1}";
        }

        private void OnMoneyChanged(int totalMoney)
        {
            SetMoneyText(totalMoney);
            PlayMoneyPanelAnimation();
        }

        private void SetMoneyText(int level)
        {
            _moneyText.text = $"${level}";
        }

        private void PlayMoneyPanelAnimation()
        {
            if (MoneyAnimationTween != null)
                MoneyAnimationTween.Kill();

            var sequence = DOTween.Sequence().SetEase(Ease.Linear).SetLink(gameObject);

            sequence.Append(_moneyPanelContent.DOScale(1.5f, MoneyPanelAnimationDuration));
            sequence.Append(_moneyPanelContent.DOScale(1f, MoneyPanelAnimationDuration));

            MoneyAnimationTween = sequence;

            MoneyAnimationTween.Play();
        }

        private void OnSettingsButtonClick()
        {
            SettingsWindow.Show();
        }

        private void OnLocalizationChanged()
        {
            OnLevelChanged(Game.Instance.CurrentLevelId.Value + 1);
        }

        private void OnLeaderboardButtonClick()
        {
            if (Game.Social != null && Game.Social.IsAuthorized())
                LeaderboardWindow.Show(Game.Social.GetLeaderboardData());
        }
    }
}
