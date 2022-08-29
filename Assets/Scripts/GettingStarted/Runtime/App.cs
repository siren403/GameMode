using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace GameMode.GettingStarted
{
    public static class GameModeSwitcher
    {
        private static readonly LinkedTask SwitchTasks = new LinkedTask();
        private static IGameMode _currentMode;
        private static bool _isSwitching = false;

        public static void SwitchMode(IGameMode gameMode)
        {
            var isAny = SwitchTasks.Any();
            SwitchTasks.Append(UniTask.Lazy(() => InternalSwitchMode(gameMode)));
            if (!isAny)
            {
                SwitchTasks.GetAwaiter();
            }
        }

        private static async UniTask InternalSwitchMode(IGameMode mode)
        {
            await UniTask.NextFrame();

            if (mode.State != GameModeState.Ended) return;

            await UniTask.WaitUntil(() => !_isSwitching).Timeout(TimeSpan.FromSeconds(10));
            _isSwitching = true;

            var masterScene = SceneManager.GetSceneByBuildIndex(0);
            if (masterScene.isSubScene)
            {
                SceneManager.SetActiveScene(masterScene);
            }

            // TODO: fade in 
            if (_currentMode != null)
            {
                // TODO: fade in 
                await _currentMode.OnEndAsync();
            }

            _currentMode = mode;
            try
            {
                // TODO: fade out
                await _currentMode.OnStartAsync();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

            if (_currentMode.State != GameModeState.Started)
            {
                throw new Exception($"require started game mode: {_currentMode.GetType().Name}");
            }

            // TODO: fade out
            _isSwitching = false;
        }
    }

    public static class App
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void BeforeSplashScreen()
        {
            Debug.Log(nameof(BeforeSplashScreen));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Bootstrap()
        {
            Debug.Log(nameof(Bootstrap));

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                GameModeSwitcher.SwitchMode(new FirstGameMode());
            }
            else
            {
                FromMaster().Forget();
            }
            // Init();
            // InitAsync().Forget();
            // ManualBuildAsync().Forget();
        }

        private static async UniTaskVoid FromMaster()
        {
            await SceneManager.LoadSceneAsync("MasterScene");
            GameModeSwitcher.SwitchMode(new FirstGameMode());
        }

        private static async UniTask ManualBuildAsync()
        {
            Debug.Log("wait...");
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            var scope = LifetimeScope.Find<SampleSceneScope>();
            Debug.Log("manual build");
            scope.Build();
        }

        private static void Init()
        {
            Debug.Log($"in: {nameof(Init)}");
            SceneManager.LoadScene("SampleScene 1");
            Debug.Log($"loaded: {nameof(Init)}");
        }

        private static async UniTask InitAsync()
        {
            Debug.Log($"in: {nameof(InitAsync)}");
            await SceneManager.LoadSceneAsync("SampleScene 1");
            Debug.Log($"loaded: {nameof(InitAsync)}");
        }
    }
}