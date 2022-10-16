using System.Collections.Generic;
using System.Linq;
using GameMode;
using Sandbox.UI.Elements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer.Unity;

namespace Sandbox
{
    public class AppSettings : ScriptableObject
    {
#if UNITY_EDITOR
        public const string AppSettingsPath = "Assets/Settings/" + nameof(AppSettings) + ".asset";

        internal static AppSettings GetOrCreateSettings()
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

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
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

        [SerializeField] private int masterSceneIndex = 0;
        [SerializeField] private ScriptableGameMode globalGameMode;
        [SerializeField] private ScriptableGameMode masterGameMode;
        [SerializeField] private List<ScriptableGameMode> gameModes;
        [SerializeField] private List<WidgetProvider> widgets;
        [SerializeField] private List<ScriptableElement> elements;
        public int MasterSceneIndex => masterSceneIndex;
        public ScriptableGameMode GlobalGameMode => globalGameMode;
        public ScriptableGameMode MasterGameMode => masterGameMode;
        public List<ScriptableGameMode> GameModes => gameModes;

        public List<WidgetProvider> Widgets => widgets;

        public List<ScriptableElement> Elements => elements;
    }

    // Register a SettingsProvider using UIElements for the drawing framework:
    static class AppSettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateAppSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
            var provider = new SettingsProvider("Project/AppSettings", SettingsScope.Project)
            {
                label = "AppSettings",
                // activateHandler is called when the user clicks on the Settings item in the Settings window.
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = AppSettings.GetSerializedSettings();

                    // rootElement is a VisualElement. If you add any children to it, the OnGUI function
                    // isn't called because the SettingsProvider uses the UIElements drawing framework.
                    // var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/settings_ui.uss");
                    // rootElement.styleSheets.Add(styleSheet);
                    var title = new Label()
                    {
                        text = "Custom UI Elements"
                    };
                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new VisualElement()
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Column
                        }
                    };
                    properties.AddToClassList("property-list");
                    rootElement.Add(properties);

                    properties.Add(new PropertyField(settings.FindProperty("level")));

                    rootElement.Bind(settings);
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] {"Level"})
            };

            return provider;
        }
    }
}