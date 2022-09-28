using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LevelConfiguration;

public class Ladder : MonoBehaviour
{
    [SerializeField] private LadderStepFabric _fabric;
    [SerializeField] private Transform[] _borders;
    [Space]
    [SerializeField] private HandsMover _hands;

    private LevelConfiguration _levelConfiguration;
    private List<LadderStep> _steps = new();

    public LadderStep NextFreeStep { get; private set; }

    public enum LadderSide
    {
        Left,
        Right,
        Default
    }

    private void OnDisable()
    {
        _hands.StepTaked -= OnStepRelease;
    }

    public void Init(LevelConfiguration levelConfiguration)
    {
        _levelConfiguration = levelConfiguration;
        _fabric.Init();

        _hands.StepTaked += OnStepRelease;
        ConfigureLadder();
        InitHands(false);
    }

    public void Restart()
    {
        InitHands(true);
    }

    private void ConfigureLadder()
    {
        ResizeBorders();
        CreateSteps();
    }

    private void InitHands(bool isReinit)
    {
        _hands.Init(_steps[0], _steps[1], isReinit);
        NextFreeStep = _steps[2];
    }


    private void ResizeBorders()
    {
        float totalHeight = GameConstants.LadderDeltaStep * (_levelConfiguration.StepsCount + 2);

        Debug.Log($"Ladder total height = {totalHeight}");

        foreach (var border in _borders)
        {
            Vector3 borderPosition = border.transform.position;
            Vector3 borderScale = border.transform.localScale;

            borderScale.y = totalHeight;
            borderPosition.y = totalHeight / 2f;

            border.transform.position = borderPosition;
            border.transform.localScale = borderScale;
        }
    }

    private void CreateSteps()
    {
        float currentHeight = 0f;
        Vector3 position = Vector3.zero;

        for (int i = 0; i < _levelConfiguration.StepsCount; i++)
        {
            position.y = currentHeight;

            LadderStep step = null;
            LadderStepType type = _levelConfiguration.GetRandomType(i);

            step = _fabric.CreateStep(type, position, SideByStepId(i));

            currentHeight += GameConstants.LadderDeltaStep;

            step.Init(i);
            _steps.Add(step);
        }

        position.y = currentHeight;
        var finishStep = _fabric.CreateFinishStep(position);
        finishStep.Init(_levelConfiguration.StepsCount);
        _steps.Add(finishStep);
        currentHeight += GameConstants.LadderDeltaStep;

        position.y = currentHeight;
        var finishButton = _fabric.CreateFinishButton(position);
        finishButton.Init(_levelConfiguration.StepsCount + 1);
        _steps.Add(finishButton);

        _steps.OrderBy(step => step.Id);
    }

    private void OnStepRelease(LadderStep step)
    {
        //if (_steps.Count > step.Id - 1)
        NextFreeStep = _steps.Where(s => s.Id == step.Id + 1).FirstOrDefault();
        Debug.Log($"Last step: {step.Id}, NextStep: {NextFreeStep?.Id}");
    }

    private LadderSide SideByStepId(int stepId) => stepId % 2 == 0 ? LadderSide.Right : LadderSide.Left;
}
