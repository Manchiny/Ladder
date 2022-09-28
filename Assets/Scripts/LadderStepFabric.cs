using UnityEngine;
using static Ladder;

public class LadderStepFabric : MonoBehaviour
{
    [SerializeField] private LadderStep _defaultLadderStepPrefab;
    [SerializeField] private LadderStepDynamic _ladderStepDynamic;

    public LadderStep CreateDefaultStep(Vector3 position) => CreateStep(_defaultLadderStepPrefab, position);

    public LadderStep CreateDynamicStep(LadderSide side, Vector3 position) => CreateStep(_ladderStepDynamic.GetPrefab(side), position);

    private LadderStep CreateStep(LadderStep stepPrefab, Vector3 position)
    {
        return Instantiate(stepPrefab, position, Quaternion.identity, transform);
    }
}
