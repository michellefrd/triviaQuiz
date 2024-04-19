using System;
using ToastForUnity.Script.Core;

namespace ToastForUnity.Resources.ToastSettings.General.WindowPanel
{
    public class WindowToastModel : ToastModelBase
    {
        public string Title;
        public string Content;
        public Action OkBtnEvent;
        public Action CancelBtnEvent;
    }
}