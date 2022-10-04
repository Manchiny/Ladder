using UnityEngine;

public class FlagRandomPhaseAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private const string FlagAnimation = "Flag";

    private void Start()
    {
        _animator.Play(FlagAnimation, 0, Random.Range(0.0f, 1.0f));
    }
}
