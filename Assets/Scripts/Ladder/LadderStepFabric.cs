using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Ladder.Ladder;
using static Assets.Scripts.Levels.LevelConfiguration;

namespace Assets.Scripts.Ladder
{
    [CreateAssetMenu]
    public class LadderStepFabric : ScriptableObject
    {
        [SerializeField] private LadderStep _defaultLadderStepPrefab;
        [SerializeField] private LadderStepDynamicHalfs _ladderStepDynamic;
        [SerializeField] private LadderStepHalfs _ladderStepHalfs;
        [SerializeField] private LadderStepHot _ladderStepHot;
        [SerializeField] private SpikyLadderStep _spikyLadderStep;
        [Space]
        [SerializeField] private LadderStep _finishLadderStepPrefab;
        [SerializeField] private FinishButtonStep _finishButtonPrefab;

        private Transform _container;
        private Dictionary<LadderStepType, LadderStep> _prefabs = new();

        public void Init(Transform stepsContainer)
        {
            _container = stepsContainer;

            _prefabs.TryAdd(LadderStepType.Default, _defaultLadderStepPrefab);
            _prefabs.TryAdd(LadderStepType.HalfDefualt, _ladderStepHalfs);
            _prefabs.TryAdd(LadderStepType.HalfDynamic, _ladderStepDynamic);
            _prefabs.TryAdd(LadderStepType.Hot, _ladderStepHot);
            _prefabs.TryAdd(LadderStepType.Spiky, _spikyLadderStep);
            _prefabs.TryAdd(LadderStepType.FinishStep, _finishLadderStepPrefab);
            _prefabs.TryAdd(LadderStepType.FinishButton, _finishButtonPrefab);
        }

        public LadderStep CreateStep(LadderStepType type, Vector3 position, LadderSide side = LadderSide.Default)
        {
            if (_prefabs.TryGetValue(type, out LadderStep step))
            {
                return CreateStep(step.GetPrefab(side), position);
            }
            else
            {
                Debug.LogError($"Can't create ladder step! Prefabs don't contains prefab type {type}!");
                return null;
            }
        }

        private LadderStep CreateStep(LadderStep stepPrefab, Vector3 position) => Instantiate(stepPrefab, position, Quaternion.identity, _container);
    }
}
