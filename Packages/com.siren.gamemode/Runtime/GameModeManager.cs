using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMode
{
    public static class GameModeManager
    {
        private static IGameMode _currentMode;

        private static bool _isSwitching = false;

        public static void SwitchMode(IGameMode mode)
        {
            SwitchModeAsync(mode).Forget();
        }

        public static async UniTask SwitchModeAsync(IGameMode mode)
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
}