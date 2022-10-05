using Assets.Scripts.Hands;
using Assets.Scripts.Ladder;
using DG.Tweening;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class StaminaView : MonoBehaviour
    {
        [SerializeField] private Image _vignette;

        private const float FadeDuration = 0.2f;

        private const float ShakeCameraDuration = 0.15f;
        private const float MaxCameraShakeStright = 0.2f;
        private const int CameaShakeVibraion = 3;

        private Stamina _stamina;
        private HandsMover _hands;
        private Camera _camera;

        private IDisposable _setFadeDispose;
        private Tween _fadeAnimation;

        private float _lastValue;
        private float _isLowEnergyValue;

        private void OnDisable()
        {
            if (_setFadeDispose != null)
                _setFadeDispose.Dispose();

            if (_hands != null)
                _hands.Taked -= ShakeCameraOnTake;
        }

        public void Init(HandsMover hands)
        {
            if (_hands == null)
            {
                _camera = Camera.main;

                _hands = hands;
                _stamina = _hands.Stamina;

                _hands.Taked += ShakeCameraOnTake;
                _setFadeDispose = _stamina.CurrentEnergy.ObserveEveryValueChanged(value => value.Value).Subscribe(OnStaminaValueChanged).AddTo(this);
            }

            _fadeAnimation?.Kill();
            OnStaminaValueChanged(Stamina.MaxEnergyValue);
        }

        private void OnStaminaValueChanged(float value)
        {
            _lastValue = value;

            if (_fadeAnimation != null && _fadeAnimation.IsActive())
                return;

            PlayFade(value);
        }

        private void PlayFade(float value)
        {
            if (_stamina.IsLowEnergy(out float alpha))
                _isLowEnergyValue = alpha;
            else
                _isLowEnergyValue = 0;

            _fadeAnimation = _vignette.DOFade(alpha, FadeDuration).SetEase(Ease.Linear).SetLink(gameObject);
            _fadeAnimation.Play();

            if (_lastValue != value)
                PlayFade(_lastValue);
        }

        private void ShakeCameraOnTake(LadderStep step, Hand hand)
        {
            if (_isLowEnergyValue > 0)
                _camera.DOShakePosition(_isLowEnergyValue * ShakeCameraDuration, _isLowEnergyValue * MaxCameraShakeStright, CameaShakeVibraion);
        }
    }
}
