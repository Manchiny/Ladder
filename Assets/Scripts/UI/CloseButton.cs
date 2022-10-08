using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class CloseButton : MonoBehaviour
    {
        private Button _button;
        private AbstractWindow _window;

        private void Awake()
        {
            _window = transform.GetComponentInParent<AbstractWindow>();
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(_window.Close);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(_window.Close);
        }
    }
}
