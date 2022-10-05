using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Ladder
{
    public class Spike : MonoBehaviour
    {
        private const float MinScale = 0.45f;

        private float _realMinScale;
        private float _defaultLocalScaleY;

        private void Awake()
        {
            var scale = transform.localScale;
            _defaultLocalScaleY = scale.y;
            _realMinScale = MinScale * _defaultLocalScaleY;

            scale.y = _realMinScale;
        }

        public Tween PlayShowAnimation(float duration)
        {
            if(gameObject !=null)
                return transform.DOScaleY(_defaultLocalScaleY, duration).SetEase(Ease.Linear).SetLink(gameObject);

            return null;
        }

        public Tween PlayHideAnimation(float duration)
        {
            if (gameObject != null)
                return transform.DOScaleY(_realMinScale, duration).SetEase(Ease.Linear).SetLink(gameObject);

            return null;
        }
    }
}