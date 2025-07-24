using UnityEngine;

public class DialougePanelTrigger : MonoBehaviour
{
    [Header("N?i dung hi?n th?")]
    public Sprite dialogueImage;
    public string dialogueTitle = "Tiêu d?";
    [TextArea(3, 5)]
    public string dialogueDescription = "N?i dung mô t? ? dây...";

    [Header("Cài d?t kích ho?t")]
    [Tooltip("N?u true, dialogue s? t? d?ng hi?n th? khi va ch?m v?i Player")]
    public bool triggerOnContact = true;
    [Tooltip("N?u true, dialogue ch? hi?n th? m?t l?n")]
    public bool showOnce = false;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnContact && (!showOnce || !hasTriggered))
            {
                ShowDialogue();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HideDialogue();
        }
    }

    public void ShowDialogue()
    {
        if (showOnce && hasTriggered)
            return;

        if (DialoguePanelManager.Instance != null)
        {
            DialoguePanelManager.Instance.ShowDialogue(dialogueImage, dialogueTitle, dialogueDescription);
            hasTriggered = true;
        }
        else
        {
            Debug.LogError("DialoguePanelManager không t?n t?i trong scene!");
        }
    }

    public void HideDialogue()
    {
        if (DialoguePanelManager.Instance != null)
        {
            DialoguePanelManager.Instance.HideDialogue();
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}