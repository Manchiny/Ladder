using Assets.Scripts.Hands;
using Assets.Scripts.Levels;
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

        private const float FadeDuration = 1f;
        private const float MoneyPanelAnimationDuration = 0.075f;
        private const float FloatingMoneyDeltaYStartPosition = 1f;

        private CanvasGroup _canvas;
        private HandsMover _hands;

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
        }

        public void Init(HandsMover hands)
        {
            _canvas.alpha = 0;
            _levelChangeDispose = Game.Instance.CurrenLevel.ObserveEveryValueChanged(x => x.Value).Subscribe(OnLevelChanged).AddTo(this);

            SetMoneyText(Game.User.Money);

            Game.User.MoneyChanged += OnMoneyChanged;

            _hands = hands;
            _staminaView.Init(_hands);

            Show();
        }

        public void Show()
        {
            _canvas.DOFade(1f, FadeDuration);
        }

        public void Hide()
        {
            _canvas.DOFade(0f, FadeDuration);
        }

        public void ShowFloatingMoney(int count, Hand hand)
        {
            FloatingMoneyText floatingMoney = Instantiate(_floatingMoneyPrefab, transform);

            Vector3 position = hand.transform.position;
            position.y += FloatingMoneyDeltaYStartPosition;

            floatingMoney.transform.position = Camera.main.WorldToScreenPoint(position);
            floatingMoney.Init(count);
        }

        private void OnLevelChanged(LevelConfiguration level)
        {
            _levelText.text = $"LEVEL {level.Id + 1}";
        }

        private void OnMoneyChanged(int totalMoney)
        {
            SetMoneyText(totalMoney);
            PlayMoneyPanelAnimation();
        }

        private void SetMoneyText(int money)
        {
            _moneyText.text = $"${money}";
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
    }
}
