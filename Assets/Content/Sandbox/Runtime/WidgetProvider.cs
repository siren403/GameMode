using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Sandbox
{
    [CreateAssetMenu(menuName = "View/" + nameof(WidgetProvider), fileName = "WidgetProvider", order = 0)]
    public class WidgetProvider : ScriptableObject
    {
        [SerializeField] private GameObject prefab;

        public GameObject Prefab => prefab;

        private void OnValidate()
        {
            if (prefab != null && !prefab.TryGetComponent(out ITitleWidget widget))
            {
                prefab = null;
                throw new Exception("not found widget");
            }
        }
    }

    public class WidgetProviderRegister<T> where T : IWidget
    {
        private readonly WidgetProvider _provider;

        public WidgetProviderRegister(WidgetProvider provider)
        {
            _provider = provider;
        }

        public T Instantiate(Transform parent)
        {
            var instance = Object.Instantiate(_provider.Prefab, parent);
            return instance.GetComponent<T>();
        }
    }

    public interface IWidget
    {
    }

    public interface ITitleWidget : IWidget
    {
        Button StartButton { get; }
    }
}