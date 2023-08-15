using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameMode.Editor.Overlays
{
    [Overlay(typeof(SceneView), OverlayId, "GameModePlayerOverlay", true)]
    public class GameModePlayerOverlay : Overlay
    {
        private const string OverlayId = "game-mode-player-overlay";

        private readonly List<string> _choices = new List<string>();

        public override void OnCreated()
        {
            
        }

        public override VisualElement CreatePanelContent()
        {
            var dropdown = new DropdownField(_choices, 0)
            {
                style =
                {
                    minWidth = 50
                }
            };
            dropdown.RegisterCallback<ClickEvent>(e =>
            {
                Debug.Log("cli");
                _choices.Add("item");
            });
            dropdown.RegisterValueChangedCallback(e =>
            {
                // dropdown.label = e.newValue;
            });
            return dropdown;
            return new Button(() => { Debug.Log("World"); })
            {
                text = "Hello",
                style =
                {
                    minWidth = 120
                }
            };
        }
    }
}