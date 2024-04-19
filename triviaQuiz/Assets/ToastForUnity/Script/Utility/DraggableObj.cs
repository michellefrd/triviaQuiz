using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToastForUnity.Script.Utility
{
    public class DraggableObj : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Transform _dropParent;
        public Guid Id { get; set; }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            transform.gameObject.TryGetComponent(out CanvasGroup canvasGroup);
            _canvasGroup = canvasGroup == null ? transform.gameObject.AddComponent<CanvasGroup>() : canvasGroup;
            _dropParent = transform.parent;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = .6f;
            _canvasGroup.blocksRaycasts = false;
            transform.SetParent(_canvas.transform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            transform.SetParent(_dropParent);
        }
    }
}