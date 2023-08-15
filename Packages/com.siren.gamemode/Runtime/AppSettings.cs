using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameMode
{
    public class AppSettings : ScriptableObject
    {
        public const int MasterSceneIndex = 0;
#if UNITY_EDITOR
        public const string AppSettingsPath = "Assets/Settings/" + nameof(AppSettings) + ".asset";

        public static AppSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AppSettings>(AppSettingsPath);
            if (settings == null)
            {
                settings = CreateInstance<AppSettings>();
                AssetDatabase.CreateAsset(settings, AppSettingsPath);
                AssetDatabase.SaveAssets();


                var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
                preloadedAssets.RemoveAll(x => x is AppSettings);
                preloadedAssets.Add(settings);
                PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            }

            Instance = settings;

            return settings;
        }

        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        public static void LoadInstanceFromPreloadAssets()
        {
            var preloadAsset = UnityEditor.PlayerSettings.GetPreloadedAssets().FirstOrDefault(x => x is AppSettings);
            if (preloadAsset is AppSettings instance)
            {
                instance.OnEnable();
            }
        }

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        [InitializeOnLoadMethod()]
        static void RuntimeInitialize()
        {
            // For editor, we need to load the Preload asset manually.
            LoadInstanceFromPreloadAssets();
        }
#endif
        void OnEnable()
        {
            Instance = this;
        }

        public static AppSettings Instance { get; private set; }

        [SerializeField] private bool skipInitialize = false;
        public bool SkipInitialize => skipInitialize;


#if UNITY_EDITOR
        [SerializeField] private SceneAsset masterScene;
        public SceneAsset MasterScene => masterScene;
#endif
        [SerializeField] private ScriptableGameMode globalGameMode;
        [SerializeField] private ScriptableGameMode masterGameMode;

        [SerializeField] private List<ScriptableGameMode> gameModes;

        public ScriptableGameMode GlobalGameMode => globalGameMode;
        public ScriptableGameMode MasterGameMode => masterGameMode;
        public List<ScriptableGameMode> GameModes => gameModes;
    }
}