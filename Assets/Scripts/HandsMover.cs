using UnityEngine;

public class HandsMover : MonoBehaviour
{
    [SerializeField] private Hand _leftHand;
    [SerializeField] private Hand _rightHand;

    private const float DeltaHeight = 3f;

    private Hand _downHand;

    public float GetAverageValue => (_leftHand.GetHeight + _rightHand.GetHeight) / 2f;
    private bool _canMove => _leftHand.CanMove && _rightHand.CanMove;

    private void Start()
    {
        _downHand = _leftHand.GetHeight < _rightHand.GetHeight ? _leftHand : _rightHand;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) == true)
            TryMove();
    }

    private void TryMove()
    {
        if (_canMove == false)
            return;

        if(_downHand.TryMove(DeltaHeight))
            SetDownHand();
    }

    private void SetDownHand()
    {
        _downHand = _downHand == _leftHand ? _rightHand : _leftHand;
    }
}
