using UnityEngine;
public class QuitApp : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        Debug.Log("Game is quitting...");
    }
}