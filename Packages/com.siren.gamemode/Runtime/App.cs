using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
#if VCONTAINER
using VContainer;
using VContainer.Unity;
#endif


namespace GameMode
{
    public class FromMaster
    {
        public string Name => nameof(FromMaster);
    }

    public static partial class App
    {
#if UNITASK
        // AfterAssembliesLoaded is called before BeforeSceneLoad
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void InitUniTaskLoop()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            Cysharp.Threading.Tasks.PlayerLoopHelper.Initialize(ref loop);
            Debug.Log(nameof(InitUniTaskLoop));
        }
#endif
        // ReSharper disable once Unity.IncorrectMethodSignature
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Bootstrap()
        {
            // EditorSceneManager.playModeStartScene = null;
            // await UniTask.NextFrame();
            // Debug.Log("success");
            // return;
            var settings = AppSettings.Instance;
            if (settings.SkipInitialize)
            {
                return;
            }

            var startScene = SceneManager.GetActiveScene();
            var startingMaster = startScene.buildIndex == AppSettings.MasterSceneIndex;

            foreach (IGameMode gameMode in settings.GameModes)
            {
                gameMode.State = GameModeState.Ended;
            }

#if VCONTAINER
            using (LifetimeScope.Enqueue(new GameModeInstaller(settings)))
            using (LifetimeScope.Enqueue(_ => { _.Register<FromMaster>(Lifetime.Singleton).AsSelf(); }))
            {
                if (SceneManager.GetActiveScene().buildIndex == AppSettings.MasterSceneIndex)
                {
                    var scope = new GameObject("MasterScope").AddComponent<LifetimeScope>();
                }
            }
#endif

            IGameMode startMode = null;
#if UNITY_EDITOR
            if (!string.IsNullOrWhiteSpace(StartingGameMode))
            {
                startMode = settings.GameModes.FirstOrDefault(_ => _.name == StartingGameMode);
                StartingGameMode = null;
            }
#endif
            if (startMode == null && startingMaster)
            {
                startMode = settings.GameModes.FirstOrDefault();
            }

            if (startMode != null)
                GameModeManager.SwitchMode(startMode);

            Debug.Log($"App Initialized: {SceneManager.GetActiveScene().name}");
        }

#if UNITY_EDITOR


        public static string StartingGameMode
        {
            get => EditorPrefs.GetString(StartingGameModeID);
            private set => EditorPrefs.SetString(StartingGameModeID, value);
        }

        public static string RestoreScene
        {
            get => EditorPrefs.GetString(RestoreSceneID);
            set => EditorPrefs.SetString(RestoreSceneID, value);
        }

        private const string RestoreSceneID = "RestoreScene";
        private const string StartingGameModeID = "StartingGameMode";

        // private const string StartFromMasterID = "StartFromMaster";

        public static void StartGameMode(string name)
        {
            if (Application.isPlaying) return;
            var settings = AppSettings.Instance;
            var containsGameMode = settings.GameModes.Select(_ => _.name).Contains(name);
            // throw new Exception($"not found game mode: {name}");
            RestoreScene = containsGameMode ? null : SceneManager.GetActiveScene().name;
            StartingGameMode = name;

            if (EditorSettings.enterPlayModeOptionsEnabled)
            {
                EditorSettings.enterPlayModeOptionsEnabled = false;
                EditorPrefs.SetBool(nameof(EditorSettings.enterPlayModeOptions), true);
            }

            EditorSceneManager.playModeStartScene = settings.MasterScene;
            EditorApplication.EnterPlaymode();
        }

        [RuntimeInitializeOnLoadMethod]
        static void ResetPlayModeStartScene()
        {
            if (EditorPrefs.GetBool(nameof(EditorSettings.enterPlayModeOptions)))
            {
                EditorSettings.enterPlayModeOptionsEnabled = true;
                EditorPrefs.DeleteKey(nameof(EditorSettings.enterPlayModeOptions));
            }


            EditorSceneManager.playModeStartScene = null;
        }

        public static void StartGameModeFirst()
        {
            var modes = AppSettings.Instance.GameModes;
            StartGameMode(modes.FirstOrDefault()?.name);
        }
#endif
    }


#if VCONTAINER
    public class GameModeInstaller : IInstaller
    {
        private readonly AppSettings _settings;

        public GameModeInstaller(AppSettings settings)
        {
            _settings = settings;
        }

        public void Install(IContainerBuilder builder)
        {
            foreach (var gameMode in _settings.GameModes)
            {
                builder.RegisterInstance(gameMode).AsSelf();
                builder.RegisterBuildCallback(_ => { _.Inject(_.Resolve(gameMode.GetType())); });
            }

            builder.RegisterEntryPoint<GameModeTicker>();
            builder.RegisterBuildCallback(_ => { GameModeManager.SetProvider(_.ToGameModeProvider(null)); });
        }
    }

    public class GameModeTicker : ITickable
    {
        public void Tick()
        {
            var currentMode = GameModeManager.CurrentMode;
            if (currentMode is not ({State: GameModeState.Started} and ITickable tickable)) return;
            tickable.Tick();
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
#endif
}