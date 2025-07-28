using UnityEngine;
public class QuitApp : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        // If we are running in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // If we are running in a standalone build
            Application.Quit();
#endif
        Debug.Log("Game is quitting...");
    }
}
