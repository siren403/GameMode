using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sandbox.UI.Elements
{
    public class BackgroundText : MonoBehaviour, IBackgroundText
    {
        public string Value
        {
            get => text.text;
            set => text.text = value;
        }

        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private RectTransform rectTransform;

        private void OnValidate()
        {
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

        public Sprite Background
        {
            get => backgroundImage.sprite;
            set => backgroundImage.sprite = value;
        }
    }
}