using DG.Tweening;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private const float MoveDuration = 0.5f;

    public bool _isProcess;

    private float _lastHeight;
    private float _needHeight;

    public bool CanMove => _isProcess == false;
    public float GetHeight => transform.position.y;

    private void Start()
    {
        _lastHeight = transform.position.y;
    }

    public bool TryMove(float deltaHeight)
    {
        if (_isProcess)
            return false;

        _needHeight = _lastHeight + deltaHeight;
        _lastHeight = _needHeight;
        MoveUp();

        return true;
    }

    private void MoveUp()
    {
        _isProcess = true;
        transform.DOMoveY(_needHeight, MoveDuration).SetLink(gameObject).SetEase(Ease.Linear).OnComplete(() => _isProcess = false);
    }
}
