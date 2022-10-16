using Cysharp.Threading.Tasks;
using GameMode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sandbox
{
    [CreateAssetMenu(menuName = "Game Mode/" + nameof(LoginGameMode), fileName = "LoginGameMode", order = 0)]
    public class LoginGameMode : ScriptableGameMode
    {
        public override async UniTask OnStartAsync()
        {
            Debug.Log(nameof(OnStartAsync));
            var titleScene = SceneManager.GetSceneByName("SandboxTitleScene");

            if (!titleScene.isLoaded)
            {
                await SceneManager.LoadSceneAsync("SandboxTitleScene", LoadSceneMode.Additive);
                titleScene = SceneManager.GetSceneByName("SandboxTitleScene");
            }

            SceneManager.SetActiveScene(titleScene);
        }

        public override async UniTask OnEndAsync()
        {
            await SceneManager.UnloadSceneAsync("SandboxTitleScene");
        }
    }
}