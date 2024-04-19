using ToastForUnity.Resources.ToastSettings.General.DefaultToast;
using ToastForUnity.Script.Core;
using UnityEngine;
using UnityEngine.UI;

public class DefaultToast : ToastPrefabBase
{
    public Text Info;
    public Image Status;
    public void Initialize(string popText, Color color)
    {
        Info.text = popText;
        Status.color = color;
    }

    public override void Initialize(ToastModelBase toastModel)
    {
        var defaultModel = toastModel as DefaultToastModel;
        if (defaultModel == null) return;
        Info.text = defaultModel.PopText;
        Status.color = defaultModel.Color;
    }
}