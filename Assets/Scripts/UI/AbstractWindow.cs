using DG.Tweening;
using RSG;
using UnityEngine;

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

    protected Sequence _sequence;

    public abstract string LockKey { get; }
    public bool AnimatedClose { get; protected set; } = false;
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
    }

    public virtual void Close()
    {
        if (_isClosing)
            return;

        _isClosing = true;

        if (AnimatedClose == true)
        {
            PlayCloseAnimation()
                 .Then(() =>
                 {
                     OnClose();
                     Destroy(gameObject);
                     ClosePromise.Resolve();
                 });
        }
        else
        {
            OnClose();
            Destroy(gameObject);
            ClosePromise.Resolve();
        }
    }

    public void Unhide()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        _canvasGroup.DOFade(1, FadeDuration).SetLink(gameObject);
    }

    public void ForceHide()
    {
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

    //protected void CloseAnimated()
    //{
    //    if (_isClosing)
    //        return;

    //    RectTransform _rectTransform = (RectTransform)_content.transform;

    //    _sequence?.Complete();

    //    _sequence = DOTween.Sequence()
    //        .SetLink(gameObject)
    //        .Append(_rectTransform.DOMoveY(_rectTransform.position.y - 100f, 0.1f))
    //        .Append(_rectTransform.DOMoveY(_rectTransform.position.y + 800f, 0.3f));

    //    _sequence.SetEase(Ease.Linear);

    //    _sequence.Play()
    //        .OnComplete(() =>
    //        {
    //            OnClose();
    //            Destroy(gameObject);
    //            ClosePromise.Resolve();
    //        });
    //}

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnClose() { }
}
