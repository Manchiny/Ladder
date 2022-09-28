using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private HandsMover _handsMover;

    private const float OffsetY = 2.35f;
    private const float MoveSpeed = 7f;

    private Vector3 _targetPosition;
    private Vector3 _smoothedPosition;

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        _targetPosition = transform.position;
        _targetPosition.y = _handsMover.GetAverageValue - OffsetY;

        _smoothedPosition = Vector3.Lerp(transform.position, _targetPosition, MoveSpeed * Time.deltaTime);
        transform.position = _smoothedPosition;
    }
}
