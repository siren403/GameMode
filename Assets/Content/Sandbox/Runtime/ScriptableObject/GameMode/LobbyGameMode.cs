using Cysharp.Threading.Tasks;
using GameMode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sandbox
{
    public interface ISceneDescriptor
    {
        string SceneName { get; }
    }

    public static class SceneDescriptorExtension
    {
        public static async UniTask<Scene> LoadSceneAsync(this ISceneDescriptor descriptor)
        {
            var sceneName = descriptor.SceneName;
            var scene = SceneManager.GetSceneByName(sceneName);

            if (!scene.isLoaded)
            {
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                scene = SceneManager.GetSceneByName(sceneName);
            }

            return scene;
        }

        public static async UniTask UnloadSceneAsync(this ISceneDescriptor descriptor)
            => await SceneManager.UnloadSceneAsync(descriptor.SceneName);
    }

    public class LobbySceneDescriptor : ISceneDescriptor
    {
        public string SceneName => "SandboxLobbyScene";
    }

    [CreateAssetMenu(menuName = "Game Mode/" + nameof(LobbyGameMode), fileName = "LobbyGameMode", order = 0)]
    public class LobbyGameMode : ScriptableGameMode
    {
        private readonly ISceneDescriptor _lobbyScene = new LobbySceneDescriptor();

        public override async UniTask OnStartAsync()
        {
            var lobbyScene = await _lobbyScene.LoadSceneAsync();
            SceneManager.SetActiveScene(lobbyScene);
        }

        public override async UniTask OnEndAsync()
        {
            await _lobbyScene.UnloadSceneAsync();
        }
    }
}