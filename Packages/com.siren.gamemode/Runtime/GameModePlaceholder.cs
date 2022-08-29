using System;
using System.Linq;
using UnityEngine;

namespace GameMode
{
    public class GameModePlaceholder : MonoBehaviour
    {
        [SerializeField] private ScriptableGameMode[] modes;

        public ScriptableGameMode[] Modes => modes;
    }
}