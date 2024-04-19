using ToastForUnity.DemoScene.Scripts;
using ToastForUnity.Script.Core;
using ToastForUnity.Script.Enum;
using UnityEngine.UI;

public class ToastPanel : DemoPanelBase
{
    public ParentController ParentPrefab;
    public Dropdown PositionDropdown;
    public InputField ContentInput;
    public Button ToastBtn;

    private void Start()
    {
        ToastBtn.onClick.AddListener(ToastPop);
    }

    private void ToastPop()
    {
        Toast.PopOut(ContentInput.text, ToastStatus.Success, ParentPrefab.GetParent((ToastPosition)PositionDropdown.value));
    }
}