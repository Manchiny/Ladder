using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyBoostView : MonoBehaviour
{
    [SerializeField] private Button _button;
    [Space]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private Image _icon;
    [Space]
    [SerializeField] private Image[] _coloredElements;

    public void Init(Boost boost, Action onClick)
    {
        _titleText.text = boost.Name;
        _priceText.text = $"${boost.GetNextLevelCost(Game.User)}";

        if (boost.Icon != null)
            _icon.sprite = boost.Icon;
        else
            _icon.gameObject.SetActive(false);

        foreach (var image in _coloredElements)
            image.color = boost.ViewColor;

        _button.onClick.AddListener(() => onClick?.Invoke());
    }
}
