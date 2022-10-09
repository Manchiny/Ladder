using DG.Tweening;
using RSG;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class AbstractWindow : MonoBehaviour
    {
        private const float FadeDuration = 2f;

        private const float CloseAnimationMoveDownDuration = 0.1f;
        private const float CloseAnimationMoveUpDuration = 0.3f;
        private const float CloseAnimationMoveDownDistance = 100f;

        protected CanvasGroup _canvasGroup;
        protected RectTransform _content;

        protected bool _isOpening = false;
        protected bool _isClosing = false;

        protected Tween _showHideAnimation;

        public abstract string LockKey { get; }
        public bool AnimatedClose { get; protected set; } = false;
        public bool NeedHideHudOnShow { get; protected set; } = false;

        public Promise ClosePromise { get; } = new Promise();

        public bool IsOpening => _isOpening;
        public bool IsClosing => _isClosing;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _content = transform.Find("Content").transform as RectTransform;
            OnAwake();
        }

        protected virtual void Start()
        {
            OnStart();
            _isOpening = true;
            Game.Localization.LanguageChanged += SetText;
        }

        public virtual void Close()
        {
            if (_isClosing)
                return;

            _isClosing = true;

            Game.Localization.LanguageChanged -= SetText;

            if (AnimatedClose == true)
                PlayCloseAnimation()
                     .Then(() => BaseClose());
            else
                BaseClose();

            void BaseClose()
            {
                OnClose();
                Destroy(gameObject);
                ClosePromise.Resolve();
            }
        }

        public void Unhide()
        {
            if (_showHideAnimation != null && _showHideAnimation.active)
            {
                _showHideAnimation.Kill();
                _showHideAnimation = null;
            }

            OnUnhide();

            if (NeedHideHudOnShow)
                Game.Windows.HUD.Hide();
            else
                Game.Windows.HUD.Show();

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _showHideAnimation = _canvasGroup.DOFade(1, FadeDuration).SetLink(gameObject);
        }

        public void ForceHide()
        {
            if (_showHideAnimation != null && _showHideAnimation.active)
            {
                _showHideAnimation.Kill();
                _showHideAnimation = null;
            }

            OnHide();

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0;
        }

        protected virtual IPromise PlayCloseAnimation()
        {
            Promise promise = new Promise();

            RectTransform _rectTransform = (RectTransform)_content.transform;
            Sequence sequence = DOTween.Sequence()
                    .SetLink(gameObject)
                    .Append(_rectTransform.DOMoveY(_rectTransform.position.y - CloseAnimationMoveDownDistance, CloseAnimationMoveDownDuration))
                    .Append(_rectTransform.DOMoveY(Screen.safeArea.height * 2f, CloseAnimationMoveUpDuration));

            sequence.SetLink(gameObject);

            sequence.Play()
                .OnComplete(() => promise.Resolve());

            return promise;
        }

        protected abstract void SetText();

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnClose() { }
        protected virtual void OnHide() { }
        protected virtual void OnUnhide() { }
    }
}
