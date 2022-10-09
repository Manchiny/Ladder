using Assets.Scripts.Ladder;
using DG.Tweening;
using RSG;
using System;
using UnityEngine;
using static Assets.Scripts.Ladder.Ladder;

namespace Assets.Scripts.Hands
{
    [RequireComponent(typeof(AudioSource))]
    public class Hand : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _fogEffect;

        private const float MoveUpDuration = 0.5f;
        private const float ForceMoveStepDuration = 0.2f;

        private Stamina _stamina;

        private bool _isProcess;
        private Tween _fallingTween;

        private HandsAnimations _animations;
        private Vector3 _defaultPosition;

        public event Action<LadderStep, Hand> Taked;
        public event Action<Hand> Failed;
        public event Action Loosed;

        public LadderSide Side { get; private set; }
        public bool IsFalling { get; private set; }
        public LadderStep LastTakedStep { get; private set; }
        public AudioSource AudioSource { get; private set; }

        public bool CanMove => _isProcess == false && IsFalling == false;
        public float GetHeight => transform.position.y;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();   
        }

        private void Start()
        {
            _defaultPosition = transform.localPosition;
            _fogEffect.Stop();
        }

        public void Init(LadderStep initStep, LadderSide side, Stamina stamina)
        {
            if (side == LadderSide.Right)
                _defaultPosition.x = GameConstants.HandDafaultXPosition;
            else
                _defaultPosition.x = -GameConstants.HandDafaultXPosition;

            _stamina = stamina;
            transform.localPosition = _defaultPosition;

            Side = side;

            _animations = new HandsAnimations(_animator);
            ForceTake(initStep);

            IsFalling = false;
        }

        public IPromise PlayFailAnimation() => _animations.PlayFail(transform);

        public void FallDown(float duration)
        {
            IsFalling = true;
            _isProcess = false;

            if (LastTakedStep != null)
                LastTakedStep.Hand = null;

            _animations.PlayRelease();

            _fallingTween = transform.DOMoveY(0, duration)
                             .SetLink(gameObject)
                             .SetEase(Ease.Linear)
                             .OnComplete(() => Loose());
        }

        public bool TryMove(LadderStep step)
        {
            if (CanMove == false || step == null)
                return false;

            if (step is FinishButtonStep)
                PressFinishButton(step);
            else
                MoveUpStep(step);

            return true;
        }

        public bool ForceTake(LadderStep step)
        {
            if (_isProcess)
                return false;

            _isProcess = true;
            IsFalling = false;

            _animations.PlayClasp();

            if (_fallingTween != null)
                _fallingTween.Kill();

            transform.DOMoveY(step.Height, ForceMoveStepDuration)
                 .SetLink(gameObject)
                 .SetEase(Ease.Linear)
                 .OnComplete(() => Take(step));

            return true;
        }

        private void Loose()
        {
            IsFalling = false;
            LastTakedStep.Hand = null;

            Loosed?.Invoke();
        }

        private void MoveUpStep(LadderStep step)
        {
            _isProcess = true;
            _animations.PlayRelease();

            if (LastTakedStep != null)
                LastTakedStep.Hand = null;

            transform.DOMoveY(step.Height, MoveUpDuration)
                 .SetLink(gameObject)
                 .SetEase(Ease.Linear)
                 .OnComplete(() => TryTake(step));
        }

        private bool TryTake(LadderStep step)
        {
            _animations.PlayClasp();

            if (step.CanBeTaked(Side))
            {
                if (_stamina.IsLowEnergy(out float factor) == false || factor < Stamina.MaxFactorToFail)
                {
                    if (IsFalling == false)
                        _stamina.ForceExpendEnergyForMove();

                    Take(step);
                    return true;
                }

                FailTake();
                return false;

            }
            else
            {
                FailTake();
                return false;
            }
        }

        private void FailTake()
        {
            _isProcess = false;
            IsFalling = true;

            Game.Sound.PlayFailedSound(AudioSource);
            Failed?.Invoke(this);
        }

        private void PressFinishButton(LadderStep step)
        {
            _isProcess = true;

            if (LastTakedStep != null)
                LastTakedStep.Hand = null;

            _animations.PlayRelease();

            Vector3 position = step.transform.position;
            position.z = -0.05f;

            Utils.WaitSeconds(0.2f)
                .Then(() =>
                {
                    _animations.PlayPush();

                    transform.DOMove(position, MoveUpDuration)
                             .SetLink(gameObject)
                             .SetEase(Ease.Linear)
                             .OnComplete(() => Take(step));
                });
        }

        private void Take(LadderStep step)
        {
            if (LastTakedStep != null)
                LastTakedStep.Hand = null;

            LastTakedStep = step;

            IsFalling = false;
            _isProcess = false;

            _fogEffect.Play();

            Taked?.Invoke(step, this);
        }
    }
}
