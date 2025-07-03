//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//[System.Serializable]
//public class DialougeCharacter
//{
//    public string name;
//    public Sprite icon;
//}

//[System.Serializable]
//public class DialogueLine
//{
//    public DialougeCharacter character;
//    [TextArea(3, 10)]
//    public string line;
//}

//[System.Serializable]
//public class Dialogue
//{
//    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
//}

//public class DialougeTrigger : MonoBehaviour
//{
//    // Dialogue data
//    public Dialogue dialogue;

//    // UI References
//    public GameObject dialoguePanel;

//    // Trigger settings
//    [Tooltip("Nếu true, dialogue sẽ tự động trigger khi va chạm với Player")]
//    public bool triggerOnContact = true;

//    [Tooltip("Nếu true, dialogue chỉ trigger một lần")]
//    public bool triggerOnce = false;

//    // Events
//    [Tooltip("Event gọi khi dialogue bắt đầu")]
//    public UnityEvent onDialogueStart;

//    [Tooltip("Event gọi khi dialogue kết thúc")]
//    public UnityEvent onDialogueEnd;

//    private bool hasTriggered = false;

//    private void OnEnable()
//    {
//        // Đăng ký lắng nghe sự kiện khi dialogue kết thúc
//        DialogueManager.OnDialogueEnd += OnDialogueEnded;
//    }

//    private void OnDisable()
//    {
//        // Hủy đăng ký để tránh memory leak
//        DialogueManager.OnDialogueEnd -= OnDialogueEnded;
//    }

//    // Phương thức được gọi khi dialogue kết thúc
//    void OnDialogueEnded()
//    {
//        // Ẩn panel nếu nó tồn tại
//        if (dialoguePanel != null)
//            dialoguePanel.SetActive(false);

//        // Gọi event khi dialogue kết thúc
//        onDialogueEnd?.Invoke();
//    }

//    // Phương thức công khai để kích hoạt dialogue
//    public void TriggerDialogue()
//    {
//        // Kiểm tra nếu chỉ trigger một lần
//        if (triggerOnce && hasTriggered)
//            return;

//        hasTriggered = true;

//        // Hiển thị panel nếu nó tồn tại
//        if (dialoguePanel != null)
//            dialoguePanel.SetActive(true);

//        // Kiểm tra DialogueManager tồn tại
//        if (DialogueManager.Instance != null)
//        {
//            DialogueManager.Instance.StartDialog(dialogue);

//            // Gọi event khi dialogue bắt đầu
//            onDialogueStart?.Invoke();
//        }
//        else
//        {
//            Debug.LogError("DialogueManager.Instance không tồn tại! Đảm bảo có một GameObject với DialogueManager trong scene.");

//            // Ẩn lại panel nếu không có DialogueManager
//            if (dialoguePanel != null)
//                dialoguePanel.SetActive(false);
//        }
//    }

//    // Được gọi khi một Collider2D khác vào vùng trigger
//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (triggerOnContact && other.CompareTag("Player"))
//        {
//            TriggerDialogue();
//        }
//    }

//    // Tùy chọn: Thêm phương thức để tiếp tục dialogue
//    public void ContinueDialogue()
//    {
//        if (DialogueManager.Instance != null)
//        {
//            DialogueManager.Instance.ContinueDialogue();
//        }
//    }

//    // Phương thức để reset trigger
//    public void ResetTrigger()
//    {
//        hasTriggered = false;
//    }
//}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialougeCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialougeCharacter character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialougeTrigger : MonoBehaviour
{
    // Dialogue data
    public Dialogue dialogue;

    // UI References
    public GameObject dialoguePanel;

    // Trigger settings
    [Tooltip("Nếu true, dialogue sẽ tự động trigger khi va chạm với Player")]
    public bool triggerOnContact = true;

    [Tooltip("Nếu true, dialogue chỉ trigger một lần")]
    public bool triggerOnce = false;

    [Tooltip("Nếu true, dialogue sẽ không xuất hiện lại ngay cả khi tải lại scene")]
    public bool persistentTrigger = false;

    [Tooltip("ID duy nhất cho trigger này, cần thiết cho chế độ persistent")]
    public string triggerID = "";

    // Events
    [Tooltip("Event gọi khi dialogue bắt đầu")]
    public UnityEvent onDialogueStart;

    [Tooltip("Event gọi khi dialogue kết thúc")]
    public UnityEvent onDialogueEnd;

    private bool hasTriggered = false;

    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện khi dialogue kết thúc
        DialogueManager.OnDialogueEnd += OnDialogueEnded;

        // Kiểm tra xem trigger này đã được kích hoạt trước đó chưa (nếu là persistent)
        if (persistentTrigger && !string.IsNullOrEmpty(triggerID))
        {
            hasTriggered = PlayerPrefs.GetInt("DialogueTrigger_" + triggerID, 0) == 1;
        }
    }

    private void OnDisable()
    {
        // Hủy đăng ký để tránh memory leak
        DialogueManager.OnDialogueEnd -= OnDialogueEnded;
    }

    // Phương thức được gọi khi dialogue kết thúc
    void OnDialogueEnded()
    {
        // Ẩn panel nếu nó tồn tại
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Gọi event khi dialogue kết thúc
        onDialogueEnd?.Invoke();
    }

    // Phương thức công khai để kích hoạt dialogue
    public void TriggerDialogue()
    {
        // Kiểm tra nếu chỉ trigger một lần hoặc đã trigger và là persistent
        if ((triggerOnce || persistentTrigger) && hasTriggered)
            return;

        hasTriggered = true;

        // Lưu trạng thái trigger nếu là persistent
        if (persistentTrigger && !string.IsNullOrEmpty(triggerID))
        {
            PlayerPrefs.SetInt("DialogueTrigger_" + triggerID, 1);
            PlayerPrefs.Save();
        }

        // Hiển thị panel nếu nó tồn tại
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Kiểm tra DialogueManager tồn tại
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialog(dialogue);

            // Gọi event khi dialogue bắt đầu
            onDialogueStart?.Invoke();
        }
        else
        {
            Debug.LogError("DialogueManager.Instance không tồn tại! Đảm bảo có một GameObject với DialogueManager trong scene.");

            // Ẩn lại panel nếu không có DialogueManager
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }
    }

    // Được gọi khi một Collider2D khác vào vùng trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnContact && other.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }

    // Tùy chọn: Thêm phương thức để tiếp tục dialogue
    public void ContinueDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ContinueDialogue();
        }
    }

    // Phương thức để reset trigger
    public void ResetTrigger()
    {
        hasTriggered = false;

        // Xóa dữ liệu lưu trữ nếu là persistent
        if (persistentTrigger && !string.IsNullOrEmpty(triggerID))
        {
            PlayerPrefs.DeleteKey("DialogueTrigger_" + triggerID);
            PlayerPrefs.Save();
        }
    }
}