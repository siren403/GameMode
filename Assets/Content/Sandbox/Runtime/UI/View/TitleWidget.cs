using UnityEngine;
using UnityEngine.UI;

namespace Sandbox
{
    public class TitleWidget : MonoBehaviour, ITitleWidget
    {
        [SerializeField] private Button startButton;
        public Button StartButton => startButton;
    }
}