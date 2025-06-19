using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutUser : MonoBehaviour
{
    [SerializeField] public string LoadScene;

   
    public void LogOut()
    {
        // Clear player information
        PlayerInfomation.ClearPlayerInfo();
        // go to the login scene
        SceneManager.LoadScene(LoadScene);
    }


}
