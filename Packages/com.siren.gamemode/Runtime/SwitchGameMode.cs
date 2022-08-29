using UnityEngine;

namespace GameMode
{
    public class SwitchGameMode : MonoBehaviour
    {
        [SerializeField] private ScriptableGameMode mode;

        private void Start()
        {
            GameModeManager.SwitchMode(mode);
        }
    }
}