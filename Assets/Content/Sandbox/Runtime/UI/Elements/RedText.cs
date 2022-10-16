using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace Sandbox.UI.Elements
{
    [CreateAssetMenu(menuName = "Elements/" + nameof(RedText), fileName = "RedText", order = 0)]
    public class RedText : AddressableElement<IRedText>
    {
        public override IElementRegister CreateRegister() => new Register(this);

        public async UniTask<IRedText> InstantiateAsync(Transform parent)
        {
            var prefab = await ElementReference.LoadAssetAsync().Task.AsUniTask();
            var instance = Instantiate(prefab, parent);
            ElementReference.ReleaseAsset();
            return instance.GetComponent<IRedText>();
        }

        private class Register : IElementRegister
        {
            private readonly RedText _text;

            public Register(RedText text)
            {
                _text = text;
            }

            public void Configuration(IContainerBuilder builder)
            {
                // builder.Register(_ => { return Instantiate(_text.Prefab).GetComponent<IRedText>(); },
                // Lifetime.Transient);
                builder.RegisterInstance(_text);
            }
        }
    }

    public interface IRedText : IText
    {
    }
}