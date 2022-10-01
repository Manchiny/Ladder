using DG.Tweening;
using RSG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelStartWindow : AbstractWindow
{
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private Image _vignette;
    [SerializeField] private Button _moneyBoostButton;

    private const float FadeDuration = 1f;
    public override string LockKey => "LevelStartWindow";

    private UserInput _userInput;

    public static LevelStartWindow Show(UserInput userInput) =>
                   Game.Windows.ScreenChange<LevelStartWindow>(true, w => w.Init(userInput));

    protected void Init(UserInput userInput)
    {
        _infoText.gameObject.SetActive(false);
        _infoText.text = "TAP TO START";

        _moneyBoostButton.onClick.AddListener(OnMoneyButtonClick);

        _userInput = userInput;
        _userInput.Touched += OnStartTouch;
        //_userInput.Untouched += OnEndTouch;  

        FadeOut()
            .Then(() => ActivateInput());
    }

    private IPromise FadeOut()
    {
        Promise promise = new Promise();

        _vignette.DOFade(0, FadeDuration).SetEase(Ease.Linear).SetLink(gameObject).OnComplete(() => promise.Resolve());

        return promise;
    }

    private void ActivateInput()
    {
        _infoText.gameObject.SetActive(true);
        _userInput.Activate();
    }

    private void OnStartTouch()
    {
        Close();
    }

    private void OnEndTouch()
    {

    }

    private void OnMoneyButtonClick()
    {
        Boost boost = Game.BoostsDatabase.GetBoost(Boost.BoostType.MoneyBoost);

        if(Game.Player.TryBuyBoost(boost))
        {
            Close();
        }

        Debug.Log("MoneyButton clicked");
    }
}
