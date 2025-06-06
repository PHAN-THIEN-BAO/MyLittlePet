using UnityEngine;

public class UIManagerWaring : MonoBehaviour
{
    public GameObject PanelUI;
    public GameObject PaneWaringlUI;


    public void OnPressWaring()
    {
        PaneWaringlUI.SetActive(true);
    }
    // if player pressed yes Waring
    public void OnPress()
    {
        HideAll();
        PanelUI.SetActive(true);
    }
    // if player pressed no Waring
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
