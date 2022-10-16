using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameMode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using Object = UnityEngine.Object;

namespace Sandbox
{
    public static class App
    {
        // ReSharper disable once Unity.IncorrectMethodSignature
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async UniTaskVoid Bootstrap()
        {
            var settings = AppSettings.Instance;
            var startScene = SceneManager.GetActiveScene();

            if (startScene.buildIndex != settings.MasterSceneIndex)
            {
#if UNITY_EDITOR
                await SceneManager.LoadSceneAsync(settings.MasterSceneIndex, LoadSceneMode.Additive);
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

            #region Widget Provider

            foreach (var provider in settings.Widgets)
            {
                builder.Register<WidgetProviderRegister<ITitleWidget>>(Lifetime.Singleton)
                    .AsSelf()
                    .WithParameter(provider);
            }

            #endregion

            #region Elements

            foreach (var element in settings.Elements)
            {
                element.CreateRegister()?.Configuration(builder);
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