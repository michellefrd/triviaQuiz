using ToastForUnity.Resources.ToastSettings.Stylish;
using ToastForUnity.Script.Core;
using ToastForUnity.Script.Enum;
using UnityEngine.UI;

namespace ToastForUnity.DemoScene.Scripts
{
    public class StylishPanel : DemoPanelBase
    {
        public ParentController ParentController;
        public Button DangerBtn;
        public Button QuestionBtn;
        public Button SuccessBtn;
        public Button WarningBtn;

        private void Start()
        {
            DangerBtn.onClick.AddListener(() =>
            {
                StylishPop("StylishToast-Danger", new StylistToastModel()
                {
                    Title = "Danger",
                    Content = "Wow, This is danger! The program might be crashing..."
                });
            });

            QuestionBtn.onClick.AddListener(() =>
            {
                StylishPop("StylishToast-Question", new StylistToastModel()
                {
                    Title = "Question",
                    Content = "Feel free to ask any question :D"
                });
            });

            SuccessBtn.onClick.AddListener(() =>
            {
                StylishPop("StylishToast-Success", new StylistToastModel()
                {
                    Title = "Success",
                    Content = "Congratulation! You made it! You are just amazing!"
                });
            });

            WarningBtn.onClick.AddListener(() =>
            {
                StylishPop("StylishToast-Warning", new StylistToastModel()
                {
                    Title = "Warning",
                    Content = "Oh ou! Please be careful."
                });
            });
        }

        private void StylishPop(string stylishName, StylistToastModel toastModel)
        {
            Toast.PopOut<StylistToastView>(stylishName, toastModel,
                ParentController.GetParent(ToastPosition.BottomCenter));
        }
    }
}