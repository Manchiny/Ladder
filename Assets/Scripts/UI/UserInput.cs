using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class UserInput : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action Touched;
        public event Action Untouched;

        private void Awake()
        {
            gameObject.SetActive(false);
        }
        public void Activate()
        {
            gameObject.SetActive(true);
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

