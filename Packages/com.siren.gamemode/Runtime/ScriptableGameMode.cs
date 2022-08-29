using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameMode
{
    public class ScriptableGameMode : ScriptableObject, IGameMode
    {
        public virtual GameModeState State { get; protected set; } = GameModeState.Ended;

        public virtual UniTask OnStartAsync()
        {
            State = GameModeState.Started;
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnEndAsync()
        {
            State = GameModeState.Ended;
            return UniTask.CompletedTask;
        }
    }

    public class ScriptableGameMode<T> : ScriptableGameMode where T : class, IGameMode, new()
    {
        protected T Mode { get; private set; } = new T();

        public override GameModeState State => Mode.State;

        public override UniTask OnStartAsync() => Mode.OnStartAsync();
        public override UniTask OnEndAsync() => Mode.OnEndAsync();
    }
}