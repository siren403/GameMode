using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMode
{
    public interface ISceneContext
    {
    }

    public interface ISceneContextDescriptor<out T> : IDisposable where T : class, ISceneContext
    {
        string Name { get; }
        LoadSceneMode LoadMode { get; }
        T Context { get; }
    }

    public abstract class SceneContextDescriptorBase<T> : ISceneContextDescriptor<T> where T : class, ISceneContext
    {
        public abstract string Name { get; }
        public abstract LoadSceneMode LoadMode { get; }

        private T _context;
        public T Context => _context ??= SceneContextManager.GetContext<T>(Name);
        public void Dispose()
        {
            _context = null;
        }
    }


    public abstract class SceneContextBase : MonoBehaviour, ISceneContext
    {
        protected virtual void Awake()
        {
            SceneContextManager.Register(gameObject.scene.name, this);
        }

        protected void OnDestroy()
        {
            SceneContextManager.Unregister(gameObject.scene.name);
        }
    }

    public static class SceneContextManager
    {
        private static readonly Dictionary<string, ISceneContext> Contexts = new Dictionary<string, ISceneContext>();

        public static void Register(string name, ISceneContext context)
        {
            if (Contexts.ContainsKey(name))
            {
                throw new Exception($"duplicated scene context: {name}");
            }

            Contexts.Add(name, context);
        }

        public static void Unregister(string name)
        {
            Contexts.Remove(name);
        }

        public static T GetContext<T>(string name) where T : class, ISceneContext
        {
            if (Contexts.TryGetValue(name, out var cached) && cached is T context)
            {
                return context;
            }

            throw new Exception($"not implementation context: {typeof(T).Name}");
        }
    }
}