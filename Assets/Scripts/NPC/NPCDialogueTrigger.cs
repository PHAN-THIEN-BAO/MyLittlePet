using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("NPC Dialogue Settings")]
    public NPCDialog DialogData;
    public GameObject dialogPanel;
    public TMP_Text dialogText, nameText;
    public Image portraitImage;
    
    [Header("Trigger Settings")]
    [Tooltip("Tự động bắt đầu dialogue khi Player vào trigger")]
    public bool autoStartOnEnter = true;
    
    [Tooltip("Tự động đóng dialogue khi Player rời trigger")]
    public bool autoCloseOnExit = true;
    
    [Tooltip("Chỉ trigger dialogue một lần")]
    public bool triggerOnce = false;
    
    [Tooltip("Delay trước khi bắt đầu dialogue (giây)")]
    public float startDelay = 0.5f;

    [Header("Close Button Integration")]
    public Button closeButton;
    
    [Header("Destruction Settings")]
    [Tooltip("GameObject sẽ bị phá hủy khi dialogue kết thúc")]
    public GameObject targetToDestroy;
    
    [Tooltip("Tự phá hủy NPC này sau khi dialogue hoàn thành")]
    public bool destroySelfAfterDialogue = false;

    [Header("Visual Indicator")]
    [Tooltip("Hiển thị indicator khi player có thể tương tác")]
    public GameObject interactionIndicator;
    
    // Private variables
    private int dialogIndex;
    private bool isTyping, isDialogActive, hasTriggered;
    private bool isPlayerInRange = false;
    private Coroutine startDialogueCoroutine;

    private void Start()
    {
        // Setup close button if assigned
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        // Đảm bảo dialogue panel bị ẩn ban đầu
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }

        // Ẩn interaction indicator ban đầu
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        // Cho phép player nhấn Space để tiếp tục dialogue khi ở trong vùng trigger
        if (isDialogActive && isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"Player entered NPC dialogue trigger: {gameObject.name}");
        
        isPlayerInRange = true;

        // Hiển thị interaction indicator
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(true);
        }

        // Tự động bắt đầu dialogue nếu được thiết lập
        if (autoStartOnEnter && (!triggerOnce || !hasTriggered))
        {
            if (startDelay > 0)
            {
                startDialogueCoroutine = StartCoroutine(StartDialogueWithDelay());
            }
            else
            {
                StartDialog();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"Player exited NPC dialogue trigger: {gameObject.name}");
        
        isPlayerInRange = false;

        // Ẩn interaction indicator
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }

        // Hủy coroutine start dialogue nếu đang chạy
        if (startDialogueCoroutine != null)
        {
            StopCoroutine(startDialogueCoroutine);
            startDialogueCoroutine = null;
        }

        // Tự động đóng dialogue nếu được thiết lập
        if (autoCloseOnExit && isDialogActive)
        {
            EndDialog();
        }
    }

    /// <summary>
    /// Coroutine để bắt đầu dialogue sau delay
    /// </summary>
    private IEnumerator StartDialogueWithDelay()
    {
        yield return new WaitForSeconds(startDelay);
        
        // Kiểm tra player vẫn trong vùng trigger
        if (isPlayerInRange && (!triggerOnce || !hasTriggered))
        {
            StartDialog();
        }
        
        startDialogueCoroutine = null;
    }

    /// <summary>
    /// Bắt đầu dialogue (có thể gọi từ bên ngoài)
    /// </summary>
    public void StartDialog()
    {
        if (DialogData == null)
        {
            Debug.LogWarning($"NPCDialogueTrigger: No DialogData assigned on {gameObject.name}");
            return;
        }

        if (isDialogActive) return;

        if (triggerOnce && hasTriggered) return;

        isDialogActive = true;
        hasTriggered = true;
        dialogIndex = 0;

        // Setup NPC info
        nameText.SetText(DialogData.npcName);
        portraitImage.sprite = DialogData.npcPortrait;

        // Hiển thị dialogue panel
        dialogPanel.SetActive(true);

        // Bắt đầu typing effect
        StartCoroutine(TypeLine());

        Debug.Log($"Started dialogue with {DialogData.npcName}");
    }

    /// <summary>
    /// Chuyển sang dòng dialogue tiếp theo
    /// </summary>
    public void NextLine()
    {
        if (!isDialogActive) return;

        if (isTyping)
        {
            // Skip typing animation
            StopAllCoroutines();
            dialogText.SetText(DialogData.dialogLines[dialogIndex]);
            isTyping = false;
        }
        else if (++dialogIndex < DialogData.dialogLines.Length)
        {
            // Move to next line
            StartCoroutine(TypeLine());
        }
        else
        {
            // End of dialogue
            EndDialog();
        }
    }

    /// <summary>
    /// Coroutine typing effect
    /// </summary>
    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogText.SetText("");

        foreach (char letter in DialogData.dialogLines[dialogIndex])
        {
            dialogText.text += letter;
            
            // Play voice sound if available
            if (DialogData.voiceSound != null)
            {
                SoundEffectManager.PlayVoice(DialogData.voiceSound, DialogData.voicePitch);
            }
            
            yield return new WaitForSeconds(DialogData.typingSpeed);
        }

        isTyping = false;

        // Auto progress if enabled for this line
        if (DialogData.autoProgressLines.Length > dialogIndex && 
            DialogData.autoProgressLines[dialogIndex])
        {
            yield return new WaitForSeconds(DialogData.autoProgressDelay);
            NextLine();
        }
    }

    /// <summary>
    /// Kết thúc dialogue
    /// </summary>
    public void EndDialog()
    {
        StopAllCoroutines();
        isDialogActive = false;
        dialogText.SetText("");
        dialogPanel.SetActive(false);

        Debug.Log($"Ended dialogue with {DialogData.npcName}");

        // Phá hủy target GameObject nếu có
        if (targetToDestroy != null)
        {
            Destroy(targetToDestroy);
            Debug.Log($"Destroyed target object: {targetToDestroy.name}");
        }

        // Tự phá hủy nếu được thiết lập
        if (destroySelfAfterDialogue)
        {
            Debug.Log($"Self-destroying NPC: {gameObject.name}");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when close button is clicked
    /// </summary>
    private void OnCloseButtonClicked()
    {
        if (isDialogActive)
        {
            EndDialog();
        }
    }

    /// <summary>
    /// Force start dialogue (bỏ qua trigger once và delay)
    /// </summary>
    public void ForceStartDialog()
    {
        if (DialogData == null) return;

        // Stop any running coroutines
        StopAllCoroutines();
        
        if (startDialogueCoroutine != null)
        {
            StopCoroutine(startDialogueCoroutine);
            startDialogueCoroutine = null;
        }

        StartDialog();
    }

    /// <summary>
    /// Force end dialogue
    /// </summary>
    public void ForceEndDialog()
    {
        if (isDialogActive)
        {
            EndDialog();
        }
    }

    /// <summary>
    /// Reset trigger để có thể trigger lại
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        Debug.Log($"Reset dialogue trigger for {gameObject.name}");
    }

    /// <summary>
    /// Kiểm tra player có đang trong vùng trigger không
    /// </summary>
    public bool IsPlayerInRange()
    {
        return isPlayerInRange;
    }

    /// <summary>
    /// Kiểm tra dialogue có đang active không
    /// </summary>
    public bool IsDialogActive()
    {
        return isDialogActive;
    }

    /// <summary>
    /// Set dialogue data mới
    /// </summary>
    public void SetDialogData(NPCDialog newDialogData)
    {
        DialogData = newDialogData;
    }

    private void OnDisable()
    {
        // Cleanup khi object bị disable
        if (startDialogueCoroutine != null)
        {
            StopCoroutine(startDialogueCoroutine);
            startDialogueCoroutine = null;
        }

        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Cleanup close button listener
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }
}