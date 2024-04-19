using System;
using ToastForUnity.Script.Core;
using UnityEngine.UI;

namespace ToastForUnity.Resources.ToastSettings.Stylish
{
    public class StylistToastView : ToastPrefabBase
    {
        public Text TitleText;
        public Text ContentText;
        public Button CloseBtn;

        private void Start()
        {
            CloseBtn.onClick.AddListener(()=>{Destroy(gameObject);});
        }

        public override void Initialize(ToastModelBase toastModel)
        {
            var stylistToastModel = toastModel as StylistToastModel;
            if (stylistToastModel != null)
            {
                TitleText.text = stylistToastModel.Title;
                ContentText.text = stylistToastModel.Content;
            }
        }
    }
}
