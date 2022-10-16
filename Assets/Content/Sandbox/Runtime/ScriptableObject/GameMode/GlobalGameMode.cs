using Cysharp.Threading.Tasks;
using GameMode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sandbox
{
    [CreateAssetMenu(menuName = "Game Mode/" + nameof(GlobalGameMode), fileName = "GlobalGameMode", order = 0)]
    public class GlobalGameMode : ScriptableGameMode
    {
        public override UniTask OnStartAsync()
        {
            var masterSceneIndex = AppSettings.Instance.MasterSceneIndex;

            var activeScene = SceneManager.GetActiveScene();

            if (activeScene.buildIndex != masterSceneIndex)
            {
                var masterScene = SceneManager.GetSceneByBuildIndex(masterSceneIndex);
                SceneManager.SetActiveScene(masterScene);
            }

            return UniTask.CompletedTask;
        }

        public override UniTask OnEndAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}