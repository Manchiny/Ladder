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

        protected CanvasGroup CanvasGroup;
        protected RectTransform Content;

        private Tween _showHideAnimation;
        private bool _isHide;

        public abstract string LockKey { get; }

        public virtual bool AnimatedClose => false;
        public virtual bool NeedHideHudOnShow => false;

        protected virtual bool NeedCloseOnOutOfClick => false;

        public bool IsOpening { get; private set; } = false;
        public bool IsClosing { get; private set; } = false;

        public Promise ClosePromise { get; } = new Promise();

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            Content = transform.Find("Content").transform as RectTransform;
            OnAwake();
        }

        protected virtual void Start()
        {
            OnStart();
            IsOpening = true;

            Game.Localization.LanguageChanged += SetText;

            if(NeedCloseOnOutOfClick)
                Game.UserInput.Touched += OnOutOfClick;
        }

        public virtual void Close()
        {
            IsOpening = false;

            if (IsClosing)
                return;

            IsClosing = true;
            Debug.Log(LockKey + " closing");

            if (NeedCloseOnOutOfClick)
                Game.UserInput.Touched -= OnOutOfClick;

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
            if (_isHide==false)
                return;

            _isHide = false;

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

            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;

            _showHideAnimation = CanvasGroup.DOFade(1, FadeDuration).SetLink(gameObject);
        }

        public void ForceHide()
        {
            if (_isHide)
                return;

            _isHide = true;

            if (_showHideAnimation != null && _showHideAnimation.active)
            {
                _showHideAnimation.Kill();
                _showHideAnimation = null;
            }

            OnHide();

            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.alpha = 0;
        }

        protected virtual IPromise PlayCloseAnimation()
        {
            Promise promise = new Promise();

            RectTransform _rectTransform = (RectTransform)Content.transform;
            Sequence sequence = DOTween.Sequence()
                    .SetLink(gameObject)
                    .Append(_rectTransform.DOMoveY(_rectTransform.position.y - CloseAnimationMoveDownDistance, CloseAnimationMoveDownDuration))
                    .Append(_rectTransform.DOMoveY(Screen.safeArea.height * 2f, CloseAnimationMoveUpDuration));

            sequence.SetLink(gameObject).SetUpdate(true);

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

        private void OnOutOfClick()
        {
            if(NeedCloseOnOutOfClick && _isHide == false)
                Close();
        }
    }
}
