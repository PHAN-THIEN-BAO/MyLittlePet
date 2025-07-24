using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{

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