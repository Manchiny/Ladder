using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class FloatingMoneyText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _moneyText;

        private const float AnimationDuration = 1f;
        private const float AnimationDeltaY = 200f;

        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void Init(int moneyCount)
        {
            _moneyText.text = $"${moneyCount}";
            PlayAnimation();
        }

        private void PlayAnimation()
        {
            float currentY = _rect.transform.position.y;
            _rect.DOMoveY(currentY + AnimationDeltaY, AnimationDuration).SetEase(Ease.Linear).SetLink(gameObject);
            _moneyText.DOFade(0, AnimationDuration).SetEase(Ease.Linear).SetLink(gameObject).OnComplete(() => Destroy(gameObject));
        }
    }
}
