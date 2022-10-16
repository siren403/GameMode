using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameMode
{
    public abstract class ScriptableGameMode : ScriptableObject, IGameMode
    {
        GameModeState IGameMode.State { get; set; } = GameModeState.Ended;

        public abstract UniTask OnStartAsync();

        public abstract UniTask OnEndAsync();
    }

    public class ScriptableGameMode<T> : ScriptableGameMode where T : class, IGameMode, new()
    {
        protected T Mode { get; private set; } = new T();

        internal GameModeState State => Mode.State;

        public override UniTask OnStartAsync() => Mode.OnStartAsync();
        public override UniTask OnEndAsync() => Mode.OnEndAsync();
    }
    
}