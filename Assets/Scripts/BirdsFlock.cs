using DG.Tweening;
using UnityEngine;

public class BirdsFlock : MonoBehaviour
{
    [SerializeField] private float _distance = 60f;
    
    private const float Speed = 20f;

    private const float RotateAngle = 180f;
    private const float RotationDuration = 0.5f;

    private float _moveDuration;

    private void Start()
    {     
        _moveDuration = _distance / Speed;
        StartFly();
    }

    private void StartFly()
    {
        var sequence = DOTween.Sequence().SetEase(Ease.Linear).SetLink(gameObject).SetLoops(-1);

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.forward * _distance;
        float targetX = startPosition.x + targetPosition.x;

        Vector3 startRotation = transform.rotation.eulerAngles;
        Vector3 targetRotation = startRotation;
        targetRotation.y += RotateAngle;

        sequence.Append(transform.DOMoveX(targetX, _moveDuration));
        sequence.Append(transform.DORotate(targetRotation, RotationDuration));
        sequence.Append(transform.DOMoveX(startPosition.x, _moveDuration));
        sequence.Append(transform.DORotate(startRotation, RotationDuration));

        sequence.Play();
    }
}
