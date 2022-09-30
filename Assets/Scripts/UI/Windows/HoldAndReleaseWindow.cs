using TMPro;
using UnityEngine;

public class HoldAndReleaseWindow : AbstractWindow
{
    [SerializeField] private TextMeshProUGUI _infoText;

    public override string LockKey => "HoldOndReleaseWindow";

    private bool _delayedClose;
    private AbstractWindow _window;
    private UserInput _userInput;

    public static HoldAndReleaseWindow Show(UserInput userInput) =>
                    Game.Windows.ScreenChange<HoldAndReleaseWindow>(true, w => w.Init(userInput));

    protected void Init(UserInput userInput)
    {
        _infoText.text = "HOLD & RELEASE";

        _userInput = userInput;
        _userInput.Touched += OnStartTouch;
        //_userInput.Untouched += OnEndTouch;

        ScalePongAnimation textAnimation = new ScalePongAnimation(_infoText.transform as RectTransform);
        _window = this;
    }

    private void OnStartTouch()
    {
        if (_delayedClose == true)
            return;

        _delayedClose = true;

        Utils.WaitSeconds(2f)
             .Then(() =>
             {
                 if (_window != null)
                     Close();
             });
    }

    private void OnEndTouch()
    {

    }

    protected override void OnClose()
    {
        _window = null;
        _userInput.Touched -= OnStartTouch;
    }
}
