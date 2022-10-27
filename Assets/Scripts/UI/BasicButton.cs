using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class BasicButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ParticleSystem _effect;
        [SerializeField] private bool _needAnimateOnClick = true;
        [SerializeField] private TextMeshProUGUI _text;

        private Button _button;

        private RectTransform _rectTransform;
        private CanvasGroup _group;

        private Vector3 _baseScale;

        private Sequence _pressTween;
        private Tween _appearTween;
        private Sequence _selectTween;

        private bool _isVisible = true;
        private bool _touchable = true;

        private Button.ButtonClickedEvent _onClick = new Button.ButtonClickedEvent();
        private ReactiveProperty<bool> _isDown = new ReactiveProperty<bool>(false);

        private event Action _downCallback;
        private event Action _upCallback;
        private event Action _resetAction;
        private event Action _soundClickCallback;

        private enum AdButtonState
        {
            None, Hidden, Loading, Active
        }

        public CanvasGroup CanvasGroup => _group;
        public ParticleSystem Effect => _effect;
        public Button Button => _button;
        public Button.ButtonClickedEvent OnClick => _button ? _button.onClick : _onClick;

        public bool Locked { get; private set; } = false;

        /** Кликабельность по кнопке **/
        public bool Touchable
        {
            get => _touchable;
            set
            {
                _touchable = value;

                if (_button)
                    _button.interactable = _touchable;
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                SetVisible(value);
            }
        }

        public string Text
        {
            get => _text.text;
            set
            {
                if(_text != null)
                    _text.text = value;

                //Scripts.Utils.Utils.ForceRebuildLayoutOnNextFrame(_text.transform.parent as RectTransform);
            }
        }

        public void Awake()
        {
            if (!_button)
                _button = GetComponent<Button>();

            _rectTransform = GetComponent<RectTransform>();

            _baseScale = _rectTransform.localScale;

            _group = GetComponent<CanvasGroup>();

            if (_effect)
                _effect.gameObject.SetActive(false);

            OnAwake();
        }

        private void OnDisable()
        {
            SetEffect(false);
        }

        protected virtual void OnDestroy()
        {
            _selectTween?.Kill();
            _selectTween = null;
        }

        public virtual void OnAwake() { }

        public void SetOnClick(Action onClick)
        {
            OnClick.RemoveAllListeners();
            OnClick.AddListener(() => onClick?.Invoke());
        }

        public void AddListener(Action onClick)
        {
            OnClick.AddListener(() => onClick?.Invoke());
        }

        public void RemoveListener(Action onClick)
        {
            OnClick.RemoveListener(() => onClick?.Invoke());
        }

        public void RemoveAllListeners()
        {
            OnClick.RemoveAllListeners();
        }

        public void SetOnDownCallback(Action callback)
        {
            _downCallback = callback;
        }

        public void SetOnUpCallback(Action callback)
        {
            _upCallback = callback;
        }

        public bool Enabled
        {
            get => !Locked;
            set
            {
                SetLock(!value);
            }
        }

        public void SetLock(bool val, bool touchable = false)
        {
            if (Locked == val)
                return;

            Locked = val;

            Touchable = val ? touchable : true; // При снятии лока всегда возвращаем кликабельность кнопки

            if (this && gameObject)
                _button.interactable = !Locked;
        }

        public virtual void OnSelectAnimation()
        {
            if (!gameObject) return;
            _selectTween?.Kill();

            _selectTween = DOTween.Sequence();
            _selectTween.Append(_rectTransform.DOScale(new Vector3(_baseScale.x * 0.95f, _baseScale.y * 1.01f, _baseScale.z), 0.05f));
            _selectTween.Append(_rectTransform.DOScale(new Vector3(_baseScale.x * 1.05f, _baseScale.y * 0.9f, _baseScale.z), 0.05f));
            _selectTween.Append(_rectTransform.DOScale(new Vector3(_baseScale.x * 0.95f, _baseScale.y * 1.01f, _baseScale.z), 0.05f));
            _selectTween.Append(_rectTransform.DOScale(_baseScale, 0.05f));
            _selectTween.OnComplete(() => _selectTween = null);
            _selectTween.SetLink(gameObject).SetUpdate(true);

            _selectTween.Play();
        }

        public virtual void OnPressAnimationStart()
        {
            if (!gameObject) return;

            if (_pressTween != null)
            {
                _pressTween.Rewind();
                _pressTween.Play();
                return;
            }

            _pressTween = DOTween.Sequence()
                .Append(_rectTransform.DOScale(new Vector3(_baseScale.x * 0.95f, _baseScale.y * .95f, _baseScale.z), 0.05f));

            _pressTween.SetLink(gameObject).SetUpdate(true);
            _pressTween.SetAutoKill(false);

            _pressTween.Play();
        }

        public virtual void OnPressAnimationFinished()
        {
            if (!gameObject) return;

            if (_pressTween != null)
            {
                _pressTween.Rewind();
            }
        }

        public virtual void StartBlinkAnimation()
        {
            if (!gameObject) return;
            _selectTween?.Kill();

            _selectTween = DOTween.Sequence();
            _selectTween.Append(_rectTransform.DOScale(new Vector3(_baseScale.x * 0.95f, _baseScale.y * 1.01f, _baseScale.z), 0.1f));
            _selectTween.Append(_rectTransform.DOScale(_baseScale, 0.1f));
            _selectTween.Append(_rectTransform.DOScale(new Vector3(_baseScale.x * 1.05f, _baseScale.y * 0.9f, _baseScale.z), 0.1f));
            _selectTween.Append(_rectTransform.DOScale(_baseScale, 0.1f));
            _selectTween.SetLoops(-1);
            _selectTween.OnComplete(() => _selectTween = null);
            _selectTween.SetLink(gameObject).SetUpdate(true);
            _selectTween.Play();
        }

        public virtual void StartGrowAnimation()
        {
            if (!gameObject) return;
            _selectTween?.Kill();

            _selectTween = DOTween.Sequence();
            _selectTween.Append(_rectTransform.DOScale(new Vector3(_baseScale.x * 1.05f, _baseScale.y * 1.05f, _baseScale.z), 0.5f));
            _selectTween.Append(_rectTransform.DOScale(_baseScale, 0.5f));
            _selectTween.SetLoops(-1);
            _selectTween.OnComplete(() => _selectTween = null);
            _selectTween.SetLink(gameObject);
            _selectTween.Play();
        }

        public void SetVisible(bool visible)
        {
            _isVisible = visible;
            AppearDisappear(visible);
        }

        public void StopAnim()
        {
            _selectTween?.Kill();
            _selectTween = null;
        }

        public void SetSound(Action soundClickCallback)
        {
            _soundClickCallback = soundClickCallback;
        }

        public virtual void OnPointerClick(PointerEventData eventData) { }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!Touchable) return;

            _isDown.Value = true;

            _downCallback?.Invoke();

            if (_needAnimateOnClick)
                OnPressAnimationStart();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!Touchable) return;

            _isDown.Value = false;

            _upCallback?.Invoke();

            float dragDistance = Vector2.Distance(eventData.pressPosition, eventData.position);
            var dragThreshold = Screen.dpi / 12;
            var isDrag = dragDistance > dragThreshold;

            //Брать приходится каждый раз, т.к. парент у кнопок может поменятся
            var parentCanvas = GetComponentInParent<CanvasGroup>();
            if (parentCanvas != null && !parentCanvas.interactable) return;

            if (isDrag)
            {
                if (_needAnimateOnClick)
                    OnPressAnimationFinished();
                return;
            }

            if (_needAnimateOnClick)
                OnSelectAnimation();

            if (!_button)
                _onClick?.Invoke();

            PlaySoundClick();
        }

        internal void PlaySoundClick()
        {
            if (_soundClickCallback != null)
                _soundClickCallback?.Invoke();
            else
                Game.Sound.PlayBasicButtonClick();
        }

        private void SetEffect(bool appear)
        {
            if (!Effect) return;
            if (!appear)
                Effect.gameObject.SetActive(false);
            else
            {
                Effect.gameObject.SetActive(true);
                Effect.Play();
            }
        }

        private void AppearDisappear(bool appear, bool needEffect = false, float duration = .25f)
        {
            if (!gameObject)
                return;

            _resetAction?.Invoke();
            _appearTween?.Kill();

            Touchable = appear;

            gameObject.SetActive(appear);
            SetEffect(needEffect && appear);

            if (CanvasGroup)
            {
                var alpha = appear ? 1f : 0f;
                _appearTween =
                CanvasGroup.DOFade(alpha, duration).SetEase(Ease.InSine).SetLink(gameObject).SetUpdate(true);

                if (!appear)
                    _appearTween.OnComplete(() => gameObject.SetActive(false));

                _resetAction = () =>
                {
                    _appearTween?.Kill();
                    if (!appear) gameObject.SetActive(false);
                    if (CanvasGroup) CanvasGroup.alpha = alpha;
                };
            }
        }
    }
}


