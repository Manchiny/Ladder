using Agava.WebUtility;
using Assets.Scripts.Hands;
using Assets.Scripts.Ladder;
using System;
using UnityEngine;

namespace Assets.Scripts.Sound
{
    public class GameSound : MonoBehaviour
    {
        [SerializeField] private AudioSource _defaultAudioSource;
        [Space]
        [SerializeField] private AudioClip _takeSound;
        [SerializeField] private AudioClip _failSound;
        [Space]
        [SerializeField] private AudioClip _buttonClick;

        private void OnDestroy()
        {
            Game.Hands.Taked -= PlayTakedSound;
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
            Game.Hands.Taked += PlayTakedSound;
            SetSound(Game.User.NeedSound);
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

        public void PlayFailedSound(AudioSource source)
        {
            PlaySound(_failSound, 0.75f, source);
        }

        private void PlayTakedSound(LadderStep step, Hand hand)
        {
            PlaySound(_takeSound, 1, hand.AudioSource);
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
