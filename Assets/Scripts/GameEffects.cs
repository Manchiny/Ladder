using UnityEngine;

public class GameEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem _falling;

    private void Start()
    {
        StopFallingEffect();
    }

    public void PlayFallingEffect() => _falling.Play();
    public void StopFallingEffect() => _falling.Stop();
}
