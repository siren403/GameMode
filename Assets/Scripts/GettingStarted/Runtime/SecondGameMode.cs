using System;
using Cysharp.Threading.Tasks;

namespace GameMode.GettingStarted
{
    public class SecondGameMode : GameModeBase
    {
        public override async UniTask OnStartAsync()
        {
            BeginState();
            var secondScene = new SecondSceneDescriptor();
            await secondScene.LoadSceneAsync();
            EndState();

            var context = secondScene.Context;
            context.InstantiatePlane();
        }

        public override async UniTask OnEndAsync()
        {
            BeginState();
            var secondScene = new FirstSceneDescriptor();
            await secondScene.UnloadSceneAsync();
            EndState();
        }
    }
}