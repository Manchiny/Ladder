using System;
using UniRx;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Ladder _ladder;
    [SerializeField] private HandsMover _hands;
    [Space]
    [SerializeField] private LevelDatabase _levels;
    [SerializeField] private WindowsController _windowsController;
    [SerializeField] private UserInput _userInput;
    [Space]
    [SerializeField] private GameEffects _effects;
    [SerializeField] private BoostsDatabase _boostsDatabase;

    private UserData _user;
    private Saver _saver;
    private Player _player;

    public static Game Instance { get; private set; }
    public static WindowsController Windows => Instance._windowsController;
    public static UserData User => Instance._user;
    public static Player Player => Instance._player;
    public static BoostsDatabase BoostsDatabase => Instance._boostsDatabase;
    public static Saver Saver => Instance._saver;
    public static UserInput UserInput => Instance._userInput;

    public ReactiveProperty<LevelConfiguration> CurrenLevel { get; private set; } = new ReactiveProperty<LevelConfiguration>();

    private void Awake()
    {
        if(Instance == null)
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
            LevelCompleteWindow.Show(null);

        if (Input.GetKeyUp(KeyCode.F4) == true)
            Saver.RemoveAllData();

    }
#endif

    private void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        _hands.Failed -= OnFail;
        _hands.Catched -= OnCatch;
        _hands.Loosed -= OnLoose;
        _hands.Completed -= OnLevelComplete;
        _hands.Taked -= OnStepTaked;

        _userInput.Touched -= _hands.TryMove;
        _userInput.Untouched -= _hands.StopMovement;

        CurrenLevel.Dispose();
    }

    private void Init()
    {
        _saver = new Saver();
        _user = _saver.LoadUserData();
        _player = new Player(_user);

        Utils.SetMainContainer(this);

        _levels.Init();
        Windows.Init();

        CurrenLevel.Value = _levels.GetLevelConfiguration(_user.CurrentLevelId);
        StartLevel(CurrenLevel.Value);
  
        _hands.Failed += OnFail;
        _hands.Catched += OnCatch;
        _hands.Loosed += OnLoose;
        _hands.Completed += OnLevelComplete;
        _hands.Taked += OnStepTaked;

        _userInput.Touched += _hands.TryMove;
        _userInput.Untouched += _hands.StopMovement; 
    }

    private void StartLevel(LevelConfiguration level)
    {
        Camera.main.backgroundColor = level.BackgroundColor;
        _ladder.Init(level);
        Windows.HUD.Init(_hands);

        LevelStartWindow.Show(_userInput);
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
        if(step.Id > 1)
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

        CurrenLevel.Value = _levels.GetNextLevelConfiguration(CurrenLevel.Value);

        _user.SetCurrentLevelId(CurrenLevel.Value);
        _saver.Save(_user);

        LevelCompleteWindow.Show(() => StartLevel(CurrenLevel.Value));
    }

    private void RestartGame()
    {
        _ladder.Restart();
        LevelStartWindow.Show(_userInput);
    }

    private void AddMoney(int count, Hand hand = null)
    {
        _user.AddMoney(count);

        if(hand != null)
        {
            // ύττεκς;
        }
    }
}
