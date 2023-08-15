using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameMode.Editor.Overlays
{
    // [Icon("Assets/unity.png")]
    [Overlay(typeof(SceneView), "GameMode")]
    public class GameModeToolbar : ToolbarOverlay
    {
        GameModeToolbar() : base(
            ModesDropdown.ID,
            StartFromMasterButton.ID
        )
        {
        }

        protected override Layout supportedLayouts => Layout.HorizontalToolbar | Layout.VerticalToolbar;

        public override void OnCreated()
        {
        }

        [EditorToolbarElement(ID, typeof(SceneView))]
        class StartFromMasterButton : EditorToolbarButton
        {
            public const string ID = nameof(GameModeToolbar) + "/" + nameof(StartFromMasterButton);

            public StartFromMasterButton()
            {
                clicked += OnClick;
                var image = EditorGUIUtility.IconContent("d_PlayButton").image;
                Add(new Image()
                {
                    image = image
                });
            }

            private void OnClick()
            {
                App.StartGameModeFirst();
            }
        }

        [EditorToolbarElement(ID, typeof(SceneView))]
        class ModesDropdown : EditorToolbarDropdownToggle, IAccessContainerWindow
        {
            public const string ID = nameof(GameModeToolbar) + "/" + nameof(ModesDropdown);
            public EditorWindow containerWindow { get; set; }

            private static int _index;

            public ModesDropdown()
            {
                dropdownClicked += OnClicked;

                style.minWidth = 150;
                var btn = this.Query<Button>().First();
                btn.clicked += () => { App.StartGameMode(btn.text); };
                btn.style.flexGrow = 1;

                RegisterCallback<AttachToPanelEvent>(e =>
                {
                    if (!TryGetGameModes(out var modes)) return;
                    _index = Mathf.Clamp(_index, 0, modes.Count);
                    btn.text = modes[_index].name;
                });
            }

            private bool TryGetGameModes(out List<ScriptableGameMode> gameModes)
            {
                var settings = AppSettings.GetOrCreateSettings();
                gameModes = settings.GameModes;
                return gameModes != null && gameModes.Any();
            }

            private void OnClicked()
            {
                if (!TryGetGameModes(out var modes)) return;

                _index = Mathf.Clamp(_index, 0, modes.Count - 1);
                Select(_index);

                var menu = new GenericMenu();
                for (int i = 0; i < modes.Count; i++)
                {
                    var index = i;
                    var mode = modes[index];
                    menu.AddItem(new GUIContent(mode.name), _index == index, () => Select(index));
                }

                void Select(int index)
                {
                    _index = index;
                    var btn = this.Query<Button>().First();
                    btn.text = modes[_index].name;
                }

                menu.ShowAsContext();
            }

            // static readonly Color[] colors = new Color[] {Color.red, Color.green, Color.cyan};
            //
            // public DropdownToggleExample()
            // {
            //     text = "Color Bar";
            //     tooltip =
            //         "Display a color rectangle in the top left of the Scene view. Toggle on or off, and open the dropdown" +
            //         "to change the color.";
            //
            //     // When the dropdown is opened, ShowColorMenu is invoked and we can create a popup menu.
            //
            //     dropdownClicked += ShowColorMenu;
            //
            //     // Subscribe to the Scene view OnGUI callback so that we can draw our color swatch.
            //
            //     SceneView.duringSceneGui += DrawColorSwatch;
            // }
            //
            // void DrawColorSwatch(SceneView view)
            // {
            //     // Test that this callback is for the Scene View that we're interested in, and also check if the toggle is on
            //     // or off (value).
            //
            //     if (view != containerWindow || !value)
            //     {
            //         return;
            //     }
            //
            //     Handles.BeginGUI();
            //     GUI.color = colors[colorIndex];
            //     GUI.DrawTexture(new Rect(8, 8, 120, 24), Texture2D.whiteTexture);
            //     GUI.color = Color.white;
            //     Handles.EndGUI();
            // }
            //
            // // When the dropdown button is clicked, this method will create a popup menu at the mouse cursor position.
            //
            // void ShowColorMenu()
            // {
            //     var menu = new GenericMenu();
            //     menu.AddItem(new GUIContent("Red"), colorIndex == 0, () => colorIndex = 0);
            //     menu.AddItem(new GUIContent("Green"), colorIndex == 1, () => colorIndex = 1);
            //     menu.AddItem(new GUIContent("Blue"), colorIndex == 2, () => colorIndex = 2);
            //     menu.ShowAsContext();
            // }
        }
    }
}