using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMode
{
    public interface IGameModeProvider
    {
        IGameMode Global { get; }
        T Resolve<T>() where T : IGameMode;
    }


    public static class GameModeManager
    {
        private static readonly LinkedTask SwitchTasks = new();
        private static IGameMode _currentMode;
        private static bool _isSwitching;
        private static IGameModeProvider _provider;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize()
        {
            SwitchTasks.Clear();
            _currentMode = null;
            _isSwitching = false;
            _provider = null;
        }

        public static IGameMode CurrentMode => _currentMode;

        public static void SetProvider(IGameModeProvider provider) => _provider = provider;

        public static void SwitchMode<T>() where T : class, IGameMode
        {
            IGameMode modeInstance = _provider.Resolve<T>();
            SwitchMode(modeInstance);
        }

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

            var global = _provider?.Global;
            if (global != null)
            {
                await global.OnStartAsync();
            }

            // TODO: fade in 
            if (_currentMode != null)
            {
                // TODO: fade in 
                _currentMode.State = GameModeState.Ending;
                await _currentMode.OnEndAsync();
                _currentMode.State = GameModeState.Ended;
            }

            _currentMode = mode;
            try
            {
                // TODO: fade out
                _currentMode.State = GameModeState.Starting;
                await _currentMode.OnStartAsync();
                _currentMode.State = GameModeState.Started;
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

            if (global != null)
            {
                await global.OnEndAsync();
            }

            // TODO: fade out
            _isSwitching = false;
        }
    }

    // public static class GameModeManager
    // {
    //     private static IGameMode _currentMode;
    //
    //     private static bool _isSwitching = false;
    //
    //     public static void SwitchMode(IGameMode mode)
    //     {
    //         SwitchModeAsync(mode).Forget();
    //     }
    //
    //     public static async UniTask SwitchModeAsync(IGameMode mode)
    //     {
    //         await UniTask.NextFrame();
    //
    //         if (mode.State != GameModeState.Ended) return;
    //
    //         await UniTask.WaitUntil(() => !_isSwitching).Timeout(TimeSpan.FromSeconds(10));
    //         _isSwitching = true;
    //
    //         var masterScene = SceneManager.GetSceneByBuildIndex(0);
    //         if (masterScene.isSubScene)
    //         {
    //             SceneManager.SetActiveScene(masterScene);
    //         }
    //
    //         // TODO: fade in 
    //         if (_currentMode != null)
    //         {
    //             // TODO: fade in 
    //             _currentMode.State = GameModeState.Ending;
    //             await _currentMode.OnEndAsync();
    //         }
    //
    //         _currentMode = mode;
    //         try
    //         {
    //             // TODO: fade out
    //             await _currentMode.OnStartAsync();
    //         }
    //         catch (Exception e)
    //         {
    //             Debug.Log(e);
    //             throw;
    //         }
    //
    //         if (_currentMode.State != GameModeState.Started)
    //         {
    //             throw new Exception($"require started game mode: {_currentMode.GetType().Name}");
    //         }
    //
    //         // TODO: fade out
    //         _isSwitching = false;
    //     }
    // }
}