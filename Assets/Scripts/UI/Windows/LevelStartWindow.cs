using Assets.Scripts.Boosts;
using DG.Tweening;
using RSG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LevelStartWindow : AbstractWindow
    {
        [SerializeField] private TextMeshProUGUI _infoText;
        [SerializeField] private Image _vignette;
        [Space]
        [SerializeField] private RectTransform _boostButtonsContainer;
        [SerializeField] private BuyBoostView _moneyBoostView;
        [SerializeField] private BuyBoostView _staminaBoostView;

        private const float FadeDuration = 1f;
        private const string TapToStartKey = "tapToStart";

        private bool _boostBuyed;
        private UserInput _userInput;

        public override string LockKey => "LevelStartWindow";

        public static LevelStartWindow Show(UserInput userInput) =>
                       Game.Windows.ScreenChange<LevelStartWindow>(true, w => w.Init(userInput));
        protected void Init(UserInput userInput)
        {
            _infoText.gameObject.SetActive(false);
            SetText();

            Boost moneyBoost = Game.BoostsDatabase.GetBoost(Boost.BoostType.MoneyBoost);
            _moneyBoostView.Init(moneyBoost, () => OnBuyBoostButtonClick(moneyBoost, _moneyBoostView.transform as RectTransform));

            Boost stamina = Game.BoostsDatabase.GetBoost(Boost.BoostType.StaminaBoost);
            _staminaBoostView.Init(stamina, () => OnBuyBoostButtonClick(stamina, _staminaBoostView.transform as RectTransform));

            _userInput = userInput;
            _userInput.Touched += OnStartTouch;

            FadeOut()
                .Then(() => ActivateInput());
        }

        protected override void OnClose()
        {
            ActivateInput();
        }

        protected override void OnHide()
        {
            _boostButtonsContainer.gameObject.SetActive(false);
        }

        protected override void OnUnhide()
        {
            base.OnUnhide();
            _boostButtonsContainer.gameObject.SetActive(true);
        }

        protected override void SetText()
        {
            _infoText.text = TapToStartKey.Localize();
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
            _userInput.SetActive(true);
        }

        private void OnStartTouch()
        {
            Close();
        }

        private void OnBuyBoostButtonClick(Boost boost, RectTransform panelClicked)
        {
            if (_boostBuyed)
                return;

            if (Game.Player.TryBuyBoost(boost))
            {
                _boostBuyed = true;

                if (boost.BuyEffect != null)
                    Instantiate(boost.BuyEffect, panelClicked.transform.position, Quaternion.identity, Game.Windows.HUD.transform);

                Utils.WaitSeconds(0.05f)
                   .Then(() => Close());
            }

            Debug.Log("BuyBoost clicked");
        }
    }
}
