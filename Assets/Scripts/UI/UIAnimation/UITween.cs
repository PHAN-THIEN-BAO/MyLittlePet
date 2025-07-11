using UnityEngine;

public class UITween : MonoBehaviour
{
    [SerializeField] GameObject pannelUI;
    [SerializeField] public float duration = 1f;
    [SerializeField] public float delay = 0.5f;
    [SerializeField] public float rootSize = 1f;



    public void OnOpenScaleEaseOutElastic()
    {
        pannelUI.transform.localScale = Vector3.zero;
        LeanTween.scale(pannelUI, new Vector3(rootSize, rootSize, rootSize), duration)
            .setDelay(delay)
            .setEase(LeanTweenType.easeOutElastic);
    }

    public void OnOpenScaleEaseOutBounce()
    {
        pannelUI.transform.localScale = Vector3.zero;
        LeanTween.scale(pannelUI, new Vector3(rootSize, rootSize, rootSize), duration)
            .setDelay(delay)
            .setEase(LeanTweenType.easeOutBounce);
    }

    public void OnOpenScaleEaseOutQuint()
    {
        pannelUI.transform.localScale = Vector3.zero;
        LeanTween.scale(pannelUI, new Vector3(rootSize, rootSize, rootSize), duration)
            .setDelay(delay)
            .setEase(LeanTweenType.easeOutQuint);
    }

    public void OnOpenFadeInPanel()
    {
        CanvasGroup canvasGroup = pannelUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = pannelUI.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        pannelUI.SetActive(true);
        LeanTween.value(pannelUI, 0f, 1f, duration)
            .setDelay(delay)
            .setOnUpdate((float val) => { canvasGroup.alpha = val; });
    }










    public void OnCloseScaleEaseInElastic(GameObject closePannel)
    {
        LeanTween.scale(pannelUI, Vector3.zero, duration)
            .setDelay(delay)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() =>
            {
                closePannel.SetActive(false);
            });
    }





}
