using System;
using Cysharp.Threading.Tasks;

namespace GameMode.GettingStarted
{
    public class FirstGameMode : GameModeBase
    {
        public override async UniTask OnStartAsync()
        {
            BeginState();
            var firstScene = new FirstSceneDescriptor();
            await firstScene.LoadSceneAsync();
            EndState();

            var context = firstScene.Context;
            context.InstantiateCube();

            GameModeManager.SwitchMode(new SecondGameMode());

            await UniTask.Delay(TimeSpan.FromSeconds(3));
        }

        public override async UniTask OnEndAsync()
        {
            BeginState();
            var firstScene = new FirstSceneDescriptor();
            await firstScene.UnloadSceneAsync();
            EndState();
        }
    }
}