using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicButton : Button
{
    private RectTransform _transform;
    private Sequence _sequence;
    private Vector2 _startScale;

    private List<Action> _onClickActions = new List<Action>();

    protected override void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _startScale = _transform.localScale;

        onClick.AddListener(OnButtonClick);
    }

    public void AddListener(Action onButtonClick)
    {
        _onClickActions.Add(onButtonClick);
    }

    public void RemoveAllListeners()
    {
        _onClickActions?.Clear();
        onClick.RemoveAllListeners();
    }

    private void OnButtonClick()
    {
        _sequence?.Kill();
        _transform.localScale = _startScale;

        _sequence = DOTween.Sequence().SetLink(gameObject);
        _sequence.Append(_transform.DOScale(0.9f, 0.05f)).SetEase(Ease.Linear);
        _sequence.Append(_transform.DOScale(1f, 0.05f)).SetEase(Ease.Linear);

        _sequence.Play()
            .OnComplete(() =>
            {
                for (int i = 0; i < _onClickActions.Count; i++)
                {
                    var action = _onClickActions[i];
                    action?.Invoke();
                }
            });
    }

    protected override void OnDestroy()
    {
        RemoveAllListeners();
    }
}
