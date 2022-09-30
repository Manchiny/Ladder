using RSG;
using System;
using TMPro;
using UnityEngine;

public class TapToCatchWindow : AbstractWindow
{
    [SerializeField] private TextMeshProUGUI _infoText;

    public override string LockKey => "TapToCatchWindow";

    private UserInput _userInput;
    private int _tapCounter;

    private float _lastClickTime;

    private event Action _enoughTapsRecived;

    public static TapToCatchWindow Show(UserInput userInput, Action onEnoughTapsRecived) =>
                Game.Windows.ScreenChange<TapToCatchWindow>(true, w => w.Init(userInput, onEnoughTapsRecived));

    protected void Init(UserInput userInput, Action onEnoughTapsRecived)
    {
        _infoText.text = "TAP TAP TAP";
        _enoughTapsRecived = onEnoughTapsRecived;

        _userInput = userInput;
        _userInput.Touched += OnStartTouch;
        _userInput.Untouched += OnEndTouch;

        ScalePongAnimation textAnimation = new ScalePongAnimation(_infoText.transform as RectTransform);
    }

    private void OnStartTouch()
    {
        if (_lastClickTime > 0)
        {
            if ((Time.realtimeSinceStartup - _lastClickTime) < GameConstants.MaxSecondsBeetweenTaps)
            {
                _tapCounter++;
            }
            else
                _tapCounter = 0;
        }
        else
            _tapCounter++;

        _lastClickTime = Time.realtimeSinceStartup;
        
        if(_tapCounter >= GameConstants.NeedTapsToCatch)
        {
            _enoughTapsRecived?.Invoke();
            Close();
        }
    }

    private void OnEndTouch()
    {

    }

    protected override void OnClose()
    {
        _userInput.Touched -= OnStartTouch;
    }
}
