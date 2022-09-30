using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Ladder _ladder;
    [SerializeField] private HandsMover _hands;
    [Space]
    [SerializeField] private LevelDatabase _levels;
    [SerializeField] private WindowsController _windowsController;
    [SerializeField] private UserInput _userInput;

    private LevelConfiguration _currentLevel;

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

    private void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        _hands.Loosed -= OnLoose;
        _hands.Failed -= OnFail;

        _userInput.Touched -= _hands.TryMove;
        _userInput.Untouched -= _hands.StopMovement;
    }

    private void Init()
    {
        Utils.SetMainContainer(this);

        _levels.Init();
        Windows.Init(); 

        _currentLevel = _levels.GetLevelConfiguration(0);

        Camera.main.backgroundColor = _currentLevel.BackgroundColor;
        _ladder.Init(_currentLevel);
        Windows.HUD.Init(_currentLevel.Id + 1);

        _hands.Loosed += OnLoose;
        _hands.Failed += OnFail;

        LevelStartWindow.Show(_userInput);

        _userInput.Touched += _hands.TryMove;
        _userInput.Untouched += _hands.StopMovement;

    }

    private void OnFail()
    {
        TapToCatchWindow.Show(_userInput, OnEnoughTaps);

        void OnEnoughTaps()
        {
            _hands.TryCatch();
        }
    }

    private void OnLoose()
    {
        Debug.Log("Game Loosed");
        _userInput.gameObject.SetActive(false);

        RestartGame();
    }

    private void RestartGame()
    {
        _ladder.Restart();
        LevelStartWindow.Show(_userInput);
    }

}
