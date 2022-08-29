using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace GameMode.GettingStarted
{
    public interface IFirstContext : ISceneContext
    {
        void InstantiateCube();
    }

    public class FirstSceneDescriptor : ISceneContextDescriptor<IFirstContext>
    {
        public string Name => "FirstScene";
        public LoadSceneMode LoadMode => LoadSceneMode.Additive;
        public IFirstContext Context => LifetimeScope.Find<FirstSceneContext>() as IFirstContext;

        public void Dispose()
        {
        }
    }

    public class FirstSceneContext : LifetimeScope, IFirstContext
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<IFirstContext>(this);
        }

        public void InstantiateCube()
        {
            Debug.Log("create cube");
            GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
    }


  
}