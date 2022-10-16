using UnityEngine;
using VContainer;

namespace Sandbox.UI.Elements
{
    public abstract class ScriptableElement : ScriptableObject
    {
        public virtual IElementRegister CreateRegister()
        {
            return null;
        }
    }

    public interface IElementRegister
    {
        void Configuration(IContainerBuilder builder);
    }

    public interface IElement
    {
        void SetParent(Transform parent);
        Vector2 Position { get; set; }
    }
}