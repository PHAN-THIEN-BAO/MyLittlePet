using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ASyncLoading : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private GameObject mainMenu;

    [Header("Loading Bar")]
    [SerializeField] private Slider loadingBar;

    public void LoadScene(string scenetoLoad)
    {
        mainMenu.SetActive(false);
        loadingScene.SetActive(true);
        StartCoroutine(LoadScenceASync(scenetoLoad));
    }

    IEnumerator LoadScenceASync(string scenetoLoad)
    {
        // Start the asynchronous loading operation
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scenetoLoad);

        while (!loadOperation.isDone) 
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingBar.value = progressValue;
            yield return null; // Wait for the next frame
        }
    }
}
