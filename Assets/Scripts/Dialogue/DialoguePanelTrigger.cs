using UnityEngine;

public class DialougePanelTrigger : MonoBehaviour
{
    [Header("Nội dung hiển thị")]
    public Sprite dialogueImage;
    public string dialogueTitle = "Tiêu đề";
    [TextArea(3, 5)]
    public string dialogueDescription = "Nội dung mô tả ở đây...";

    [Header("Cài đặt kích hoạt")]
    [Tooltip("Nếu true, dialogue sẽ tự động hiển thị khi va chạm với Player")]
    public bool triggerOnContact = true;
    [Tooltip("Nếu true, dialogue chỉ hiển thị một lần")]
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
            Debug.LogError("DialoguePanelManager không tồn tại trong scene!");
        }
    }

    public void HideDialogue()
    {
        if (DialoguePanelManager.Instance != null)
        {
            DialoguePanelManager.Instance.HideDialogue();
        }
    }

    // Reset trạng thái đã hiển thị
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}