using ToastForUnity.Resources.ToastSettings.General.WindowPanel;
using ToastForUnity.Script.Core;
using ToastForUnity.Script.Enum;
using UnityEngine.UI;

namespace ToastForUnity.DemoScene.Scripts
{
    public class WindowPanel : DemoPanelBase
    {
        public ParentController ParentController;
        public InputField TitleInput;
        public InputField ContentInput;
        public Button WinPanelBtn;

        private void Start()
        {
            WinPanelBtn.onClick.AddListener(PanelPop);
        }

        private void PanelPop()
        {
            Toast.WindowPopOut(new WindowToastModel()
            {
                Title = TitleInput.text,
                Content = ContentInput.text,
                OkBtnEvent = () =>
                {
                    Toast.PopOut("OK", ToastStatus.Success, ParentController.GetParent(ToastPosition.BottomCenter));
                },
                CancelBtnEvent = () =>
                {
                    Toast.PopOut("Cancel", ToastStatus.Danger, ParentController.GetParent(ToastPosition.BottomCenter));
                }
            });
        }
    }
}