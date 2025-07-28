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

    public void CheckLogin()
    {
        if (PlayerPrefs.HasKey("SavedUser"))
        {
            mainMenu.SetActive(false);
            loadingScene.SetActive(true);
            StartCoroutine(LoadScenceASync(MainScene));
        }
        else
        {
            mainMenu.SetActive(false);
            loadingScene.SetActive(true);
            StartCoroutine(LoadScenceASync(LoginScene));
        }
    }

    IEnumerator LoadScenceASync(string scenetoLoad)
    {
        UnityEngine.AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scenetoLoad);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingBar.value = progressValue;
            yield return null;
        }
    }

}