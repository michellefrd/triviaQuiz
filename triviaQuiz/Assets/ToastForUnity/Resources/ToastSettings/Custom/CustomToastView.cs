using ToastForUnity.Script.Core;
using UnityEngine.UI;

public class CustomToastView : ToastPrefabBase
{
    public Text ContentText;

    public override void Initialize(ToastModelBase toastModel)
    {
        CustomModel customModel = toastModel as CustomModel;
        if (customModel != null) 
            ContentText.text = customModel.Content;
    }
}
