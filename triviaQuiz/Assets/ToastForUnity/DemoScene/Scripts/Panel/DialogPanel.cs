using ToastForUnity.Script.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ToastForUnity.DemoScene.Scripts
{
    public class DialogPanel : DemoPanelBase
    {
        public GameObject Parent;
        public InputField ContentInput;
        public Button DialogBtn;

        private void Start()
        {
            DialogBtn.onClick.AddListener(DialogPop);
        }

        private void DialogPop()
        {
            Toast.DialogPopOut(ContentInput.text, Parent.transform);
        }
    }
}