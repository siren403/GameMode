using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameMode
{
    public class LinkedTask
    {
        private readonly LinkedList<AsyncLazy> _linkedList = new();

        private LinkedListNode<AsyncLazy> _currentNode;

        public bool Any() => _linkedList.Any() && _currentNode != null;

        public void Append(AsyncLazy task)
        {
            if (!_linkedList.Any())
            {
                _linkedList.AddFirst(new LinkedListNode<AsyncLazy>(task));
            }
            else
            {
                _linkedList.AddLast(new LinkedListNode<AsyncLazy>(task));
            }
        }

        public UniTask.Awaiter GetAwaiter()
        {
            if (!_linkedList.Any())
            {
#if UNITY_EDITOR
                return UniTask.Create(() =>
                {
                    Debug.Log("empty linked task");
                    return UniTask.CompletedTask;
                }).GetAwaiter();
#else
                return UniTask.CompletedTask.GetAwaiter();
#endif
            }

            if (_currentNode != null) return _currentNode.Value.GetAwaiter();

            _currentNode = _linkedList.First;
            return UniTask.Create(async () =>
            {
                while (_currentNode != null)
                {
                    await _currentNode.Value;
                    if (_currentNode.Next == null)
                    {
                        break;
                    }

                    _currentNode = _currentNode.Next;
                }

                _currentNode = null;
                _linkedList.Clear();
            }).GetAwaiter();
        }

        public void Clear()
        {
            _linkedList.Clear();
            _currentNode = null;
        }
    }
}