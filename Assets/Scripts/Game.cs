using Assets.Scripts.Boosts;
using Assets.Scripts.Hands;
using Assets.Scripts.Ladder;
using Assets.Scripts.Levels;
using Assets.Scripts.Localization;
using Assets.Scripts.Saves;
using Assets.Scripts.Social;
using Assets.Scripts.Social.Adverts;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using System;
using UniRx;
using UnityEngine;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Ladder.Ladder _ladder;
        [SerializeField] private HandsMover _hands;
        [Space]
        [SerializeField] private LevelDatabase _levels;
        [SerializeField] private WindowsController _windowsController;
        [SerializeField] private UserInput _userInput;
        [Space]
        [SerializeField] private GameEffects _effects;
        [SerializeField] private BoostsDatabase _boostsDatabase;
        [Space]
        [SerializeField] private Transform _backgroundObjectHolder;
        [SerializeField] private SpriteRenderer _background;
        [Space]
        [SerializeField] private LocalizationDatabase _localizationDatabase;
        [SerializeField] private GameSound _gameSound;

        private float StaminaLowEnergyFactorToShowTiredWindow = 0.5f;

        private UserData _user;
        private Saver _saver;
        private Player _player;
        private GameLocalization _gameLocalization;

        private Vector2 _resolution;

        private AbstractSocialAdapter _socialAdapter;
        private AbstractAdvertisingAdapter _adverts;

        public event Action LevelCompleted;
        public event Action ScreenResized;

        public static Game Instance { get; private set; }

        public LevelConfiguration CurrentLevel { get; private set; }
        public ReactiveProperty<int> CurrentLevelId { get; private set; } = new ReactiveProperty<int>();

        public static GameLocalization Localization => Instance?._gameLocalization;
        public static WindowsController Windows => Instance._windowsController;
        public static UserData User => Instance._user;
        public static Player Player => Instance._player;
        public static BoostsDatabase BoostsDatabase => Instance._boostsDatabase;
        public static Saver Saver => Instance._saver;
        public static UserInput UserInput => Instance._userInput;
        public static HandsMover Hands => Instance._hands;
        public static GameSound Sound => Instance._gameSound;
        public static AbstractSocialAdapter Social => Instance._socialAdapter;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                return;
            }

            Destroy(this);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.F1) == true)
                User.ShowData();

            if (Input.GetKeyUp(KeyCode.F3) == true)
                SettingsWindow.Show();

            if (Input.GetKeyUp(KeyCode.F4) == true)
                Saver.RemoveAllData();
#endif

            if (_resolution.x != Screen.width || _resolution.y != Screen.height)
            {
                _resolution.x = Screen.width;
                _resolution.y = Screen.height;
                ScreenResized?.Invoke();
            }
        }

        private void Start()
        {
            Init();
        }

        private void OnApplicationFocus(bool focus)
        {
#if !UNITY_EDITOR
            if (focus == false)
                _saver.Save(_user);
#endif
        }

        private void OnDisable()
        {
            RemoveSubscribes();
        }

        public static string Localize(string key, params string[] parameters) => Localization?.Localize(key, parameters) ?? key;

        public void ChangeLocale(string local)
        {
            if (local == GameLocalization.CurrentLocale)
                return;

            _gameLocalization.LoadKeys(local, _localizationDatabase);
            User.SavedLocale = GameLocalization.CurrentLocale;
        }

        public void SetSound(bool needOn)
        {
            User.NeedSound = needOn;
            _saver.Save(User.GetData());
            _gameSound.SetSound(needOn);
        }

        public void SetSaver(Saver saver)
        {
            if (saver == null || saver.GetType() == _saver.GetType())
                return;

            if (_saver != null)
                _saver.Save(User.GetData());

            _saver = saver;
            _saver.Save(User.GetData());
        }

        private void Init()
        {
            _resolution = new Vector2(Screen.width, Screen.height);

            Utils.SetMainContainer(this);
            InitSocialAdapter();

            if (_socialAdapter != null && _socialAdapter.IsInited && _socialAdapter.IsAuthorized)
                _saver = _socialAdapter.GetSaver;
            else
                _saver = new DefaultSaver();

            _user = _saver.LoadUserData();
            _player = new Player(_user);

            _gameLocalization = new GameLocalization();

            string locale = _user.SavedLocale;

            if (locale.IsNullOrEmpty())
                locale = GameLocalization.GetSystemLocaleByCapabilities();

            _gameLocalization.LoadKeys(locale, _localizationDatabase);

            _levels.Init();
            _gameSound.Init();

            var nextLevel = _levels.GetLevelConfiguration(_user.CurrentLevelId);

            CurrentLevel = nextLevel;
            StartLevel(CurrentLevel, false);

            _hands.Failed += OnFail;
            _hands.Catched += OnCatch;
            _hands.Loosed += OnLoose;
            _hands.Completed += OnLevelComplete;
            _hands.Taked += OnStepTaked;

            _userInput.Touched += OnInputTouch;
            _userInput.Untouched += OnInputUntouch;
        }

        private void RemoveSubscribes()
        {
            _hands.Failed -= OnFail;
            _hands.Catched -= OnCatch;
            _hands.Loosed -= OnLoose;
            _hands.Completed -= OnLevelComplete;
            _hands.Taked -= OnStepTaked;
            _hands.Taked -= ShowTiredWindowIfNeed;

            _userInput.Touched -= OnInputTouch;
            _userInput.Untouched -= OnInputUntouch;

            CurrentLevelId.Dispose();
        }

        private void InitSocialAdapter()
        {
#if UNITY_WEBGL || UNITY_EDITOR
#if YANDEX_GAMES
            _socialAdapter = new YandexSocialAdapter();
            _adverts = new YandexAdvertisingAdapter();
#endif
#endif
            if (_socialAdapter != null)
            {
                _socialAdapter.Init();
                _adverts.Init(_socialAdapter);
            }
        }

        private void StartLevel(LevelConfiguration level, bool isReinit)
        {
            CurrentLevelId.Value = User.CurrentLevelId;

            if (isReinit == false)
            {
                foreach (var item in _backgroundObjectHolder.GetComponentsInChildren<LevelBackgroundObject>())
                    Destroy(item.gameObject);

                if (level.BackgroundObject != null)
                    Instantiate(level.BackgroundObject, _backgroundObjectHolder);

                Camera.main.backgroundColor = level.CameraBackgroundColor;
                _background.color = level.BackgroundColor;
            }

            _ladder.Init(level);
            Windows.HUD.Init(_hands, isReinit);

            if (CurrentLevelId.Value == 0)
                InitTutor(LevelStartWindow.Show(_userInput));
            else
                LevelStartWindow.Show(_userInput);
        }

        private void InitTutor(LevelStartWindow window)
        {
            window.ClosePromise
                .Then(() => HoldAndReleaseWindow.Show());

            _hands.Taked += ShowTiredWindowIfNeed;
        }

        private void ShowTiredWindowIfNeed(LadderStep step, Hand hand)
        {
            if (_hands.Stamina.IsLowEnergy(out float factor) && factor >= StaminaLowEnergyFactorToShowTiredWindow)
            {
                _hands.Taked -= ShowTiredWindowIfNeed;
                YouAreTiredWindow.Show(_hands.Stamina);
            }
        }

        private void OnFail()
        {
            Debug.Log("Game: on fail");
            _effects.PlayFallingEffect();

            AbstractWindow window = null;
            window = TapToCatchWindow.Show(_userInput, _hands, OnEnoughTaps);

            void OnEnoughTaps()
            {
                if (_hands.TryCatch())
                    window?.Close();
            }
        }

        private void OnStepTaked(LadderStep step, Hand hand)
        {
            if (step.Id > 1)
            {
                int count = (int)_player.CalculateEndValueWithBoosts<MoneyBoost>(GameConstants.BaseMoneyBonusForStep);
                AddMoney(count, hand);
                Windows.HUD.ShowFloatingMoney(count, hand);
            }
        }

        private void OnCatch()
        {
            _effects.StopFallingEffect();
            Saver.Save(_user.GetData());
        }

        private void OnLoose()
        {
            Debug.Log("Game Loosed");
            _userInput.SetActive(false);

            _effects.StopFallingEffect();
            Saver.Save(_user.GetData());

            RestartGame();
        }

        private void OnLevelComplete()
        {
            _userInput.SetActive(false);

            int nextLevel = CurrentLevelId.Value + 1;

            CurrentLevel = _levels.GetLevelConfiguration(nextLevel);

            _user.SetCurrentLevelId(nextLevel);
            _saver.Save(_user.GetData());

            LevelCompleteWindow.Show(OnContinueButtonClick);
            _effects.PlayKonfettiEffect();

            LevelCompleted?.Invoke();

            void OnContinueButtonClick()
            {
                if (_adverts != null)
                    _adverts.TryShowInterstitial()
                            .Then(() => StartLevel(CurrentLevel, false));
                else
                    StartLevel(CurrentLevel, false);
            }
        }

        private void RestartGame()
        {
            _ladder.Restart();
            Windows.HUD.Init(Hands, true);

            LevelStartWindow.Show(_userInput);
        }

        private void AddMoney(int count, Hand hand = null)
        {
            _user.AddMoney(count);
        }

        private void OnInputTouch()
        {
            if (UserInput.IsActive)
                _hands.TryMove();
        }

        private void OnInputUntouch()
        {
            _hands.StopMovement();
        }
    }
}
