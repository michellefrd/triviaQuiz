using ToastForUnity.Script.Core;
using UnityEngine.UI;

namespace ToastForUnity.Resources.ToastSettings.General.DialogToast
{
    public class DialogToastView : ToastPrefabBase
    {
        public Text dialogText;
        public override void Initialize(ToastModelBase toastModel)
        {
            dialogText.text = ((DialogToastModel)toastModel).DialogText;
        }
    }
}
