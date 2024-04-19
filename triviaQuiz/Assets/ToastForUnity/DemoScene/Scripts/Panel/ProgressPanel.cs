using ToastForUnity.Resources.ToastSettings.Progress;
using ToastForUnity.Script.Core;
using ToastForUnity.Script.Enum;
using UnityEngine.UI;

namespace ToastForUnity.DemoScene.Scripts
{
    public class ProgressPanel : DemoPanelBase
    {
        public ParentController ParentController;
        public Dropdown StatusDropdown;
        public InputField ContentInput;
        public Button ProgressBtn;

        private void Start()
        {
            ProgressBtn.onClick.AddListener(ProgressPop);
        }

        private void ProgressPop()
        {
            Toast.PopOut<ProgressToastView>("ProgressToast", new ProgressToastModel()
            {
                DestroyWhenProgressComplete = true,
                ProgressDone = () =>
                {
                    Toast.PopOut("Progress Done", ToastStatus.Success,
                        ParentController.GetParent(ToastPosition.BottomCenter));
                },
                ProgressRunOnStart = true,
                Status = (ProgressToastStatus)StatusDropdown.value,
                Title = ContentInput.text,
                ProgressValueChanged = null
            });
        }
    }
}