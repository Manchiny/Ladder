using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _moneyText;

    private const float FadeDuration = 1f;

    private CanvasGroup _canvas;

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
    }

    public void Init(int levelId)
    {
        _canvas.alpha = 0;

        _levelText.text = $"LEVEL {levelId}";
        Show();
    }

    public void Show()
    {
        _canvas.DOFade(1f, FadeDuration);
    }

    public void Hide()
    {
        _canvas.DOFade(0f, FadeDuration);
    }
}
