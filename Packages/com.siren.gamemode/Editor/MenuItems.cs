using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMode.Editor
{
    public static class GameObjectMenus
    {

        #region GameObject

        [MenuItem("GameObject/GameMode/Placeholder (EditorOnly)")]
        public static void CreateEditorOnlyPlaceholder()
        {
            var placeholder = Object.FindObjectOfType<GameModePlaceholder>();
            if (placeholder != null)
            {
                EditorGUIUtility.PingObject(placeholder);
            }
            else
            {
                placeholder = new GameObject("GameMode (EditorOnly)").AddComponent<GameModePlaceholder>();
                placeholder.tag = "EditorOnly";
                placeholder.transform.SetSiblingIndex(0);
            }
        }

        #endregion
    }
}