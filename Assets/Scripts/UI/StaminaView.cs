using DG.Tweening;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class StaminaView : MonoBehaviour
{
    [SerializeField] private Image _vignette;

    private const float PercentTosStartShowVignette = 20f;
    private const float FadeDuration = 0.2f;

    private Stamina _stamina;

    private IDisposable _setFadeDispose;
    private Tween _fadeAnimationSequence;

    private float _lastValue;

    private void OnDisable()
    {
        _setFadeDispose.Dispose();
    }

    public void Init(Stamina stamina)
    {
        if (_stamina == null)
        {
            _stamina = stamina;
            _setFadeDispose = _stamina.CurrentEnergy.ObserveEveryValueChanged(value => value.Value).Subscribe(OnStaminaValueChanged).AddTo(this);
        }

        _lastValue = 0;
        _vignette.DOFade(0, 0);
    }

    private void OnStaminaValueChanged(float value)
    {
        _lastValue = value;

        if (_fadeAnimationSequence != null && _fadeAnimationSequence.IsActive())
            return;

        PlayFade(value);
    }

    private void PlayFade(float value)
    {
        float valuePercent = value * 100f / Stamina.MaxEnergyValue;
        float alpha = 0;

        if (valuePercent <= PercentTosStartShowVignette)
        {
            alpha = 1 - valuePercent / PercentTosStartShowVignette;
            Debug.Log("Alpha = " + alpha);
        }

        _fadeAnimationSequence = _vignette.DOFade(alpha, FadeDuration).SetEase(Ease.Linear).SetLink(gameObject);
        _fadeAnimationSequence.Play();

        if (_lastValue != value)
            PlayFade(_lastValue);
    }
}
