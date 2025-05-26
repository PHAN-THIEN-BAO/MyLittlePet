using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject StoreUI;


    public void OnStorePress()
    {
        StoreUI.SetActive(true);
    }
    public void OnBackStorePress()
    {
        StoreUI.SetActive(false);
    }
}
