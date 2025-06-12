using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    [Header("Settings")] [SerializeField] public string mainSceneName;
   
    public void OnButtonClick()
    {
        SceneManager.LoadScene(mainSceneName);

    }
    
    
    
}
