using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameMode.GettingStarted
{
    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log($"{gameObject.scene.name}: {nameof(Awake)}");
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log($"{gameObject.scene.name}: {nameof(Start)}");
        }

       
        private void OnEnable()
        {
            Debug.Log($"{gameObject.scene.name}: {nameof(OnEnable)}");
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}