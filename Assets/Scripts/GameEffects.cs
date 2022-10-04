using UnityEngine;


namespace Assets.Scripts
{
    public class GameEffects : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _falling;
        [SerializeField] private ParticleSystem _konfetti;

        private void Start()
        {
            StopFallingEffect();
        }

        public void PlayFallingEffect() => _falling.Play();
        public void StopFallingEffect() => _falling.Stop();
        public void PlayKonfettiEffect() => _konfetti.Play();
    }
}
