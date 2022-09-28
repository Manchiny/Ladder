using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Ladder _ladder;
    [SerializeField] private HandsMover _hands;
    [Space]
    [SerializeField] private LevelDatabase _levels;

    private LevelConfiguration _currentLevel;

    public static Game Instance { get; private set; }

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
    }

    private void Init()
    {
        _levels.Init();
        _currentLevel = _levels.GetLevelConfiguration(0);

        _ladder.Init(_currentLevel);

        _hands.Loosed += OnLoose;
        _hands.Failed += OnFail;
    }

    private void OnFail()
    {

    }

    private void OnLoose()
    {
        Debug.Log("Game Loosed");
        RestartGame();
    }

    private void RestartGame()
    {
        _ladder.Restart();
    }

}
