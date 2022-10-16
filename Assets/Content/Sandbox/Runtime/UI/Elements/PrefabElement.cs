using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace Sandbox.UI.Elements
{
    public abstract partial class PrefabElement<TElement> : ScriptableElement where TElement : IElement
    {
        [SerializeField] private GameObject prefab;

        protected GameObject Prefab => prefab;

        private void OnValidate()
        {
            if (prefab != null && !prefab.TryGetComponent(out TElement component))
            {
                prefab = null;
                throw new Exception(name);
            }
        }
    }

    public abstract class AddressableElement<TElement> : ScriptableElement where TElement : IElement
    {
        [SerializeField] private AssetReferenceT<GameObject> element;

        protected AssetReferenceT<GameObject> ElementReference => element;

        // [SerializeField] private GameObject prefab;
        //
        // protected GameObject Prefab => prefab;
        //
        // private void OnValidate()
        // {
        //     if (prefab != null && !prefab.TryGetComponent(out TElement component))
        //     {
        //         prefab = null;
        //         throw new Exception(name);
        //     }
        // }
    }
}