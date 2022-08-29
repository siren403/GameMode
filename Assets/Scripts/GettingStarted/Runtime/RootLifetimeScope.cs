using System;
using VContainer;
using VContainer.Unity;

namespace GameMode.GettingStarted
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IRootProvider, RootProvider>(Lifetime.Singleton);
        }
    }

    public interface IRootProvider
    {
        string GetId();
    }

    public class RootProvider : IRootProvider
    {
        public string GetId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}