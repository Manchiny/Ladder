using DG.Tweening;
using System;
using TMPro;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [Space]
    [SerializeField] private RectTransform _moneyPanelContent;

    private const float FadeDuration = 1f;
    private const float MoneyPanelAnimationDuration = 0.075f;

    private CanvasGroup _canvas;

    private Tween MoneyAnimationTween;
    private IDisposable _levelChangeDispose;

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
    }

    private void OnDestroy()
    {
        _levelChangeDispose?.Dispose();
        Game.User.MoneyChanged -= SetMoneyAndAnimate;
    }

    public void Init()
    {
        _canvas.alpha = 0;
        _levelChangeDispose = Game.Instance.CurrenLevel.ObserveEveryValueChanged(x => x.Value).Subscribe(OnLevelChanged).AddTo(this);

        SetMoneyText(Game.User.Money);

        Game.User.MoneyChanged += SetMoneyAndAnimate;
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

    private void OnLevelChanged(LevelConfiguration level)
    {
        _levelText.text = $"LEVEL {level.Id + 1}";
    }

    private void SetMoneyAndAnimate(int money)
    {
        SetMoneyText(money);
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
