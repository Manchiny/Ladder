using Assets.Scripts.Hands;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TapToCatchWindow : AbstractWindow
    {
        [SerializeField] private TextMeshProUGUI _infoText;

        public override string LockKey => "TapToCatchWindow";
        private const string TapToCatchLocalizationKey = "tapToCatch";

        private static TapToCatchWindow _window;

        private UserInput _userInput;
        private HandsMover _hands;

        private int _tapCounter;

        private float _lastClickTime;

        private event Action _enoughTapsRecived;

        public static TapToCatchWindow Show(UserInput userInput, HandsMover hands, Action onEnoughTapsRecived) =>
                    Game.Windows.ScreenChange<TapToCatchWindow>(true, w => w.Init(userInput, hands, onEnoughTapsRecived));

        private void OnDestroy()
        {
            _window = null;
            _tapCounter = 0;

            _userInput.Touched -= OnStartTouch;
            _hands.Loosed -= OnLoose;
        }

        protected void Init(UserInput userInput, HandsMover hands, Action onEnoughTapsRecived)
        {
            Debug.Log("Tap to catch window try init...");

            if (_window != null)
                _window.Close();

            _window = this;

            Debug.Log("Tap to catch window inited");

            SetText();

            _enoughTapsRecived = onEnoughTapsRecived;

            _hands = hands;

            _hands.Loosed += OnLoose;

            _userInput = userInput;
            _userInput.Touched += OnStartTouch;

            ScalePongAnimation textAnimation = new ScalePongAnimation(_infoText.transform as RectTransform);
        }

        protected override void SetText()
        {
            _infoText.text = TapToCatchLocalizationKey.Localize();
        }

        private void OnLoose()
        {
            Close();
        }

        private void OnStartTouch()
        {
            if (_lastClickTime > 0)
            {
                if ((Time.realtimeSinceStartup - _lastClickTime) < GameConstants.MaxSecondsBeetweenTaps)
                    _tapCounter++;
                else
                    _tapCounter = 0;
            }
            else
                _tapCounter++;

            _lastClickTime = Time.realtimeSinceStartup;

            if (_tapCounter >= GameConstants.NeedTapsToCatch)
                _enoughTapsRecived?.Invoke();
        }
    }
}
