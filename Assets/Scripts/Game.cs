using Assets.Scripts.Boosts;
using Assets.Scripts.Hands;
using Assets.Scripts.Ladder;
using Assets.Scripts.Levels;
using Assets.Scripts.Localization;
using Assets.Scripts.Social;
using Assets.Scripts.Social.Adverts;
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
        [SerializeField] private LocalizationDatabase _localizationDatabase;

        private float StaminaLowEnergyFactorToShowTiredWindow = 0.5f;

        private UserData _user;
        private Saver _saver;
        private Player _player;

        private AbstractSocialAdapter _socialAdapter;
        private AbstractAdvertisingAdapter _adverts;

        public event Action LevelCompleted;

        public static Game Instance { get; private set; }

        public GameLocalization GameLocalization { get; internal set; }
        public static GameLocalization Localization => Instance?.GameLocalization;

        public LevelConfiguration CurrentLevel { get; private set; }
        public ReactiveProperty<int> CurrentLevelId { get; private set; } = new ReactiveProperty<int>();

        public static WindowsController Windows => Instance._windowsController;
        public static UserData User => Instance._user;
        public static Player Player => Instance._player;
        public static BoostsDatabase BoostsDatabase => Instance._boostsDatabase;
        public static Saver Saver => Instance._saver;
        public static UserInput UserInput => Instance._userInput;
        public static HandsMover Hands => Instance._hands;

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

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F1) == true)
                ChangeLocale(Locale.EN);

            if (Input.GetKeyUp(KeyCode.F2) == true)
                ChangeLocale(Locale.RU);

            if (Input.GetKeyUp(KeyCode.F3) == true)
                SettingsWindow.Show();

            if (Input.GetKeyUp(KeyCode.F4) == true)
                Saver.RemoveAllData();
        }
#endif

        private void Start()
        {
            Init();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus == false)
                _saver.Save(_user);
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

            GameLocalization.LoadKeys(local, _localizationDatabase);
            User.SavedLocale = GameLocalization.CurrentLocale;
        }

        private void Init()
        {
            Utils.SetMainContainer(this);
            InitSocialAdapter();

            _saver = new Saver();
            _user = _saver.LoadUserData();
            _player = new Player(_user);

            GameLocalization = new GameLocalization();

            string locale = _user.SavedLocale;

            if (locale.IsNullOrEmpty())
                locale = GameLocalization.GetSystemLocaleByCapabilities();

            GameLocalization.LoadKeys(locale, _localizationDatabase);

            _levels.Init();

            var nextLevel = _levels.GetLevelConfiguration(_user.CurrentLevelId);

            CurrentLevel = nextLevel;
            StartLevel(CurrentLevel, false);

            _hands.Failed += OnFail;
            _hands.Catched += OnCatch;
            _hands.Loosed += OnLoose;
            _hands.Completed += OnLevelComplete;
            _hands.Taked += OnStepTaked;

            _userInput.Touched += _hands.TryMove;
            _userInput.Untouched += _hands.StopMovement;
        }

        private void RemoveSubscribes()
        {
            _hands.Failed -= OnFail;
            _hands.Catched -= OnCatch;
            _hands.Loosed -= OnLoose;
            _hands.Completed -= OnLevelComplete;
            _hands.Taked -= OnStepTaked;
            _hands.Taked -= ShowTiredWindowIfNeed;

            _userInput.Touched -= _hands.TryMove;
            _userInput.Untouched -= _hands.StopMovement;

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

                Camera.main.backgroundColor = level.BackgroundColor;
            }

            _ladder.Init(level);
            Windows.HUD.Init(_hands);

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
            _effects.PlayFallingEffect();

            TapToCatchWindow.Show(_userInput, _hands, OnEnoughTaps);

            void OnEnoughTaps()
            {
                _hands.TryCatch();
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
            Saver.Save(_user);
        }

        private void OnLoose()
        {
            Debug.Log("Game Loosed");
            _userInput.gameObject.SetActive(false);

            _effects.StopFallingEffect();
            Saver.Save(_user);

            RestartGame();
        }

        private void OnLevelComplete()
        {
            _userInput.gameObject.SetActive(false);

            int nextLevel = CurrentLevelId.Value + 1;

            CurrentLevel = _levels.GetLevelConfiguration(nextLevel);

            _user.SetCurrentLevelId(nextLevel);
            _saver.Save(_user);

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
            Windows.HUD.Init(Hands);

            LevelStartWindow.Show(_userInput);
        }

        private void AddMoney(int count, Hand hand = null)
        {
            _user.AddMoney(count);
        }
    }
}
