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

    private LevelConfiguration _currentLevel;
    private UserData _user;
    private Saver _saver;

    public static Game Instance { get; private set; }
    public static WindowsController Windows => Instance._windowsController;

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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1) == true)
            LevelCompleteWindow.Show(null);
    }

    private void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        _hands.Loosed -= OnLoose;
        _hands.Failed -= OnFail;
        _hands.Completed -= OnLevelComplete;
        _hands.Catched -= OnCatch;

        _userInput.Touched -= _hands.TryMove;
        _userInput.Untouched -= _hands.StopMovement;
    }

    private void Init()
    {
        _saver = new Saver();
        _user = _saver.LoadUserData();

        Utils.SetMainContainer(this);

        _levels.Init();
        Windows.Init();

        _currentLevel = _levels.GetLevelConfiguration(_user.CurrentLevelId);
        StartLevel(_currentLevel);
  
        _hands.Loosed += OnLoose;
        _hands.Failed += OnFail;
        _hands.Completed += OnLevelComplete;
        _hands.Catched += OnCatch;

        _userInput.Touched += _hands.TryMove;
        _userInput.Untouched += _hands.StopMovement; 
    }

    private void StartLevel(LevelConfiguration level)
    {
        Camera.main.backgroundColor = level.BackgroundColor;
        _ladder.Init(level);
        Windows.HUD.Init(level.Id + 1);

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

    private void OnCatch()
    {
        _effects.StopFallingEffect();
    }

    private void OnLoose()
    {
        Debug.Log("Game Loosed");
        _userInput.gameObject.SetActive(false);

        _effects.StopFallingEffect();

        RestartGame();
    }

    private void OnLevelComplete()
    {
        _userInput.gameObject.SetActive(false);

        _currentLevel = _levels.GetNextLevelConfiguration(_currentLevel);

        _user.SetCurrentLevelId(_currentLevel);
        _saver.Save(_user);

        LevelCompleteWindow.Show(() => StartLevel(_currentLevel));
    }

    private void RestartGame()
    {
        _ladder.Restart();
        LevelStartWindow.Show(_userInput);
    }

}
