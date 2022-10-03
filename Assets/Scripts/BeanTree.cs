using DG.Tweening;
using UnityEngine;

public class BeanTree : MonoBehaviour
{
    private const float RotationTarget = 1f;
    private const float RotationDuration = 1f;

    private void Start()
    {
        Vector3 rotation = transform.localRotation.eulerAngles;
        rotation.x = RotationTarget;

        transform.localRotation = Quaternion.Euler(rotation);
        StartAnimation();
    }

    private void StartAnimation()
    {
        Vector3 rotation = transform.localRotation.eulerAngles;
        Vector3 targetRotation = rotation;
        targetRotation.x = -rotation.x;

        var sequence = DOTween.Sequence().SetEase(Ease.Linear).SetLink(gameObject).SetLoops(-1);

        sequence.Append(transform.DOLocalRotate(targetRotation, RotationDuration));
        sequence.Append(transform.DOLocalRotate(rotation, RotationDuration));
    }
}
