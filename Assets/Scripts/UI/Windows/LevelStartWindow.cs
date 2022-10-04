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
        [SerializeField] private BuyBoostView _moneyBoostView;
        [SerializeField] private BuyBoostView _staminaBoostView;

        private bool _boostBuyed;

        private const float FadeDuration = 1f;
        public override string LockKey => "LevelStartWindow";

        private UserInput _userInput;

        public static LevelStartWindow Show(UserInput userInput) =>
                       Game.Windows.ScreenChange<LevelStartWindow>(true, w => w.Init(userInput));

        protected void Init(UserInput userInput)
        {
            _infoText.gameObject.SetActive(false);
            _infoText.text = "TAP TO START";

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

        private void OnBuyBoostButtonClick(Boost boost, RectTransform panelClicked)
        {
            if (_boostBuyed)
                return;

            if (Game.Player.TryBuyBoost(boost))
            {
                _boostBuyed = true;

                if (boost.BuyEffect != null)
                {
                    ParticleSystem fx = Instantiate(boost.BuyEffect, Camera.main.transform);
                    fx.transform.localRotation = Quaternion.Euler(Vector3.zero);

                    fx.transform.position = Camera.main.ScreenToWorldPoint(panelClicked.transform.position);

                    var localPosition = fx.transform.localPosition;
                    localPosition.z = GameConstants.UIEffectZPosition;
                    fx.transform.localPosition = localPosition;
                    fx.Play();
                }

                Utils.WaitSeconds(0.05f)
                   .Then(() => Close());
            }

            Debug.Log("BuyBoost clicked");
        }
    }
}
