using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
public class npc1 : MonoBehaviour, IInteractable
{
    public NPCDialog DialogData;
    public GameObject dialogPanel;
    public TMPro.TMP_Text dialogText, nameText;
    public Image portraitImage;
    public GameObject targetToDestroy;
    [Header("NPC Indicators")]
    public GameObject exclamationMark;
    [Header("Boss Indicator")]
    [Tooltip("GameObject c?a Boss NPC")]
    public GameObject bossNPC;
    [Tooltip("D?u ch?m than c?a Boss")]
    public GameObject bossExclamationMark;
    [Header("Boss Quest Image")]
    [Tooltip("Panel ch?a hình ?nh hi?n th? sau khi nói chuy?n v?i Boss")]
    public GameObject imagePanel;
    [Tooltip("Hình ?nh hi?n th? sau khi nói chuy?n v?i Boss")]
    public Image questImage;
    [Tooltip("Sprite s? hi?n th? sau khi nói chuy?n v?i Boss")]
    public Sprite questSprite;
    [Tooltip("Th?i gian hi?n th? hình ?nh (giây)")]
    public float imageDisplayTime = 5f;
    [Header("Player Thought Panel")]
    [Tooltip("Panel ch?a suy nghi c?a nhân v?t chính")]
    public GameObject thoughtPanel;
    [Tooltip("Text hi?n th? suy nghi c?a nhân v?t chính")]
    public TextMeshProUGUI thoughtText;
    [Tooltip("Th?i gian hi?n th? suy nghi (giây, 0 d? hi?n th? vinh vi?n)")]
    public float thoughtDisplayTime = 7f;
    [Tooltip("Suy nghi c?a nhân v?t sau khi nói chuy?n v?i Boss")]
    [TextArea(3, 5)]
    public string bossThoughtText = "Th?t là b?c mình! S?p l?i giao thêm vi?c. Tôi nên ngh? vi?c du?c r?i!";
    [Tooltip("V? trí hi?n th? panel suy nghi (theo t?a d? màn hình)")]
    public Vector2 thoughtPanelPosition = new Vector2(0, 250);
    [Header("Close Button Integration")]
    public Button closeButton;
    [Tooltip("Nút dóng panel hình ?nh")]
    public Button closeImageButton;
    [Tooltip("Nút dóng panel suy nghi")]
    public Button closeThoughtButton;
    private int dialogIndex;
    private bool isTyping, isDialogActive;
    private Coroutine imageDisplayCoroutine;
    private Coroutine thoughtDisplayCoroutine;
    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        if (closeImageButton != null)
        {
            closeImageButton.onClick.AddListener(CloseImagePanel);
        }
        if (closeThoughtButton != null)
        {
            closeThoughtButton.onClick.AddListener(CloseThoughtPanel);
        }
        if (bossExclamationMark != null && !DialogManager.Instance.hasTalkedToBob)
        {
            bossExclamationMark.SetActive(false);
        }
        if (imagePanel != null)
        {
            imagePanel.SetActive(false);
        }
        if (thoughtPanel != null)
        {
            thoughtPanel.SetActive(false);
            RectTransform rectTransform = thoughtPanel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = thoughtPanelPosition;
            }
        }
    }
    private void OnCloseButtonClicked()
    {
        if (isDialogActive)
        {
            EndDialog();
        }
    }
    public bool CanInteract()
    {
        if (DialogData.npcName == "Boss" && !DialogManager.Instance.hasTalkedToBob)
        {
            Debug.Log("You need to talk to NPC Bob first!");
            return false;
        }
        return !isDialogActive;
    }
    public void Interact()
    {
        if (DialogData == null)
        {
            return;
        }
        if (isDialogActive)
        {
            NextLine();
        }
        else
        {
            StartDialog();
        }
    }
    void StartDialog()
    {
        isDialogActive = true;
        dialogIndex = 0;
        nameText.SetText(DialogData.npcName);
        portraitImage.sprite = DialogData.npcPortrait;
        dialogPanel.SetActive(true);
        StartCoroutine(TypeLine());
    }
    void NextLine()
    {
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
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogText.SetText("");
        foreach (char letter in DialogData.dialogLines[dialogIndex])
        {
            dialogText.text += letter;
            SoundEffectManager.PlayVoice(DialogData.voiceSound, DialogData.voicePitch);
            yield return new WaitForSeconds(DialogData.typingSpeed);
        }
        isTyping = false;
        if (DialogData.autoProgressLines.Length > dialogIndex && DialogData.autoProgressLines[dialogIndex])
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
        if (DialogData.npcName == "Bob")
        {
            DialogManager.Instance.hasTalkedToBob = true;
            if (bossExclamationMark != null)
            {
                bossExclamationMark.SetActive(true);
            }
        }
        if (DialogData.npcName == "Boss")
        {
            ShowQuestImage();
            ShowPlayerThought();
        }
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
        }
        if (targetToDestroy != null)
        {
            Destroy(targetToDestroy);
        }
    }
    private void ShowQuestImage()
    {
        if (imagePanel == null)
        {
            Debug.LogWarning("Image Panel is not assigned!");
            return;
        }
        if (imageDisplayCoroutine != null)
        {
            StopCoroutine(imageDisplayCoroutine);
        }
        if (questImage != null && questSprite != null)
        {
            questImage.sprite = questSprite;
        }
        imagePanel.SetActive(true);
        if (imageDisplayTime > 0)
        {
            imageDisplayCoroutine = StartCoroutine(HideImageAfterDelay());
        }
    }
    private void ShowPlayerThought()
    {
        if (thoughtPanel == null)
        {
            Debug.LogWarning("Thought Panel is not assigned!");
            return;
        }
        if (thoughtDisplayCoroutine != null)
        {
            StopCoroutine(thoughtDisplayCoroutine);
        }
        if (thoughtText != null)
        {
            thoughtText.text = bossThoughtText;
        }
        thoughtPanel.SetActive(true);
        StartCoroutine(FadeInThoughtPanel());
        if (thoughtDisplayTime > 0)
        {
            thoughtDisplayCoroutine = StartCoroutine(HideThoughtAfterDelay());
        }
    }
    private IEnumerator FadeInThoughtPanel()
    {
        CanvasGroup canvasGroup = thoughtPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = thoughtPanel.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        float duration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
    private IEnumerator HideImageAfterDelay()
    {
        yield return new WaitForSeconds(imageDisplayTime);
        CloseImagePanel();
    }
    private IEnumerator HideThoughtAfterDelay()
    {
        yield return new WaitForSeconds(thoughtDisplayTime);
        CloseThoughtPanel();
    }
    public void CloseImagePanel()
    {
        if (imagePanel != null)
        {
            imagePanel.SetActive(false);
        }
        imageDisplayCoroutine = null;
    }
    public void CloseThoughtPanel()
    {
        StartCoroutine(FadeOutThoughtPanel());
    }
    private IEnumerator FadeOutThoughtPanel()
    {
        CanvasGroup canvasGroup = thoughtPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = thoughtPanel.AddComponent<CanvasGroup>();
        }
        float startAlpha = canvasGroup.alpha;
        float duration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        thoughtPanel.SetActive(false);
        thoughtDisplayCoroutine = null;
    }
    public void StopInteract()
    {
        if (isDialogActive)
        {
            EndDialog();
        }
    }
}