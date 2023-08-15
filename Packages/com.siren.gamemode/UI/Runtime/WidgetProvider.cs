using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameMode.UI
{
    [CreateAssetMenu(menuName = "View/" + nameof(WidgetProvider), fileName = "WidgetProvider", order = 0)]
    public class WidgetProvider : ScriptableObject
    {
        [SerializeField] protected GameObject prefab;

        public GameObject Prefab => prefab;
    }

    public abstract class WidgetProvider<T> : WidgetProvider where T : IWidget
    {
        private void OnValidate()
        {
            if (prefab != null && !prefab.TryGetComponent(out T widget))
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

   
}