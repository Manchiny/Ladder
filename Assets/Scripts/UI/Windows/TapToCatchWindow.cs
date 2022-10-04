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

        private UserInput _userInput;
        private int _tapCounter;
        private HandsMover _hands;

        private float _lastClickTime;

        private event Action _enoughTapsRecived;

        public static TapToCatchWindow Show(UserInput userInput, HandsMover hands, Action onEnoughTapsRecived) =>
                    Game.Windows.ScreenChange<TapToCatchWindow>(true, w => w.Init(userInput, hands, onEnoughTapsRecived));

        protected void Init(UserInput userInput, HandsMover hands, Action onEnoughTapsRecived)
        {
            Debug.Log("Tap to catch window init...");
            _infoText.text = "TAP TAP TAP";
            _enoughTapsRecived = onEnoughTapsRecived;

            _hands = hands;
            _hands.Catched += Close;

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

            if (_tapCounter >= GameConstants.NeedTapsToCatch)
                _enoughTapsRecived?.Invoke();
        }

        private void OnEndTouch()
        {

        }

        protected override void OnClose()
        {
            _userInput.Touched -= OnStartTouch;
            _userInput.Untouched -= OnEndTouch;

            _hands.Catched -= Close;
        }
    }
}
