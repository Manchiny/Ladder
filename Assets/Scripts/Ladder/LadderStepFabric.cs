using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Ladder.Ladder;
using static Assets.Scripts.Levels.LevelConfiguration;

namespace Assets.Scripts.Ladder
{
    public class LadderStepFabric : MonoBehaviour
    {
        [SerializeField] private LadderStep _defaultLadderStepPrefab;
        [SerializeField] private LadderStepDynamicHalfs _ladderStepDynamic;
        [SerializeField] private LadderStepHalfs _ladderStepHalfs;
        [SerializeField] private LadderStepHot _ladderStepHot;
        [SerializeField] private SpikyLadderStep _spikyLadderStep;
        [Space]
        [SerializeField] private LadderStep _finishLadderStepPrefab;
        [SerializeField] private FinishButtonStep _finishButtonPrefab;

        private Dictionary<LadderStepType, LadderStep> _prefabs = new();

        public void Init()
        {
            _prefabs.TryAdd(LadderStepType.Default, _defaultLadderStepPrefab);
            _prefabs.TryAdd(LadderStepType.HalfDefualt, _ladderStepHalfs);
            _prefabs.TryAdd(LadderStepType.HalfDynamic, _ladderStepDynamic);
            _prefabs.TryAdd(LadderStepType.Hot, _ladderStepHot);
            _prefabs.TryAdd(LadderStepType.Spiky, _spikyLadderStep);
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

        public LadderStep CreateFinishStep(Vector3 position) => CreateStep(_finishLadderStepPrefab, position);
        public LadderStep CreateFinishButton(Vector3 position) => CreateStep(_finishButtonPrefab, position);

        private LadderStep CreateStep(LadderStep stepPrefab, Vector3 position) => Instantiate(stepPrefab, position, Quaternion.identity, transform);
    }
}
