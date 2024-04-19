using System;
using ToastForUnity.Script.Core;
using UnityEngine.Events;

namespace ToastForUnity.Resources.ToastSettings.Progress
{
    public enum ProgressToastStatus
    {
        Success,
        Warning,
        Danger,
        Info
    }
    
    public class ProgressToastModel : ToastModelBase
    {
        public string Title;
        public bool ProgressRunOnStart;
        public bool DestroyWhenProgressComplete;
        public ProgressToastStatus Status;
        public UnityAction<float> ProgressValueChanged;
        public Action ProgressDone;
    }
}