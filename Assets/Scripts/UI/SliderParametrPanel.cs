using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SliderParametrPanel : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private BasicButton _button;
        [Space]
        [SerializeField] private TextMeshProUGUI _parametrNameText;
        [SerializeField] private TextMeshProUGUI _onText;
        [SerializeField] private TextMeshProUGUI _offText;

        private const float SliderAnimationDuration = 0.2f;

        private bool _enabled;
        private Tween _sliderAnimation;

        private event Action<bool> _onClickCallback;

        private void OnDestroy()
        {
            _button.RemoveListener(OnSliderClick);
        }

        public void Init(bool isNowEnabled, string parametrName, Action<bool> onClick)
        {
            _onClickCallback = onClick;

            SetText(parametrName);

            _enabled = isNowEnabled;
            AnimateSlider(_enabled);

            _button.AddListener(OnSliderClick);
        }

        public void SetText(string parametrName)
        {
            _parametrNameText.text = parametrName + ":";
            _offText.text = "off".Localize();
            _onText.text = "on".Localize();
        }

        private void OnSliderClick()
        {
            _enabled = !_enabled;

            AnimateSlider(_enabled);
            _onClickCallback?.Invoke(_enabled);
        }

        private void AnimateSlider(bool enable)
        {
            float value = enable ? _slider.maxValue : _slider.minValue;
            float currentValue = _slider.value;
            float distance = Mathf.Abs(value - currentValue);

            if (distance == 0)
                return;

            float factor = _slider.maxValue / distance;
            float duration = SliderAnimationDuration / factor;

            if (_sliderAnimation != null)
                _sliderAnimation.Kill();

            _sliderAnimation = _slider.DOValue(value, duration).SetUpdate(true);
        }
    }
}
