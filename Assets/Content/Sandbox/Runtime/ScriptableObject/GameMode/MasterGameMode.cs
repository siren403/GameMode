using System;
using Cysharp.Threading.Tasks;
using GameMode;
using Sandbox.UI.Elements;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Object = UnityEngine.Object;
using Text = Sandbox.UI.Elements.Text;

namespace Sandbox
{
    public static class CanvasExtensions
    {
        public static void UseMobileScale(this CanvasScaler scaler, Vector2 targetSize)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = targetSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        }
    }

    [CreateAssetMenu(menuName = "Game Mode/" + nameof(MasterGameMode), fileName = "MasterGameMode", order = 0)]
    public class MasterGameMode : ScriptableGameMode
    {
        [Inject] private WidgetProviderRegister<ITitleWidget> _provider;
        [Inject] private IObjectResolver _resolver;

        public override async UniTask OnStartAsync()
        {
            Canvas((canvas, scaler) =>
                {
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    scaler.UseMobileScale(new Vector2(1080, 1920));
                },
                Widget<ITitleWidget>(_ =>
                {
                    _.StartButton.onClick.AddListener(GameModeManager.SwitchMode<LobbyGameMode>);
                }),
                (parent) =>
                {
                    var text = _resolver.Resolve<IText>();
                    text.SetParent(parent);
                    text.Value = "Text Element";

                    var t2 = _resolver.Resolve<IBackgroundText>();
                    t2.SetParent(parent);
                    t2.Value = @"1
2
3
4
5
";
                    var t3 = _resolver.Resolve<IBackgroundText>();
                    t3.SetParent(parent);
                    t3.Value = @"a b c d e";
                    t3.Position = new Vector2(100, 0);

                    _resolver.Resolve<RedText>().InstantiateAsync(parent).ContinueWith(_ => { _.Value = "red"; });
                });
        }

        private Action<Transform> Widget<TWidget>(Action<TWidget> configuration = null) where TWidget : IWidget
        {
            return parent =>
            {
                var widget = _provider.Instantiate(parent);
                if (widget is TWidget w)
                {
                    configuration?.Invoke(w);
                }
            };
        }

        private void Canvas(Action<Canvas, CanvasScaler> configuration, params Action<Transform>[] children)
        {
            var canvas = new GameObject("Canvas")
                .AddComponent<Canvas>();
            var raycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
            var scaler = canvas.gameObject.AddComponent<CanvasScaler>();

            configuration(canvas, scaler);
            foreach (var child in children)
            {
                child?.Invoke(canvas.transform);
            }
        }

        public override async UniTask OnEndAsync()
        {
        }
    }
}