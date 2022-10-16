using System;
using TMPro;
using UnityEngine;

namespace Sandbox.UI.Elements
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(RectTransform))]
    public class TMProText : MonoBehaviour, IText
    {
        public string Value
        {
            get => text.text;
            set => text.text = value;
        }

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private RectTransform rectTransform;

        private void OnValidate()
        {
            if (text == null)
            {
                text = GetComponent<TextMeshProUGUI>();
            }

            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
        }

        public void SetParent(Transform parent)
        {
            rectTransform.SetParent(parent);
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public Vector2 Position
        {
            get => rectTransform.anchoredPosition;
            set => rectTransform.anchoredPosition = value;
        }
    }
}