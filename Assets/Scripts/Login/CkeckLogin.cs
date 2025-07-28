using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class CkeckLogin : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private GameObject mainMenu;


    [SerializeField] public string LoginScene;
    [SerializeField] public string MainScene;

    [Header("Loading Bar")]
    [SerializeField] private Slider loadingBar;

    /// <summary>
    /// Checks if the player is logged in and loads the appropriate scene.
    /// </summary>
    public void CheckLogin()
    {
        // Check if the player is logged in
        if (PlayerPrefs.HasKey("SavedUser"))
        {
            mainMenu.SetActive(false);
            loadingScene.SetActive(true);
            // Player is logged in, proceed to the next scene
            StartCoroutine(LoadScenceASync(MainScene));
        }
        else
        {
            mainMenu.SetActive(false);
            loadingScene.SetActive(true);
            // Player is not logged in, show login UI or handle accordingly
            StartCoroutine(LoadScenceASync(LoginScene));
        }
    }

    IEnumerator LoadScenceASync(string scenetoLoad)
    {
        // Start the asynchronous loading operation
        UnityEngine.AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scenetoLoad);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingBar.value = progressValue;
            yield return null; // Wait for the next frame
        }
    }

}
