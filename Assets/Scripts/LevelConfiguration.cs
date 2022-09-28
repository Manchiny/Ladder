using System;
using System.Collections.Generic;
using UnityEngine;
using static LevelConfiguration;

[CreateAssetMenu]
public class LevelConfiguration : ScriptableObject
{
    [Header("Ladder configuration")]
    [SerializeField] private int _ladderStepsCount = 50;
    [Space]
    [SerializeField] private List<LadderStepsChanse> _notDefaultLadderSteps;

    private const int MaxSummaryNonDefualtStepsChance = 100;

    private Dictionary<LadderStepType, int> _notDefualtStepsChanses = new();
    private System.Random _random = new System.Random();

    public int Id { get; private set; }

    public int StepsCount => _ladderStepsCount;

    private void Awake()
    {
#if UNITY_EDITOR
        ValidateLadderConfiguration();
#endif
    }

    public enum LadderStepType
    {
        Default,
        HalfDefualt,
        HalfDynamic
    }


    public void Init(int id)
    {
        Id = id;

        foreach (var step in _notDefaultLadderSteps)
        {
            if (_notDefualtStepsChanses.ContainsKey(step.StepType))
                Debug.LogError($"Incorrect level configuration! NotDefaultLadderSteps contains doubled type <{step.StepType}>! Level id - {Id}, Name: {name}");
            else
                _notDefualtStepsChanses.Add(step.StepType, step.Chanse);
        }
    }

    public LadderStepType GetRandomType(int stepNumber)
    {
        int randomValue = _random.Next(0, MaxSummaryNonDefualtStepsChance);

        LadderStepType type = LadderStepType.Default;

        if (stepNumber >= GameConstants.MinNonDefaultStepId)
        {
            int lastSummValue = 0;
            int chanseSumm = 0;

            foreach (var step in _notDefualtStepsChanses)
            {
                lastSummValue = chanseSumm;
                chanseSumm += step.Value;

                if (randomValue > lastSummValue && randomValue <= chanseSumm)
                    return step.Key;
            }
        }

        return type;
    }

    private void ValidateLadderConfiguration()
    {
        int summaryNonDefualtStepsChannce = 0;

        foreach (var step in _notDefualtStepsChanses)
            summaryNonDefualtStepsChannce += step.Value;

        if (summaryNonDefualtStepsChannce > MaxSummaryNonDefualtStepsChance)
        {
            Debug.LogError($"Incorrect level configuration - summaryNonDefualtStepsChannce >  MaxSummaryNonDefualtStepsChanceLevel! id - {Id}, Name: {name}");
        }
    }

}

[Serializable]
public class LadderStepsChanse
{
    [SerializeField] private LadderStepType _stepType;
    [SerializeField] private int _chanse;

    public LadderStepType StepType => _stepType;
    public int Chanse => _chanse;
}
