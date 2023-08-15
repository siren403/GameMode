using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameMode.App;

namespace GameMode.Editor
{
    public static class EditorApp
    {
        private static readonly object Owner = new object();

        private static IEnumerator LoadSceneAsync(string sceneName)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return operation;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void LoadRestoreScene()
        {
            var activeScene = SceneManager.GetActiveScene().name;
            if (activeScene.Equals(RestoreScene))
            {
                RestoreScene = null;
                return;
            }

            if (string.IsNullOrWhiteSpace(StartingGameMode) && !string.IsNullOrWhiteSpace(RestoreScene))
            {
                EditorCoroutineUtility.StartCoroutine(LoadSceneAsync(RestoreScene), Owner);
            }
        }
    }
}