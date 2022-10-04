using Assets.Scripts.Hands;
using UnityEngine;

namespace Assets.Scripts.LevelLadder
{
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
                _ladderStep.OnHandCollided(checker);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out HandChecker checker))
                _ladderStep.OnHandExit(checker);
        }
    }
}
