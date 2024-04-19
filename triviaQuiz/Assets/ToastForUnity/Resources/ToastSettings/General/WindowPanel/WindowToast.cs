using System;
using ToastForUnity.Resources.ToastSettings.General.WindowPanel;
using ToastForUnity.Script.Core;
using UnityEngine.UI;

public class WindowToast : ToastPrefabBase
{
    public Text TitleText;
    public Image TitleImage;

    public Text ContentText;
    public Button OkBtn;
    public Button CancelBtn;

    public Action okEvent;
    public Action cancelEvent;

    public void Initialize(string title, string content)
    {
        TitleText.text = title;
        ContentText.text = content;
        OkBtn.onClick.AddListener(ClickOKBtn);
        CancelBtn.onClick.AddListener(ClickCancelBtn);
    }

    private void ClickOKBtn()
    {
        okEvent?.Invoke();
        Destroy(this.gameObject);
    }
    
    private void ClickCancelBtn()
    {
        cancelEvent?.Invoke();
        Destroy(this.gameObject);
    }

    public override void Initialize(ToastModelBase toastModel)
    {
        var windowModel = toastModel as WindowToastModel;
        TitleText.text = windowModel.Title;
        ContentText.text = windowModel.Content;
        okEvent += windowModel.OkBtnEvent;
        cancelEvent += windowModel.CancelBtnEvent;
        OkBtn.onClick.AddListener(ClickOKBtn);
        CancelBtn.onClick.AddListener(ClickCancelBtn);
    }
}