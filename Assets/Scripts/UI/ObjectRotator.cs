using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class ObjectRotator : MonoBehaviour
    {
        private const float RotationDuration = 3f;

        private void Start()
        {
            if(gameObject != null)
                transform.DOLocalRotate(new Vector3(0, 0, -360), RotationDuration, RotateMode.FastBeyond360)
                            .SetRelative(true)
                            .SetEase(Ease.Linear)
                            .SetLink(gameObject)
                            .SetLoops(-1, LoopType.Restart);
        }
    }
}
