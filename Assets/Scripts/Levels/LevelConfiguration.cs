using System;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Levels.LevelConfiguration;

namespace Assets.Scripts.Levels
{
    [CreateAssetMenu]
    public class LevelConfiguration : ScriptableObject
    {
        [Header("Ladder configuration")]
        [SerializeField] private int _ladderStepsCount = 50;
        [SerializeField] private int _stepsIntevalsBeetweenDefault = 5;
        [SerializeField] private int _minDefaultStepsCountInRowInInterval = 2;
        [SerializeField] private int _defaultStepChanseOnNotDefaultPosition = 10;
        [Space]
        [SerializeField] private List<LadderStepsChanse> _notDefaultLadderSteps;
        [Space]
        [SerializeField] Color _backgroundColor;
        [SerializeField] Color _cameraBackgroundColor;
        [SerializeField] private LevelBackgroundObject _levelBackgroundObject;

        private const int MaxSummaryNonDefualtStepsChance = 100;
        
        private int? _notUsedRandomValue;

        private Dictionary<LadderStepType, int> _notDefualtStepsChanses = new();
        private System.Random _random = new System.Random();

        public enum LadderStepType
        {
            Default,
            HalfDefualt,
            HalfDynamic,
            Hot,
            Spiky,
            FinishStep,
            FinishButton
        }

        public int StepsCount => _ladderStepsCount;
        public Color BackgroundColor => _backgroundColor;
        public Color CameraBackgroundColor => _cameraBackgroundColor;
        public LevelBackgroundObject BackgroundObject => _levelBackgroundObject;

        private void Awake()
        {
#if UNITY_EDITOR
            ValidateLadderConfiguration();
#endif
        }

        public void Init()
        {
            foreach (var step in _notDefaultLadderSteps)
            {
                if (_notDefualtStepsChanses.ContainsKey(step.StepType))
                    Debug.LogError($"Incorrect level configuration! NotDefaultLadderSteps contains doubled type <{step.StepType}>! Name: {name}");
                else if(step.StepType == LadderStepType.FinishButton || step.StepType == LadderStepType.FinishStep || step.StepType == LadderStepType.Default)
                    Debug.LogError($"Incorrect level configuration! NotDefaultLadderSteps don't can contain Defualt, FinishStep or FinishButton types! Name: {name}");
                else
                    _notDefualtStepsChanses.Add(step.StepType, step.Chanse);
            }
        }

        public LadderStepType GetRandomType(int stepNumber)
        {
            LadderStepType type = LadderStepType.Default;

            if (stepNumber >= GameConstants.MinNonDefaultStepId)
            {
                int randomValue = _notUsedRandomValue == null ? _random.Next(0, MaxSummaryNonDefualtStepsChance) : (int)_notUsedRandomValue;

                int maxInclusiveNonDefaultId = GameConstants.MinNonDefaultStepId + (stepNumber / _stepsIntevalsBeetweenDefault) * _stepsIntevalsBeetweenDefault - _minDefaultStepsCountInRowInInterval;
                int minInclusiveNonDefaultId = maxInclusiveNonDefaultId - _minDefaultStepsCountInRowInInterval;

                if (stepNumber >= minInclusiveNonDefaultId && stepNumber <= maxInclusiveNonDefaultId)
                {
                    int lastSummValue = 0;
                    int chanseSumm = 0;
                    _notUsedRandomValue = null;

                    foreach (var step in _notDefualtStepsChanses)
                    {
                        lastSummValue = chanseSumm;
                        chanseSumm += step.Value;

                        if (randomValue >= lastSummValue && randomValue <= chanseSumm)
                            return step.Key;
                    }

                    lastSummValue = chanseSumm;
                    chanseSumm += _defaultStepChanseOnNotDefaultPosition;

                    if (randomValue >= lastSummValue && randomValue <= chanseSumm)
                        return LadderStepType.Default;

                    int notDefaultStepsCount = _notDefualtStepsChanses.Count;
                    int randomStepId = _random.Next(0, notDefaultStepsCount-1);

                    int counter = 0;

                    foreach (var stepChanse in _notDefualtStepsChanses)
                    {
                        if (counter == randomStepId)
                            return stepChanse.Key;

                        counter++;
                    }

                }
                else if (_notDefualtStepsChanses.ContainsKey(LadderStepType.HalfDefualt))
                {
                    _notUsedRandomValue = randomValue;

                    int chanse = _notDefualtStepsChanses[LadderStepType.HalfDefualt];

                    if (randomValue <= chanse)
                        return LadderStepType.HalfDefualt;
                }
            }

            return type;
        }

        private void ValidateLadderConfiguration()
        {
            int summaryNonDefualtStepsChannce = _defaultStepChanseOnNotDefaultPosition;

            foreach (var step in _notDefualtStepsChanses)
                summaryNonDefualtStepsChannce += step.Value;

            if (summaryNonDefualtStepsChannce > MaxSummaryNonDefualtStepsChance)
                Debug.LogError($"Incorrect level configuration - summaryNonDefualtStepsChannce >  MaxSummaryNonDefualtStepsChanceLevel! Name: {name}");
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
}
