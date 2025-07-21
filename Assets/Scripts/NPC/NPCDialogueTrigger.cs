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
    [Tooltip("T? d?ng b?t d?u dialogue khi Player vào trigger")]
    public bool autoStartOnEnter = true;
    [Tooltip("T? d?ng dóng dialogue khi Player r?i trigger")]
    public bool autoCloseOnExit = true;
    [Tooltip("Ch? trigger dialogue m?t l?n")]
    public bool triggerOnce = false;
    [Tooltip("Delay tru?c khi b?t d?u dialogue (giây)")]
    public float startDelay = 0.5f;
    [Header("Close Button Integration")]
    public Button closeButton;
    [Header("Destruction Settings")]
    [Tooltip("GameObject s? b? phá h?y khi dialogue k?t thúc")]
    public GameObject targetToDestroy;
    [Tooltip("T? phá h?y NPC này sau khi dialogue hoàn thành")]
    public bool destroySelfAfterDialogue = false;
    [Header("Visual Indicator")]
    [Tooltip("Hi?n th? indicator khi player có th? tuong tác")]
    public GameObject interactionIndicator;
    private int dialogIndex;
    private bool isTyping, isDialogActive, hasTriggered;
    private bool isPlayerInRange = false;
    private Coroutine startDialogueCoroutine;
    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }
    }
    private void Update()
    {
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
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(true);
        }
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
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }
        if (startDialogueCoroutine != null)
        {
            StopCoroutine(startDialogueCoroutine);
            startDialogueCoroutine = null;
        }
        if (autoCloseOnExit && isDialogActive)
        {
            EndDialog();
        }
    }
    private IEnumerator StartDialogueWithDelay()
    {
        yield return new WaitForSeconds(startDelay);
        if (isPlayerInRange && (!triggerOnce || !hasTriggered))
        {
            StartDialog();
        }
        startDialogueCoroutine = null;
    }
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
        nameText.SetText(DialogData.npcName);
        portraitImage.sprite = DialogData.npcPortrait;
        dialogPanel.SetActive(true);
        StartCoroutine(TypeLine());
        Debug.Log($"Started dialogue with {DialogData.npcName}");
    }
    public void NextLine()
    {
        if (!isDialogActive) return;
        if (isTyping)
        {
            StopAllCoroutines();
            dialogText.SetText(DialogData.dialogLines[dialogIndex]);
            isTyping = false;
        }
        else if (++dialogIndex < DialogData.dialogLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialog();
        }
    }
    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogText.SetText("");
        foreach (char letter in DialogData.dialogLines[dialogIndex])
        {
            dialogText.text += letter;
            if (DialogData.voiceSound != null)
            {
                SoundEffectManager.PlayVoice(DialogData.voiceSound, DialogData.voicePitch);
            }
            yield return new WaitForSeconds(DialogData.typingSpeed);
        }
        isTyping = false;
        if (DialogData.autoProgressLines.Length > dialogIndex &&
            DialogData.autoProgressLines[dialogIndex])
        {
            yield return new WaitForSeconds(DialogData.autoProgressDelay);
            NextLine();
        }
    }
    public void EndDialog()
    {
        StopAllCoroutines();
        isDialogActive = false;
        dialogText.SetText("");
        dialogPanel.SetActive(false);
        Debug.Log($"Ended dialogue with {DialogData.npcName}");
        if (targetToDestroy != null)
        {
            Destroy(targetToDestroy);
            Debug.Log($"Destroyed target object: {targetToDestroy.name}");
        }
        if (destroySelfAfterDialogue)
        {
            Debug.Log($"Self-destroying NPC: {gameObject.name}");
            Destroy(gameObject);
        }
    }
    private void OnCloseButtonClicked()
    {
        if (isDialogActive)
        {
            EndDialog();
        }
    }
    public void ForceStartDialog()
    {
        if (DialogData == null) return;
        StopAllCoroutines();
        if (startDialogueCoroutine != null)
        {
            StopCoroutine(startDialogueCoroutine);
            startDialogueCoroutine = null;
        }
        StartDialog();
    }
    public void ForceEndDialog()
    {
        if (isDialogActive)
        {
            EndDialog();
        }
    }
    public void ResetTrigger()
    {
        hasTriggered = false;
        Debug.Log($"Reset dialogue trigger for {gameObject.name}");
    }
    public bool IsPlayerInRange()
    {
        return isPlayerInRange;
    }
    public bool IsDialogActive()
    {
        return isDialogActive;
    }
    public void SetDialogData(NPCDialog newDialogData)
    {
        DialogData = newDialogData;
    }
    private void OnDisable()
    {
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
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }
}