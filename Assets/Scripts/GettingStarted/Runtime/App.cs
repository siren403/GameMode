using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace GameMode.GettingStarted
{
    

    public static class App
    {
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void BeforeSplashScreen()
        {
            Debug.Log(nameof(BeforeSplashScreen));
        }

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Bootstrap()
        {
            Debug.Log(nameof(Bootstrap));

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                GameModeManager.SwitchMode(new FirstGameMode());
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
            GameModeManager.SwitchMode(new FirstGameMode());
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