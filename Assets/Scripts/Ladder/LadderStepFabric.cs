using System.Collections.Generic;
using UnityEngine;
using static Ladder;
using static LevelConfiguration;

public class LadderStepFabric : MonoBehaviour
{
    [SerializeField] private LadderStep _defaultLadderStepPrefab;
    [SerializeField] private LadderStepDynamicHalfs _ladderStepDynamic;
    [SerializeField] private LadderStepHalfs _ladderStepHalfs;
    [SerializeField] private LadderStep _finishLadderStepPrefab;
    [SerializeField] private FinishButtonStep _finishButtonPrefab;

    private Dictionary<LadderStepType, LadderStep> _prefabs = new();

    public void Init()
    {
        _prefabs.TryAdd(LadderStepType.Default, _defaultLadderStepPrefab);
        _prefabs.TryAdd(LadderStepType.HalfDefualt, _ladderStepHalfs);
        _prefabs.TryAdd(LadderStepType.HalfDynamic, _ladderStepDynamic);
    }

    public LadderStep CreateStep(LadderStepType type, Vector3 position, LadderSide side = LadderSide.Default)
    {
        if(_prefabs.TryGetValue(type, out LadderStep step))
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
