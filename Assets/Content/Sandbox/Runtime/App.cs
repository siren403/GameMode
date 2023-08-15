using System.Linq;
using Cysharp.Threading.Tasks;
using GameMode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Sandbox
{
    public static class App
    {
        // ReSharper disable once Unity.IncorrectMethodSignature
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async UniTaskVoid Bootstrap()
        {
            return;
            var settings = AppSettings.Instance;
            var startScene = SceneManager.GetActiveScene();

            if (startScene.buildIndex != AppSettings.MasterSceneIndex)
            {
#if UNITY_EDITOR
                await SceneManager.LoadSceneAsync(AppSettings.MasterSceneIndex, LoadSceneMode.Additive);
#else
                //TODO: 빌드 시 MasterScene을 0번으로
                await SceneManager.LoadSceneAsync(settings.MasterSceneIndex);
#endif
            }

            var builder = new ContainerBuilder();

            #region GameMode with VContainer

            foreach (var gameMode in settings.GameModes.Concat(
                         new[]
                         {
                             settings.GlobalGameMode,
                             settings.MasterGameMode
                         }
                     ))
            {
                builder.RegisterInstance(gameMode).AsSelf();
                builder.RegisterBuildCallback(_ => { _.Inject(_.Resolve(gameMode.GetType())); });
            }

            #endregion

            var container = builder.Build();
            GameModeManager.SetProvider(container.ToGameModeProvider(settings.GlobalGameMode));
            GameModeManager.SwitchMode(settings.MasterGameMode);

            if (settings.GameModes.Any())
            {
                GameModeManager.SwitchMode(settings.GameModes.First());
            }
        }
    }

    public static class GameModeProviderExtensions
    {
        private class GameModeProvider : IGameModeProvider
        {
            private readonly IObjectResolver _resolver;

            public GameModeProvider(IObjectResolver resolver, IGameMode globalGameMode)
            {
                _resolver = resolver;
                Global = globalGameMode;
            }

            public IGameMode Global { get; }

            public T Resolve<T>() where T : IGameMode
            {
                var instance = _resolver.Resolve<T>();
                return instance;
            }
        }

        public static IGameModeProvider ToGameModeProvider(this IObjectResolver resolver, IGameMode globalGameMode)
        {
            return new GameModeProvider(resolver, globalGameMode);
        }
    }
}