using ToastForUnity.DemoScene.Scripts;
using UnityEngine;

public class ToastDemoScript : MonoBehaviour
{
    [Header("Setting Panel")] 
    public DemoPanelBase[] PanelSettingList;

    private void Start()
    {
        foreach (var panel in PanelSettingList)
        {
            panel.PanelBtn.onClick.AddListener(()=>
            {
                TriggerPanel(panel.MainPanel);
            });
        }
    }

    private void TriggerPanel(GameObject panelObj)
    {
        foreach (var panel in PanelSettingList)
        {
            panel.MainPanel.SetActive(false);
        }
        
        panelObj.SetActive(true);
    }
}
