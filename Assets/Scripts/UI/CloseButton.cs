using UnityEngine;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(BasicButton))]
    public class CloseButton : MonoBehaviour
    {
        private BasicButton _button;
        private AbstractWindow _window;

        private void Awake()
        {
            _window = transform.GetComponentInParent<AbstractWindow>();
            _button = GetComponent<BasicButton>();
        }

        private void Start()
        {
            _button.SetOnClick(OnClick);
        }

        private void OnEnable()
        {
            _button.SetOnClick(OnClick);
        }

        private void OnDisable()
        {
            _button.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            Debug.Log("Close clicked");
            _window.Close();
        }
    }
}
