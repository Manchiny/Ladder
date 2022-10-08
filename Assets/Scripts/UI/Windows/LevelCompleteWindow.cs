using DG.Tweening;
using RSG;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LevelCompleteWindow : AbstractWindow
    {
        [SerializeField] private TextMeshProUGUI _completeText;
        [SerializeField] private RectTransform _panelWithStars;
        [SerializeField] private List<RectTransform> _stars;
        [Space]
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _continueButtonText;

        public override string LockKey => "LevelCompleteWindow";

        private const string TitleLocalizationKey = "reachedToSky";
        private const string ButtonLocalizationKey = "tapToContinue";

        private event Action _continueButtonClicked;

        public static LevelCompleteWindow Show(Action onContinueClicked) =>
                   Game.Windows.ScreenChange<LevelCompleteWindow>(true, w => w.Init(onContinueClicked));

        private void OnDestroy()
        {
            Game.Localization.LanguageChanged -= SetText;
        }

        protected void Init(Action onContinueClicked)
        {
            SetText();

            Game.Localization.LanguageChanged += SetText;

            _continueButtonClicked = onContinueClicked;
            _continueButton.onClick.AddListener(OnContinueButtonClick);

            _panelWithStars.localScale = Vector3.zero;
            _stars.ForEach(star => star.localScale = Vector3.zero);

            PlaySequencesScaleAnimation(_panelWithStars)
                .Then(PlayStarsAnimation);
        }

        private IPromise PlayStarsAnimation()
        {
            Promise promise = new Promise();

            List<Func<IPromise>> animations = new();
            _stars.ForEach(star => animations.Add(() => PlaySequencesScaleAnimation(star)));

            var sequence = Promise.Sequence(animations);

            sequence
                .Then(() => promise.Resolve());

            return promise;
        }

        private IPromise PlaySequencesScaleAnimation(RectTransform transform)
        {
            Promise promise = new Promise();

            List<Func<IPromise>> animations = new();

            animations.Add(() => PlayScaleAnimation(transform, 1.3f, 0.12f));
            animations.Add(() => PlayScaleAnimation(transform, 0.8f, 0.05f));
            animations.Add(() => PlayScaleAnimation(transform, 1.15f, 0.04f));
            animations.Add(() => PlayScaleAnimation(transform, 0.9f, 0.03f));
            animations.Add(() => PlayScaleAnimation(transform, 1f, 0.02f));

            var sequence = Promise.Sequence(animations);

            sequence
               .Then(() => promise.Resolve());

            return promise;
        }

        private IPromise PlayScaleAnimation(RectTransform transform, float endValue, float duration)
        {
            Promise promise = new Promise();

            transform.DOScale(endValue, duration)
                .SetEase(Ease.Linear)
                .SetLink(transform.gameObject)
                .OnComplete(() => promise.Resolve());

            return promise;
        }

        private void OnContinueButtonClick()
        {
            if (_continueButtonClicked != null)
                _continueButtonClicked?.Invoke();

            _continueButton.onClick.RemoveAllListeners();

            Close();
        }

        private void SetText()
        {
            _completeText.text = TitleLocalizationKey.Localize();
            _continueButtonText.text = ButtonLocalizationKey.Localize();
        }
    }
}
