using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private LadderStepFabric _fabric;
    [SerializeField] private Transform[] _borders;
    [Space]
    [SerializeField] private int _stepsCount;
    [SerializeField] private HandsMover _hands;

    private const int DynamicChance = 20;
    private const int HalfStepChance = 15;

    private List<LadderStep> _steps = new();
    public LadderStep NextFreeStep { get; private set; }

    public enum LadderSide
    {
        Left,
        Right
    }

    private void Awake()
    {
        ConfigureLadder();
    }

    private void Start()
    {
        _hands.StepTaked += OnStepRelease;
    }

    private void OnDisable()
    {
        _hands.StepTaked -= OnStepRelease;
    }

    private void ConfigureLadder()
    {
        ResizeBorders();
        CreateSteps();

        _hands.Init(_steps[0], _steps[1]);
        NextFreeStep = _steps[2];
    }


    private void ResizeBorders()
    {
        float totalHeight = GameConstants.LadderDeltaStep * (_stepsCount + 2);

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

        for (int i = 0; i < _stepsCount; i++)
        {
            position.y = currentHeight;

            int random = Random.Range(0, 100);

            LadderStep step;

            if (random <= DynamicChance)
                step = _fabric.CreateDynamicStep(SideByStepId(i), position);
            else if (random > DynamicChance && random <= HalfStepChance + DynamicChance)
                step = _fabric.CreateHalfStep(SideByStepId(i), position);
            else
                step = _fabric.CreateDefaultStep(position);

            currentHeight += GameConstants.LadderDeltaStep;
            step.Init(i);
            _steps.Add(step);
        }

        position.y = currentHeight;
        var finishStep = _fabric.CreateFinishStep(position);
        finishStep.Init(_stepsCount);
        _steps.Add(finishStep);
        currentHeight += GameConstants.LadderDeltaStep;

        position.y = currentHeight;
        var finishButton = _fabric.CreateFinishButton(position);
        finishButton.Init(_stepsCount + 1);
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
