using UnityEngine;
using VContainer;

namespace Sandbox.UI.Elements
{
    [CreateAssetMenu(menuName = "Elements/" + nameof(BackgroundTextElement), fileName = "BackgroundTextElement",
        order = 0)]
    public class BackgroundTextElement : PrefabElement<IBackgroundText>
    {
        public override IElementRegister CreateRegister() => new Register(this);

        private class Register : IElementRegister
        {
            private readonly BackgroundTextElement _text;

            public Register(BackgroundTextElement text)
            {
                _text = text;
            }

            public void Configuration(IContainerBuilder builder)
            {
                builder.Register(_ => { return Instantiate(_text.Prefab).GetComponent<IBackgroundText>(); },
                    Lifetime.Transient);
            }
        }
    }

    public interface IBackgroundText : IText
    {
        Sprite Background { get; set; }
    }
}