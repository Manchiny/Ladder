using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private HandsMover _handsMover;
    [Space]
    [SerializeField] private float _offsetY = 4f;
    [SerializeField] private float _moveSpeed = 10f;

    private Vector3 _targetPosition;
    private Vector3 _smoothedPosition;

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        _targetPosition = transform.position;
        _targetPosition.y = _handsMover.GetAverageValue - _offsetY;

        _smoothedPosition = Vector3.Lerp(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
        transform.position = _smoothedPosition;
    }
}
