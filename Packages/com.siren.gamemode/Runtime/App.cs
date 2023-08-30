//#define DOMAIN_RELOAD_HANDLING

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
    public static partial class App
    {
        private static IGameMode _startMode;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize()
        {
            _startMode = null;
            Debug.Log("app static init");
        }

        // AfterAssembliesLoaded is called before BeforeSceneLoad
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void InitUniTaskLoop()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopHelper.Initialize(ref loop);
            Debug.Log(nameof(InitUniTaskLoop));
        }


        // ReSharper disable once Unity.IncorrectMethodSignature
        // BeforeSceneLoad에서는 UniTask의 awaiter가 처리 되지않아 Delay, WaitUntil등 오퍼레이터가 동작하지 않음.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Bootstrap()
        {
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

#if UNITY_EDITOR
            if (!string.IsNullOrWhiteSpace(StartingGameMode))
            {
                _startMode = settings.GameModes.FirstOrDefault(_ => _.name == StartingGameMode);
                StartingGameMode = null;
            }
#endif
            if (_startMode == null && startingMaster)
            {
                _startMode = settings.GameModes.FirstOrDefault();
            }

            Debug.Log($"App Initialized: {SceneManager.GetActiveScene().name}");
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void SceneLoaded()
        {
            if (_startMode != null)
                GameModeManager.SwitchMode(_startMode);

            Debug.Log("switch start mode");
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

#if DOMAIN_RELOAD_HANDLING
            // Warn: 도메인 리로드 비활성화 상태에서 EnterPlaymode로 진입 시 
            // BeforeSceneLoad 시점에서 UniTask 오퍼레이터가 동작하지 않음.
            // 임의로 도메인리로드를 플레이모드 진입 전에 활성화하던가 AfterSceneLoad에서부터
            // 오퍼레이터를 사용하면 된다.
            if (EditorSettings.enterPlayModeOptionsEnabled)
            {
                EditorSettings.enterPlayModeOptionsEnabled = false;
                EditorPrefs.SetBool(nameof(EditorSettings.enterPlayModeOptions), true);
            }
#endif
            EditorSceneManager.playModeStartScene = settings.MasterScene;
            EditorApplication.EnterPlaymode();
        }

        [RuntimeInitializeOnLoadMethod]
        static void ResetPlayModeStartScene()
        {
#if DOMAIN_RELOAD_HANDLING
            if (EditorPrefs.GetBool(nameof(EditorSettings.enterPlayModeOptions)))
            {
                EditorSettings.enterPlayModeOptionsEnabled = true;
                EditorPrefs.DeleteKey(nameof(EditorSettings.enterPlayModeOptions));
            }
#endif
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