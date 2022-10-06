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

        private static TapToCatchWindow _window;

        private UserInput _userInput;
        private HandsMover _hands;

        private int _tapCounter;

        private float _lastClickTime;
        private bool _cathced;

        private event Action _enoughTapsRecived;

        public static TapToCatchWindow Show(UserInput userInput, HandsMover hands, Action onEnoughTapsRecived) =>
                    Game.Windows.ScreenChange<TapToCatchWindow>(true, w => w.Init(userInput, hands, onEnoughTapsRecived));

        protected void Init(UserInput userInput, HandsMover hands, Action onEnoughTapsRecived)
        {
            if (_window != null)
                _window.Close();

            _window = this;

            Debug.Log("Tap to catch window init...");
            _infoText.text = "TAP TAP TAP";
            _enoughTapsRecived = onEnoughTapsRecived;

            _hands = hands;
            _hands.Catched += OnCatch;

            _userInput = userInput;
            _userInput.Touched += OnStartTouch;

            ScalePongAnimation textAnimation = new ScalePongAnimation(_infoText.transform as RectTransform);
        }

        private void OnCatch()
        {
            _cathced = true;

            _tapCounter = 0;
            Close();
        }

        private void OnStartTouch()
        {
            if (_cathced)
                return;

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

        protected override void OnClose()
        {
            _window = null;

            _userInput.Touched -= OnStartTouch;
            _hands.Catched -= Close;
        }
    }
}
