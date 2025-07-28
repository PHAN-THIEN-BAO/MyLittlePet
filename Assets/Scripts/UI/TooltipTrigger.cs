using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip Settings")]
    [TextArea(3, 5)]
    public string tooltipText = "";
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    public Color textColor = Color.white;
    
    public System.Func<string> GetDynamicTooltip;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TooltipSystem.Instance != null)
        {
            string displayText = GetDynamicTooltip?.Invoke() ?? tooltipText;
            
            if (!string.IsNullOrEmpty(displayText))
            {
                TooltipSystem.Instance.SetTooltipColor(backgroundColor, textColor);
                TooltipSystem.Instance.ShowTooltip(displayText, Input.mousePosition);
            }
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipSystem.Instance != null)
        {
            TooltipSystem.Instance.HideTooltip();
        }
    }
    
    public void SetTooltipText(string text)
    {
        tooltipText = text;
    }
    
    public void SetTooltipColors(Color bgColor, Color txtColor)
    {
        backgroundColor = bgColor;
        textColor = txtColor;
    }
}