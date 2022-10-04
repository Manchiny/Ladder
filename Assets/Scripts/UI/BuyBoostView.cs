using Assets.Scripts.Boosts;
using DG.Tweening;
using RSG;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class BuyBoostView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [Space]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Image _icon;
        [Space]
        [SerializeField] private Image[] _coloredElements;

        private const float AnimationDuration = 0.05f;
        private const float AnimationScaleValue = 0.8f;

        private RectTransform _rect;

        private bool _locked;
        private event Action _onClick;


        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void Init(Boost boost, Action onClick)
        {
            _locked = false;

            _titleText.text = boost.Name;

            if (boost.NeedShow && boost.TryGetNextLevelCost(out int cost))
            {
                _priceText.text = $"${cost}";

                if (boost.Icon != null)
                    _icon.sprite = boost.Icon;
                else
                    _icon.gameObject.SetActive(false);

                foreach (var image in _coloredElements)
                    image.color = boost.ViewColor;

                _onClick = onClick;
                _button.onClick.AddListener(OnClick);
            }
            else
                gameObject.SetActive(false);
        }

        public void SetLocked(bool needLock)
        {
            _locked = needLock;
        }

        private void OnClick()
        {
            if (_locked)
                return;

            SetLocked(true);

            PlayAnimation()
                .Then(() => _onClick?.Invoke());
        }

        private IPromise PlayAnimation()
        {
            Promise promise = new Promise();

            //if ( _clickAnimation != null && _clickAnimation.IsPlaying())
            //    return;

            var sequence = DOTween.Sequence().SetEase(Ease.Linear).SetLink(gameObject).OnComplete(() => promise.Resolve());

            sequence.Append(ScaleAnimation(AnimationScaleValue));
            sequence.Append(ScaleAnimation(1));

            return promise;
        }

        private Tween ScaleAnimation(float scaleValue)
        {
            return _rect.DOScale(scaleValue, AnimationDuration);
        }
    }
}
