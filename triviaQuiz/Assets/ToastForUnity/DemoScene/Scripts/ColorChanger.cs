using UnityEngine;
using UnityEngine.UI;

namespace ToastForUnity.DemoScene.Scripts
{
    public class ColorChanger : MonoBehaviour
    {
        public Image Background;
        public Color CurrentColor;
        public Button ColorBtn;

        private void Start()
        {
            ColorBtn.onClick.AddListener(ChangeBackgroundColor);
        }

        private void ChangeBackgroundColor()
        {
            Background.color = CurrentColor;
        }
    }
}