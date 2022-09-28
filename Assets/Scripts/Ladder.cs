using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private LadderStepFabric _fabric;
    [SerializeField] private Transform[] _borders;
    [Space]
    [SerializeField] private float _stepsCount;
    [SerializeField] private HandsMover _hands;

    private const int DynamicChance = 20;
    private const float LadderDeltaStep = 1.5f;

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
        float totalHeight = LadderDeltaStep * _stepsCount;

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

        for (int i = 0; i < _stepsCount; i++)
        {
            Vector3 position = Vector3.zero;
            position.y = currentHeight;

            int random = Random.Range(0, 100);

            LadderStep step;

            if (random <= DynamicChance)
               step = _fabric.CreateDynamicStep(SideByStepId(i), position);
            else
                step = _fabric.CreateDefaultStep(position);

            currentHeight += LadderDeltaStep;
            step.Init(i);
            _steps.Add(step);
        }

        _steps.OrderBy(step => step.Id);
    }

    private void OnStepRelease(LadderStep step)
    {
        if (_steps.Count > step.Id - 1)
            NextFreeStep = _steps.Where(s => s.Id == step.Id + 1).FirstOrDefault();

        Debug.Log($"Last step: {step.Id}, NextStep: {NextFreeStep.Id}");
    }

    private LadderSide SideByStepId(int stepId) => stepId % 2 == 0 ? LadderSide.Right : LadderSide.Left;
}
