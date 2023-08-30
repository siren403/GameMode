using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
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
        class StartFromMasterButton : ToolbarButton
        {
            public const string ID = nameof(GameModeToolbar) + "/" + nameof(StartFromMasterButton);
            private readonly PlayModeListener _playModeListener;

            public StartFromMasterButton()
            {
                clicked += OnClick;
                var image = EditorGUIUtility.IconContent("d_PlayButton").image;
                Add(new Image()
                {
                    image = image
                });
                _playModeListener = new PlayModeListener(this, OnPlayModeStateChanged);
            }

            private void OnClick()
            {
                App.StartGameModeFirst();
            }

            private void OnPlayModeStateChanged(PlayModeStateChange change)
            {
                SetEnabled(change.IsEdit());
            }
        }

        [EditorToolbarElement(ID, typeof(SceneView))]
        class ModesDropdown : EditorToolbarDropdownToggle, IAccessContainerWindow
        {
            public const string ID = nameof(GameModeToolbar) + "/" + nameof(ModesDropdown);
            public EditorWindow containerWindow { get; set; }

            private static int _index;
            private readonly Button _playModeButton;
            private readonly PlayModeListener _playModeListener;

            public ModesDropdown()
            {
                dropdownClicked += OnClicked;

                style.minWidth = 150;
                _playModeButton = this.Query<Button>().First();
                _playModeButton.clicked += () => { App.StartGameMode(_playModeButton.text); };
                _playModeButton.style.flexGrow = 1;

                _playModeListener = new PlayModeListener(this, OnPlayModeStateChanged);
                RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            }

            private void OnAttachToPanel(AttachToPanelEvent e)
            {
                if (!TryGetGameModes(out var modes)) return;
                _index = Mathf.Clamp(_index, 0, modes.Count);
                _playModeButton.text = modes[_index].name;
            }

            private void OnPlayModeStateChanged(PlayModeStateChange change)
            {
                _playModeButton.SetEnabled(change.IsEdit());
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

    sealed class PlayModeListener
    {
        private readonly VisualElement _element;
        private readonly Action<PlayModeStateChange> _onChanged;

        public PlayModeListener(VisualElement element, Action<PlayModeStateChange> onChanged)
        {
            _element = element;
            _onChanged = onChanged;
            _element.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            _element.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            _element.UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            _element.UnregisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            _onChanged?.Invoke(change);
        }
    }

    public static class PlayModeExtensions
    {
        public static bool IsPlay(this PlayModeStateChange change)
            => change is PlayModeStateChange.ExitingEditMode or PlayModeStateChange.EnteredPlayMode;

        public static bool IsEdit(this PlayModeStateChange change)
            => change is PlayModeStateChange.EnteredEditMode or PlayModeStateChange.ExitingPlayMode;
    }
}