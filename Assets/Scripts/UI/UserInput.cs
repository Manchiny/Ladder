using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class UserInput : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action Touched;
        public event Action Untouched;

        private bool _isActive;
        public bool IsActive => _isActive && gameObject.activeInHierarchy;

        private void Awake()
        {
            SetActive(false);
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
        }

        public void OnPointerClick(PointerEventData eventData) { }

        public void OnPointerDown(PointerEventData eventData)
        {
            Touched?.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Untouched?.Invoke();
        }
    }
}

