using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMode.GettingStarted
{
    public class SampleSceneScope : LifetimeScope
    {
        public string key;

        protected override void Awake()
        {
            Debug.Log(nameof(SampleSceneScope) + key);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<VEntry>().WithParameter(key);
        }
    }

    public class VEntry : IInitializable, IStartable, IPostStartable, IAsyncStartable, ITickable
    {
        private readonly string _key;

        public VEntry(string key)
        {
            _key = key;
        }

        public void Initialize()
        {
            Debug.Log($"[{_key}] {nameof(VEntry)}: {nameof(Initialize)}");
        }

        public void Start()
        {
            Debug.Log($"[{_key}] {nameof(VEntry)}: {nameof(Start)}");
        }

        public void PostStart()
        {
            Debug.Log($"[{_key}] {nameof(VEntry)}: {nameof(PostStart)}");
        }

        private LinkedTask _tasks = new LinkedTask();

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            _tasks.Append(UniTask.Lazy(async () =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellation);
                Debug.Log("First");
            }));
            _tasks.Append(UniTask.Lazy(async () =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellation);
                Debug.Log("Second");
            }));
            _tasks.Append(UniTask.Lazy(async () =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellation);
                Debug.Log("Third");
            }));
        }

        private int n;

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AppendAsync().Forget();
            }
        }

        private async UniTaskVoid AppendAsync()
        {
            var isAny = _tasks.Any();
            _tasks.Append(UniTask.Lazy(async () =>
            {
                var value = ++n;
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                Debug.Log(value.ToString());
            }));
            if (!isAny)
            {
                await _tasks;
            }
        }
    }
}