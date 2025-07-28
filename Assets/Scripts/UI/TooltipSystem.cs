using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }
    
    [Header("Tooltip UI")]
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;
    public Image tooltipBackground;
    public float showDelay = 0.5f;
    public Vector2 offset = new Vector2(10, 10);

    [Header("Fixed Position Settings")]
    public bool useFixedPosition = true;
    public Vector2 fixedPosition = new Vector2(100, -100);
    
    private Canvas canvas;
    private RectTransform canvasRect;
    private Coroutine showCoroutine;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            canvas = GetComponentInParent<Canvas>();
            canvasRect = canvas.GetComponent<RectTransform>();
            
            if (tooltipPanel != null)
                tooltipPanel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ShowTooltip(string text, Vector2 position = default)
    {
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);
            
        Vector2 actualPosition = useFixedPosition ? fixedPosition : position;
        showCoroutine = StartCoroutine(ShowTooltipDelayed(text, actualPosition));
    }
    
    public void HideTooltip()
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }
        
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
    
    private System.Collections.IEnumerator ShowTooltipDelayed(string text, Vector2 position)
    {
        yield return new WaitForSeconds(showDelay);
        
        if (tooltipPanel != null && tooltipText != null)
        {
            tooltipText.text = text;
            tooltipPanel.SetActive(true);
            
            RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
            
            if (useFixedPosition)
            {
                tooltipRect.anchoredPosition = fixedPosition;
            }
            else
            {
                Vector2 localPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, position + offset, canvas.worldCamera, out localPosition);
                
                Vector3[] canvasCorners = new Vector3[4];
                canvasRect.GetWorldCorners(canvasCorners);
                Vector3[] tooltipCorners = new Vector3[4];
                tooltipRect.GetWorldCorners(tooltipCorners);
                
                if (localPosition.x + tooltipRect.rect.width > canvasRect.rect.width / 2)
                    localPosition.x -= tooltipRect.rect.width + offset.x * 2;
                    
                if (localPosition.y + tooltipRect.rect.height > canvasRect.rect.height / 2)
                    localPosition.y -= tooltipRect.rect.height + offset.y * 2;
                
                tooltipRect.localPosition = localPosition;
            }
        }
    }
    
    public void SetTooltipColor(Color backgroundColor, Color textColor)
    {
        if (tooltipBackground != null)
            tooltipBackground.color = backgroundColor;
        if (tooltipText != null)
            tooltipText.color = textColor;
    }
}