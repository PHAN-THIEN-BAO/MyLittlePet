using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    
    [Header("Transition Effect")]
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private float transitionDuration = 1f;
    
    public static Vector3 PlayerSpawnPosition { get; set; }
    public static bool ShouldRepositionPlayer { get; set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TransitionToScene(string sceneName, Vector3 playerSpawnPos)
    {
        StartCoroutine(TransitionCoroutine(sceneName, playerSpawnPos));
    }

    private IEnumerator TransitionCoroutine(string sceneName, Vector3 playerSpawnPos)
    {
        PlayerSpawnPosition = playerSpawnPos;
        ShouldRepositionPlayer = true;
        
        if (transitionPanel != null)
        {
            yield return StartCoroutine(FadeIn());
        }
        
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        transitionPanel.SetActive(true);
        CanvasGroup canvasGroup = transitionPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = transitionPanel.AddComponent<CanvasGroup>();
        
        float elapsed = 0;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / transitionDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    public IEnumerator FadeOut()
    {
        if (transitionPanel != null)
        {
            CanvasGroup canvasGroup = transitionPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = transitionPanel.AddComponent<CanvasGroup>();
            
            float elapsed = 0;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / transitionDuration);
                yield return null;
            }
            canvasGroup.alpha = 0;
            transitionPanel.SetActive(false);
        }
    }
}