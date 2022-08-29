using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMode
{
    public static class GameModeExtensions
    {
        public static async UniTask<Scene> LoadSceneAsync(this IGameMode gameMode,
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Additive,
            bool isActive = true)
        {
            await SceneManager.LoadSceneAsync(sceneName, mode);
            var scene = SceneManager.GetSceneByName(sceneName);
            if (isActive)
            {
                SceneManager.SetActiveScene(scene);
            }

            return scene;
        }

        public static async UniTask<Scene> LoadSceneAsync<T>(this ISceneContextDescriptor<T> descriptor,
            bool isActive = true) where T : class, ISceneContext
        {
            await SceneManager.LoadSceneAsync(descriptor.Name, descriptor.LoadMode);
            var scene = SceneManager.GetSceneByName(descriptor.Name);
            if (isActive)
            {
                SceneManager.SetActiveScene(scene);
            }

            return scene;
        }

        public static UniTask UnloadSceneAsync<T>(this ISceneContextDescriptor<T> descriptor) where T : class, ISceneContext
        {
            descriptor.Dispose();
            return SceneManager.UnloadSceneAsync(descriptor.Name).ToUniTask();
        }
    }

    public abstract class GameModeBase : IGameMode
    {
        public GameModeState State { get; private set; } = GameModeState.Ended;

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

        protected void BeginState()
        {
            if (State is GameModeState.Starting or GameModeState.Ending)
            {
                throw new Exception($"already begin state: {State}");
            }

            State = State == GameModeState.Ended ? GameModeState.Starting : GameModeState.Ending;
        }

        protected void EndState()
        {
            if (State is GameModeState.Started or GameModeState.Ended)
            {
                throw new Exception($"already end state: {State}");
            }

            State = State == GameModeState.Starting ? GameModeState.Started : GameModeState.Ended;
        }

        protected IDisposable StateScope()
        {
            return new StateScopeHandler(this);
        }

        private readonly struct StateScopeHandler : IDisposable
        {
            private readonly GameModeBase _mode;

            public StateScopeHandler(GameModeBase mode)
            {
                _mode = mode;
                _mode.BeginState();
            }

            public void Dispose()
            {
                _mode.EndState();
            }
        }
    }
}