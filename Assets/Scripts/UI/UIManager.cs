using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject PanelUI;


    public void OnPress()
    {
        PanelUI.SetActive(true);
    }
    public void OnBackPress()
    {
        PanelUI.SetActive(false);
    }
}
