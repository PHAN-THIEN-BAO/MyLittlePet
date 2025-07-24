using UnityEngine;

public class UIManagerWaring : MonoBehaviour
{
    public GameObject PanelUI;
    public GameObject PaneWaringlUI;


    public void OnPressWaring()
    {
        PaneWaringlUI.SetActive(true);
    }
    public void OnPress()
    {
        HideAll();
        PanelUI.SetActive(true);
    }
    public void OnBackPress()
    {
        HideAll();
    }

    public void HideAll()
    {
        PanelUI.SetActive(false);
        PaneWaringlUI.SetActive(false);
    }
}