using UnityEngine;
using static Ladder;

public class LadderStepFabric : MonoBehaviour
{
    [SerializeField] private LadderStep _defaultLadderStepPrefab;
    [SerializeField] private LadderStepDynamicHalfs _ladderStepDynamic;
    [SerializeField] private LadderStepHalfs _ladderStepHalfs;
    [SerializeField] private LadderStep _finishLadderStepPrefab;
    [SerializeField] private FinishButtonStep _finishButtonPrefab;

    public LadderStep CreateDefaultStep(Vector3 position) => CreateStep(_defaultLadderStepPrefab, position);
    public LadderStep CreateDynamicStep(LadderSide side, Vector3 position) => CreateStep(_ladderStepDynamic.GetPrefab(side), position);
    public LadderStep CreateHalfStep(LadderSide side, Vector3 position) => CreateStep(_ladderStepHalfs.GetPrefab(side), position);
    public LadderStep CreateFinishStep(Vector3 position) => CreateStep(_finishLadderStepPrefab, position);
    public LadderStep CreateFinishButton(Vector3 position) => CreateStep(_finishButtonPrefab, position);

    private LadderStep CreateStep(LadderStep stepPrefab, Vector3 position)
    {
        return Instantiate(stepPrefab, position, Quaternion.identity, transform);
    }
}
