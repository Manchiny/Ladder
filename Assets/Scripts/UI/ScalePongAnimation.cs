using DG.Tweening;
using UnityEngine;

public class ScalePongAnimation 
{
    private const float EndValue = 1.2f;
    private const float Duration = 0.5f;

    private RectTransform _transform;

    public ScalePongAnimation(RectTransform transform)
    {
        _transform = transform;
        AddScaleAnimation();
    }

    private void AddScaleAnimation()
    {
        _transform.DOScale(EndValue, Duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetLink(_transform.gameObject);
    }
}
