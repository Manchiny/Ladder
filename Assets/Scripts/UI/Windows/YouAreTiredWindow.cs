using System;
using TMPro;
using UniRx;
using UnityEngine;

public class YouAreTiredWindow : AbstractWindow
{
    [SerializeField] private TextMeshProUGUI _releaseHandText;
    [SerializeField] private TextMeshProUGUI _youAreTiredText;

    public override string LockKey => "YouAreTiredWindow";

    private Stamina _stamina;
    private IDisposable _checkEnergyDispose;

    public static YouAreTiredWindow Show(Stamina stamina) =>
                    Game.Windows.ScreenChange<YouAreTiredWindow>(true, w => w.Init(stamina));

    protected void Init(Stamina stamina)
    {
        _releaseHandText.text = "RELEASE TOUR HAND";
        _youAreTiredText.text = "YOU ARE TIRED";

        new ScalePongAnimation(_releaseHandText.transform as RectTransform);

        _stamina = stamina;
        _checkEnergyDispose = _stamina.CurrentEnergy.ObserveEveryValueChanged(value => value.Value).Subscribe(OnStaminaValueChanged).AddTo(this);
    }

    private void OnStaminaValueChanged(float value)
    {
        if (_stamina.IsLowEnergy(out float factor) == false)
            Close();
    }

    private void OnDestroy()
    {
        if (_checkEnergyDispose != null)
            _checkEnergyDispose.Dispose();
    }
}