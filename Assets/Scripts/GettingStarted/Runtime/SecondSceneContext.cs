using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace GameMode.GettingStarted
{
    public interface ISecondContext : ISceneContext
    {
        void InstantiatePlane();
    }

    public class SecondSceneDescriptor : ISceneContextDescriptor<ISecondContext>
    {
        public string Name => "SecondScene";
        public LoadSceneMode LoadMode => LoadSceneMode.Additive;

        private SecondSceneContext _context;

        public ISecondContext Context
        {
            get
            {
                if (_context == null)
                {
                    var context = LifetimeScope.Find<SecondSceneContext>() as SecondSceneContext;
                    if (context == null)
                    {
                        throw new Exception("Not found context");
                    }

                    context.Build();
                    _context = context;
                }

                return _context;
            }
        }

        public void Dispose()
        {
        }
    }

    public class SecondSceneContext : LifetimeScope, ISecondContext
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPoints>();
        }

        public void InstantiatePlane()
        {
            Debug.Log("create plane");
            GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        }

        public class EntryPoints : IInitializable
        {
            private readonly IRootProvider _provider;

            public EntryPoints(IRootProvider provider)
            {
                _provider = provider;
            }

            public void Initialize()
            {
                Debug.Log(_provider.GetId());
                Debug.Log(nameof(SecondSceneContext) + nameof(Initialize));
            }
        }
    }
}