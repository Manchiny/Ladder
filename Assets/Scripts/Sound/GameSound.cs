using Agava.WebUtility;
using Assets.Scripts.Hands;
using Assets.Scripts.Ladder;
using UnityEngine;

namespace Assets.Scripts.Sound
{
    public class GameSound : MonoBehaviour
    {
        [SerializeField] private AudioSource _defaultAudioSource;
        [SerializeField] private AudioSource _backgrounsAudioSource;
        [Space]
        [SerializeField] private AudioClip _takeSound;
        [SerializeField] private AudioClip _failSound;
        [SerializeField] private AudioClip _looseSound;
        [SerializeField] private AudioClip _fallingSound;
        [Space]
        [SerializeField] private AudioClip _buttonClick;
        [SerializeField] private AudioClip _congratsSound;
        [Space]
        [SerializeField] private AudioClip _backGroundSound;

        private bool _backgroundPlaying;

        private void OnDestroy()
        {
            Game.Hands.Taked -= PlayTakedSound;
            Game.Hands.Loosed -= PlayLooseSound;
            Game.Hands.Completed -= PlayCongratsSound;
        }

        private void OnEnable()
        {
            WebApplication.InBackgroundChangeEvent += OnInBackgroundChange;
        }

        private void OnDisable()
        {
            WebApplication.InBackgroundChangeEvent -= OnInBackgroundChange;
        }

        public void Init()
        {
            SetSound(Game.User.NeedSound);

            Game.Hands.Taked += PlayTakedSound;
            Game.Hands.Loosed += PlayLooseSound;
            Game.Hands.Completed += PlayCongratsSound;

            PlayBackgroundSound();
        }

        public void SetSound(bool needActivate)
        {
            AudioListener.pause = !needActivate;
            AudioListener.volume = needActivate ? 1 : 0;
        }

        public void PlayBasicButtonClick()
        {
            PlaySound(_buttonClick, 0.75f);
        }

        public void PlayCongratsSound()
        {
            PlaySound(_congratsSound, 1);
        }

        public void PlayFailedSound(AudioSource source)
        {
            PlaySound(_failSound, 0.75f, source);

            PlaySound(_fallingSound, 1f, _backgrounsAudioSource);
            _backgroundPlaying = false;
        }

        private void PlayTakedSound(LadderStep step, Hand hand)
        {
            PlaySound(_takeSound, 0.8f, hand.AudioSource);

            PlayBackgroundSound();
        }

        private void PlayLooseSound()
        {
            var source = Game.Hands.DownHand.AudioSource;
            PlaySound(_looseSound, 1f, source);
        }

        private void PlayBackgroundSound()
        {
            if (_backgroundPlaying)
                return;

            _backgroundPlaying = true;

            PlaySound(_backGroundSound, 0.5f, _backgrounsAudioSource);
        }

        private void PlaySound(AudioClip clip, float volume, AudioSource source = null)
        {
            if(clip != null)
            {
                if (source == null)
                    source = _defaultAudioSource;

                source.volume = volume;
                source.clip = clip;
                source.Play();
            }
        }

        private void OnInBackgroundChange(bool inBackground)
        {
            if (inBackground == false)
                SetSound(Game.User.NeedSound);
            else
                SetSound(false);
        }
    }
}
