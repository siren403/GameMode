using System;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Sandbox.UI.Elements
{
    [CreateAssetMenu(menuName = "Elements/" + nameof(Text), fileName = "Text", order = 0)]
    public class Text : PrefabElement<IText>
    {
        public override IElementRegister CreateRegister() => new Register(this);

        private class Register : IElementRegister
        {
            private readonly Text _text;

            public Register(Text text)
            {
                _text = text;
            }

            public void Configuration(IContainerBuilder builder)
            {
                builder.Register(_ =>
                {
                    return Instantiate(_text.Prefab).GetComponent<IText>();
                }, Lifetime.Transient);
            }
        }
    }

    public interface IText : IElement
    {
        string Value { get; set; }
    }
}