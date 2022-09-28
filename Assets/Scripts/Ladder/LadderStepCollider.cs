using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LadderStepCollider : MonoBehaviour
{
    private LadderStep _ladderStep;

    private void Awake()
    {
        _ladderStep = GetComponentInParent<LadderStep>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out HandChecker checker))
           _ladderStep.OnTaked();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out HandChecker checker))
            _ladderStep.OnRelease();
    }
}
