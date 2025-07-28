//////using UnityEngine;
//////using System.Collections;
//////using System.Collections.Generic;
//////using TMPro;
//////using UnityEngine.UI;

//////public class npc1 : MonoBehaviour, IInteractable
//////{
//////    public NPCDialog DialogData;
//////    public GameObject dialogPanel;
//////    public TMPro.TMP_Text dialogText, nameText;
//////    public Image portraitImage;
//////    public GameObject targetToDestroy; // GameObject sẽ bị phá hủy khi hội thoại kết thúc

//////    [Header("Close Button Integration")]
//////    public Button closeButton; // Assign the close button in inspector

//////    private int dialogIndex;
//////    private bool isTyping, isDialogActive;

//////    private void Start()
//////    {
//////        // Setup close button if assigned
//////        if (closeButton != null)
//////        {
//////            closeButton.onClick.AddListener(OnCloseButtonClicked);
//////        }
//////    }

//////    // Called when close button is clicked
//////    private void OnCloseButtonClicked()
//////    {
//////        if (isDialogActive)
//////        {
//////            EndDialog();
//////        }
//////    }

//////    public bool CanInteract()
//////    {
//////        if (DialogData.npcName == "Boss" && !DialogManager.Instance.hasTalkedToBob)
//////        {
//////            // Có thể thêm phản hồi cho người chơi: "Bạn cần nói chuyện với NPC Bob trước"
//////            Debug.Log("You need to talk to NPC Bob first!");
//////            return false;
//////        }

//////        return !isDialogActive;
//////    }

//////    public void Interact()
//////    {
//////        if (DialogData == null)
//////        {
//////            return;
//////        }
//////        if (isDialogActive)
//////        {
//////            NextLine();
//////        }
//////        else
//////        {
//////            StartDialog();
//////        }
//////    }

//////    void StartDialog()
//////    {
//////        isDialogActive = true;
//////        dialogIndex = 0;

//////        nameText.SetText(DialogData.npcName);
//////        portraitImage.sprite = DialogData.npcPortrait;

//////        dialogPanel.SetActive(true);

//////        StartCoroutine(TypeLine());
//////    }

//////    void NextLine()
//////    {
//////        if (isTyping)
//////        {
//////            StopAllCoroutines();
//////            dialogText.SetText(DialogData.dialogLines[dialogIndex]);
//////            isTyping = false;
//////        }
//////        else if(++dialogIndex < DialogData.dialogLines.Length)
//////        {
//////            StartCoroutine(TypeLine());
//////        }
//////        else
//////        {
//////            EndDialog();
//////        }
//////    }

//////    IEnumerator TypeLine()
//////    {
//////        isTyping = true;
//////        dialogText.SetText("");

//////        foreach (char letter in DialogData.dialogLines[dialogIndex])
//////        {
//////            dialogText.text += letter;
//////            SoundEffectManager.PlayVoice(DialogData.voiceSound, DialogData.voicePitch);
//////            yield return new WaitForSeconds(DialogData.typingSpeed);
//////        }

//////        isTyping = false;

//////        if (DialogData.autoProgressLines.Length > dialogIndex && DialogData.autoProgressLines[dialogIndex])
//////        {
//////            yield return new WaitForSeconds(DialogData.autoProgressDelay);
//////            NextLine();
//////        } 
//////    }

//////    public void EndDialog()
//////    {
//////        StopAllCoroutines();
//////        isDialogActive = false;
//////        dialogText.SetText("");
//////        dialogPanel.SetActive(false);

//////        //Đánh dấu đã nói chuyện với NPC Bob
//////        if (DialogData.npcName == "Bob")  // Tên phải khớp với phần Inspector
//////        {
//////            DialogManager.Instance.hasTalkedToBob = true;
//////        }


//////        // Phá hủy target GameObject nếu nó tồn tại
//////        if (targetToDestroy != null)
//////        {
//////            Destroy(targetToDestroy);
//////        }
//////    }

//////    public void StopInteract()
//////    {
//////        // Implement StopInteract to handle when player exits interaction area
//////        if (isDialogActive)
//////        {
//////            EndDialog();
//////        }
//////    }
//////}

////using UnityEngine;
////using System.Collections;
////using System.Collections.Generic;
////using TMPro;
////using UnityEngine.UI;

////public class npc1 : MonoBehaviour, IInteractable
////{
////    public NPCDialog DialogData;
////    public GameObject dialogPanel;
////    public TMPro.TMP_Text dialogText, nameText;
////    public Image portraitImage;
////    public GameObject targetToDestroy; // GameObject sẽ bị phá hủy khi hội thoại kết thúc

////    [Header("NPC Indicators")]
////    public GameObject exclamationMark; // Dấu chấm than "!" trên đầu NPC

////    [Header("Boss Indicator")]
////    [Tooltip("GameObject của Boss NPC")]
////    public GameObject bossNPC; // Tham chiếu đến GameObject của Boss
////    [Tooltip("Dấu chấm than của Boss")]
////    public GameObject bossExclamationMark; // Dấu chấm than của Boss

////    [Header("Close Button Integration")]
////    public Button closeButton; // Assign the close button in inspector

////    private int dialogIndex;
////    private bool isTyping, isDialogActive;

////    private void Start()
////    {
////        // Setup close button if assigned
////        if (closeButton != null)
////        {
////            closeButton.onClick.AddListener(OnCloseButtonClicked);
////        }

////        // Ẩn dấu chấm than của Boss nếu chưa nói chuyện với Bob
////        if (bossExclamationMark != null && !DialogManager.Instance.hasTalkedToBob)
////        {
////            bossExclamationMark.SetActive(false);
////        }
////    }

////    // Called when close button is clicked
////    private void OnCloseButtonClicked()
////    {
////        if (isDialogActive)
////        {
////            EndDialog();
////        }
////    }

////    public bool CanInteract()
////    {
////        if (DialogData.npcName == "Boss" && !DialogManager.Instance.hasTalkedToBob)
////        {
////            // Có thể thêm phản hồi cho người chơi: "Bạn cần nói chuyện với NPC Bob trước"
////            Debug.Log("You need to talk to NPC Bob first!");
////            return false;
////        }

////        return !isDialogActive;
////    }

////    public void Interact()
////    {
////        if (DialogData == null)
////        {
////            return;
////        }
////        if (isDialogActive)
////        {
////            NextLine();
////        }
////        else
////        {
////            StartDialog();
////        }
////    }

////    void StartDialog()
////    {
////        isDialogActive = true;
////        dialogIndex = 0;

////        nameText.SetText(DialogData.npcName);
////        portraitImage.sprite = DialogData.npcPortrait;

////        dialogPanel.SetActive(true);

////        StartCoroutine(TypeLine());
////    }

////    void NextLine()
////    {
////        if (isTyping)
////        {
////            StopAllCoroutines();
////            dialogText.SetText(DialogData.dialogLines[dialogIndex]);
////            isTyping = false;
////        }
////        else if (++dialogIndex < DialogData.dialogLines.Length)
////        {
////            StartCoroutine(TypeLine());
////        }
////        else
////        {
////            EndDialog();
////        }
////    }

////    IEnumerator TypeLine()
////    {
////        isTyping = true;
////        dialogText.SetText("");

////        foreach (char letter in DialogData.dialogLines[dialogIndex])
////        {
////            dialogText.text += letter;
////            SoundEffectManager.PlayVoice(DialogData.voiceSound, DialogData.voicePitch);
////            yield return new WaitForSeconds(DialogData.typingSpeed);
////        }

////        isTyping = false;

////        if (DialogData.autoProgressLines.Length > dialogIndex && DialogData.autoProgressLines[dialogIndex])
////        {
////            yield return new WaitForSeconds(DialogData.autoProgressDelay);
////            NextLine();
////        }
////    }

////    public void EndDialog()
////    {
////        StopAllCoroutines();
////        isDialogActive = false;
////        dialogText.SetText("");
////        dialogPanel.SetActive(false);

////        // Đánh dấu đã nói chuyện với NPC Bob và hiển thị dấu chấm than của Boss
////        if (DialogData.npcName == "Bob")  // Tên phải khớp với phần Inspector
////        {
////            DialogManager.Instance.hasTalkedToBob = true;

////            // Hiển thị dấu chấm than cho Boss
////            if (bossExclamationMark != null)
////            {
////                bossExclamationMark.SetActive(true);
////            }
////        }

////        // Ẩn dấu chấm than "!" của NPC hiện tại nếu nó tồn tại
////        if (exclamationMark != null)
////        {
////            exclamationMark.SetActive(false);
////        }

////        // Phá hủy target GameObject nếu nó tồn tại
////        if (targetToDestroy != null)
////        {
////            Destroy(targetToDestroy);
////        }
////    }

////    public void StopInteract()
////    {
////        // Implement StopInteract to handle when player exits interaction area
////        if (isDialogActive)
////        {
////            EndDialog();
////        }
////    }
////}

//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine.UI;

//public class npc1 : MonoBehaviour, IInteractable
//{
//    public NPCDialog DialogData;
//    public GameObject dialogPanel;
//    public TMPro.TMP_Text dialogText, nameText;
//    public Image portraitImage;
//    public GameObject targetToDestroy; // GameObject sẽ bị phá hủy khi hội thoại kết thúc

//    [Header("NPC Indicators")]
//    public GameObject exclamationMark; // Dấu chấm than "!" trên đầu NPC

//    [Header("Boss Indicator")]
//    [Tooltip("GameObject của Boss NPC")]
//    public GameObject bossNPC; // Tham chiếu đến GameObject của Boss
//    [Tooltip("Dấu chấm than của Boss")]
//    public GameObject bossExclamationMark; // Dấu chấm than của Boss

//    [Header("Boss Quest Image")]
//    [Tooltip("Panel chứa hình ảnh hiển thị sau khi nói chuyện với Boss")]
//    public GameObject imagePanel; // Panel chứa hình ảnh
//    [Tooltip("Hình ảnh hiển thị sau khi nói chuyện với Boss")]
//    public Image questImage; // Hình ảnh hiển thị
//    [Tooltip("Sprite sẽ hiển thị sau khi nói chuyện với Boss")]
//    public Sprite questSprite; // Sprite hiển thị
//    [Tooltip("Thời gian hiển thị hình ảnh (giây)")]
//    public float imageDisplayTime = 5f; // Thời gian hiển thị hình ảnh

//    [Header("Close Button Integration")]
//    public Button closeButton; // Assign the close button in inspector
//    [Tooltip("Nút đóng panel hình ảnh")]
//    public Button closeImageButton; // Nút đóng panel hình ảnh

//    private int dialogIndex;
//    private bool isTyping, isDialogActive;
//    private Coroutine imageDisplayCoroutine;

//    private void Start()
//    {
//        // Setup close button if assigned
//        if (closeButton != null)
//        {
//            closeButton.onClick.AddListener(OnCloseButtonClicked);
//        }

//        // Setup close image button if assigned
//        if (closeImageButton != null)
//        {
//            closeImageButton.onClick.AddListener(CloseImagePanel);
//        }

//        // Ẩn dấu chấm than của Boss nếu chưa nói chuyện với Bob
//        if (bossExclamationMark != null && !DialogManager.Instance.hasTalkedToBob)
//        {
//            bossExclamationMark.SetActive(false);
//        }

//        // Đảm bảo panel hình ảnh bị ẩn khi bắt đầu
//        if (imagePanel != null)
//        {
//            imagePanel.SetActive(false);
//        }
//    }

//    // Called when close button is clicked
//    private void OnCloseButtonClicked()
//    {
//        if (isDialogActive)
//        {
//            EndDialog();
//        }
//    }

//    public bool CanInteract()
//    {
//        if (DialogData.npcName == "Boss" && !DialogManager.Instance.hasTalkedToBob)
//        {
//            // Có thể thêm phản hồi cho người chơi: "Bạn cần nói chuyện với NPC Bob trước"
//            Debug.Log("You need to talk to NPC Bob first!");
//            return false;
//        }

//        return !isDialogActive;
//    }

//    public void Interact()
//    {
//        if (DialogData == null)
//        {
//            return;
//        }
//        if (isDialogActive)
//        {
//            NextLine();
//        }
//        else
//        {
//            StartDialog();
//        }
//    }

//    void StartDialog()
//    {
//        isDialogActive = true;
//        dialogIndex = 0;

//        nameText.SetText(DialogData.npcName);
//        portraitImage.sprite = DialogData.npcPortrait;

//        dialogPanel.SetActive(true);

//        StartCoroutine(TypeLine());
//    }

//    void NextLine()
//    {
//        if (isTyping)
//        {
//            StopAllCoroutines();
//            dialogText.SetText(DialogData.dialogLines[dialogIndex]);
//            isTyping = false;
//        }
//        else if (++dialogIndex < DialogData.dialogLines.Length)
//        {
//            StartCoroutine(TypeLine());
//        }
//        else
//        {
//            EndDialog();
//        }
//    }

//    IEnumerator TypeLine()
//    {
//        isTyping = true;
//        dialogText.SetText("");

//        foreach (char letter in DialogData.dialogLines[dialogIndex])
//        {
//            dialogText.text += letter;
//            SoundEffectManager.PlayVoice(DialogData.voiceSound, DialogData.voicePitch);
//            yield return new WaitForSeconds(DialogData.typingSpeed);
//        }

//        isTyping = false;

//        if (DialogData.autoProgressLines.Length > dialogIndex && DialogData.autoProgressLines[dialogIndex])
//        {
//            yield return new WaitForSeconds(DialogData.autoProgressDelay);
//            NextLine();
//        }
//    }

//    public void EndDialog()
//    {
//        StopAllCoroutines();
//        isDialogActive = false;
//        dialogText.SetText("");
//        dialogPanel.SetActive(false);

//        // Đánh dấu đã nói chuyện với NPC Bob và hiển thị dấu chấm than của Boss
//        if (DialogData.npcName == "Bob")  // Tên phải khớp với phần Inspector
//        {
//            DialogManager.Instance.hasTalkedToBob = true;

//            // Hiển thị dấu chấm than cho Boss
//            if (bossExclamationMark != null)
//            {
//                bossExclamationMark.SetActive(true);
//            }
//        }

//        // Hiển thị hình ảnh sau khi nói chuyện với Boss
//        if (DialogData.npcName == "Boss")
//        {
//            ShowQuestImage();
//        }

//        // Ẩn dấu chấm than "!" của NPC hiện tại nếu nó tồn tại
//        if (exclamationMark != null)
//        {
//            exclamationMark.SetActive(false);
//        }

//        // Phá hủy target GameObject nếu nó tồn tại
//        if (targetToDestroy != null)
//        {
//            Destroy(targetToDestroy);
//        }
//    }

//    // Hiển thị hình ảnh sau khi nói chuyện với Boss
//    private void ShowQuestImage()
//    {
//        // Kiểm tra xem có panel hình ảnh không
//        if (imagePanel == null)
//        {
//            Debug.LogWarning("Image Panel is not assigned!");
//            return;
//        }

//        // Nếu đã có coroutine đang chạy, dừng lại
//        if (imageDisplayCoroutine != null)
//        {
//            StopCoroutine(imageDisplayCoroutine);
//        }

//        // Nếu có hình ảnh và sprite, thiết lập sprite cho hình ảnh
//        if (questImage != null && questSprite != null)
//        {
//            questImage.sprite = questSprite;
//        }

//        // Hiển thị panel hình ảnh
//        imagePanel.SetActive(true);

//        // Nếu có thời gian hiển thị > 0, tự động ẩn sau khoảng thời gian đó
//        if (imageDisplayTime > 0)
//        {
//            imageDisplayCoroutine = StartCoroutine(HideImageAfterDelay());
//        }
//    }

//    // Tự động ẩn hình ảnh sau một khoảng thời gian
//    private IEnumerator HideImageAfterDelay()
//    {
//        yield return new WaitForSeconds(imageDisplayTime);
//        CloseImagePanel();
//    }

//    // Đóng panel hình ảnh
//    public void CloseImagePanel()
//    {
//        if (imagePanel != null)
//        {
//            imagePanel.SetActive(false);
//        }

//        // Reset coroutine
//        imageDisplayCoroutine = null;
//    }

//    public void StopInteract()
//    {
//        // Implement StopInteract to handle when player exits interaction area
//        if (isDialogActive)
//        {
//            EndDialog();
//        }
//    }
//}

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
    public GameObject targetToDestroy; // GameObject sẽ bị phá hủy khi hội thoại kết thúc

    [Header("NPC Indicators")]
    public GameObject exclamationMark; // Dấu chấm than "!" trên đầu NPC

    [Header("Boss Indicator")]
    [Tooltip("GameObject của Boss NPC")]
    public GameObject bossNPC; // Tham chiếu đến GameObject của Boss
    [Tooltip("Dấu chấm than của Boss")]
    public GameObject bossExclamationMark; // Dấu chấm than của Boss

    [Header("Boss Quest Image")]
    [Tooltip("Panel chứa hình ảnh hiển thị sau khi nói chuyện với Boss")]
    public GameObject imagePanel; // Panel chứa hình ảnh
    [Tooltip("Hình ảnh hiển thị sau khi nói chuyện với Boss")]
    public Image questImage; // Hình ảnh hiển thị
    [Tooltip("Sprite sẽ hiển thị sau khi nói chuyện với Boss")]
    public Sprite questSprite; // Sprite hiển thị
    [Tooltip("Thời gian hiển thị hình ảnh (giây)")]
    public float imageDisplayTime = 5f; // Thời gian hiển thị hình ảnh

    [Header("Player Thought Panel")]
    [Tooltip("Panel chứa suy nghĩ của nhân vật chính")]
    public GameObject thoughtPanel; // Panel chứa suy nghĩ
    [Tooltip("Text hiển thị suy nghĩ của nhân vật chính")]
    public TextMeshProUGUI thoughtText; // Text hiển thị suy nghĩ
    [Tooltip("Thời gian hiển thị suy nghĩ (giây, 0 để hiển thị vĩnh viễn)")]
    public float thoughtDisplayTime = 7f; // Thời gian hiển thị suy nghĩ
    [Tooltip("Suy nghĩ của nhân vật sau khi nói chuyện với Boss")]
    [TextArea(3, 5)]
    public string bossThoughtText = "Thật là bực mình! Sếp lại giao thêm việc. Tôi nên nghỉ việc được rồi!"; // Suy nghĩ mặc định
    [Tooltip("Vị trí hiển thị panel suy nghĩ (theo tọa độ màn hình)")]
    public Vector2 thoughtPanelPosition = new Vector2(0, 250); // Vị trí hiển thị panel

    [Header("Close Button Integration")]
    public Button closeButton; // Assign the close button in inspector
    [Tooltip("Nút đóng panel hình ảnh")]
    public Button closeImageButton; // Nút đóng panel hình ảnh
    [Tooltip("Nút đóng panel suy nghĩ")]
    public Button closeThoughtButton; // Nút đóng panel suy nghĩ

    private int dialogIndex;
    private bool isTyping, isDialogActive;
    private Coroutine imageDisplayCoroutine;
    private Coroutine thoughtDisplayCoroutine;

    private void Start()
    {
        // Setup close button if assigned
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        // Setup close image button if assigned
        if (closeImageButton != null)
        {
            closeImageButton.onClick.AddListener(CloseImagePanel);
        }

        // Setup close thought button if assigned
        if (closeThoughtButton != null)
        {
            closeThoughtButton.onClick.AddListener(CloseThoughtPanel);
        }

        // Ẩn dấu chấm than của Boss nếu chưa nói chuyện với Bob
        if (bossExclamationMark != null && !DialogManager.Instance.hasTalkedToBob)
        {
            bossExclamationMark.SetActive(false);
        }

        // Đảm bảo panel hình ảnh bị ẩn khi bắt đầu
        if (imagePanel != null)
        {
            imagePanel.SetActive(false);
        }

        // Đảm bảo panel suy nghĩ bị ẩn khi bắt đầu
        if (thoughtPanel != null)
        {
            thoughtPanel.SetActive(false);

            // Đặt vị trí cho panel suy nghĩ nếu đã chỉ định
            RectTransform rectTransform = thoughtPanel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = thoughtPanelPosition;
            }
        }
    }

    // Called when close button is clicked
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
            // Có thể thêm phản hồi cho người chơi: "Bạn cần nói chuyện với NPC Bob trước"
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

        // Đánh dấu đã nói chuyện với NPC Bob và hiển thị dấu chấm than của Boss
        if (DialogData.npcName == "Bob")  // Tên phải khớp với phần Inspector
        {
            DialogManager.Instance.hasTalkedToBob = true;

            // Hiển thị dấu chấm than cho Boss
            if (bossExclamationMark != null)
            {
                bossExclamationMark.SetActive(true);
            }
        }

        // Hiển thị hình ảnh và suy nghĩ sau khi nói chuyện với Boss
        if (DialogData.npcName == "Boss")
        {
            ShowQuestImage();
            ShowPlayerThought();
        }

        // Ẩn dấu chấm than "!" của NPC hiện tại nếu nó tồn tại
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
        }

        // Phá hủy target GameObject nếu nó tồn tại
        if (targetToDestroy != null)
        {
            Destroy(targetToDestroy);
        }
    }

    // Hiển thị hình ảnh sau khi nói chuyện với Boss
    private void ShowQuestImage()
    {
        // Kiểm tra xem có panel hình ảnh không
        if (imagePanel == null)
        {
            Debug.LogWarning("Image Panel is not assigned!");
            return;
        }

        // Nếu đã có coroutine đang chạy, dừng lại
        if (imageDisplayCoroutine != null)
        {
            StopCoroutine(imageDisplayCoroutine);
        }

        // Nếu có hình ảnh và sprite, thiết lập sprite cho hình ảnh
        if (questImage != null && questSprite != null)
        {
            questImage.sprite = questSprite;
        }

        // Hiển thị panel hình ảnh
        imagePanel.SetActive(true);

        // Nếu có thời gian hiển thị > 0, tự động ẩn sau khoảng thời gian đó
        if (imageDisplayTime > 0)
        {
            imageDisplayCoroutine = StartCoroutine(HideImageAfterDelay());
        }
    }

    // Hiển thị suy nghĩ của nhân vật chính sau khi nói chuyện với Boss
    private void ShowPlayerThought()
    {
        // Kiểm tra xem có panel suy nghĩ không
        if (thoughtPanel == null)
        {
            Debug.LogWarning("Thought Panel is not assigned!");
            return;
        }

        // Nếu đã có coroutine đang chạy, dừng lại
        if (thoughtDisplayCoroutine != null)
        {
            StopCoroutine(thoughtDisplayCoroutine);
        }

        // Nếu có text suy nghĩ, thiết lập nội dung
        if (thoughtText != null)
        {
            thoughtText.text = bossThoughtText;
        }

        // Hiển thị panel suy nghĩ với hiệu ứng fade in
        thoughtPanel.SetActive(true);
        StartCoroutine(FadeInThoughtPanel());

        // Nếu có thời gian hiển thị > 0, tự động ẩn sau khoảng thời gian đó
        if (thoughtDisplayTime > 0)
        {
            thoughtDisplayCoroutine = StartCoroutine(HideThoughtAfterDelay());
        }
    }

    // Hiệu ứng fade in cho panel suy nghĩ
    private IEnumerator FadeInThoughtPanel()
    {
        // Lấy component CanvasGroup nếu có
        CanvasGroup canvasGroup = thoughtPanel.GetComponent<CanvasGroup>();

        // Nếu không có CanvasGroup, thêm mới
        if (canvasGroup == null)
        {
            canvasGroup = thoughtPanel.AddComponent<CanvasGroup>();
        }

        // Bắt đầu từ trong suốt
        canvasGroup.alpha = 0f;

        // Fade in trong 0.5 giây
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo alpha cuối cùng là 1
        canvasGroup.alpha = 1f;
    }

    // Tự động ẩn hình ảnh sau một khoảng thời gian
    private IEnumerator HideImageAfterDelay()
    {
        yield return new WaitForSeconds(imageDisplayTime);
        CloseImagePanel();
    }

    // Tự động ẩn suy nghĩ sau một khoảng thời gian
    private IEnumerator HideThoughtAfterDelay()
    {
        yield return new WaitForSeconds(thoughtDisplayTime);
        CloseThoughtPanel();
    }

    // Đóng panel hình ảnh
    public void CloseImagePanel()
    {
        if (imagePanel != null)
        {
            imagePanel.SetActive(false);
        }

        // Reset coroutine
        imageDisplayCoroutine = null;
    }

    // Đóng panel suy nghĩ
    public void CloseThoughtPanel()
    {
        StartCoroutine(FadeOutThoughtPanel());
    }

    // Hiệu ứng fade out cho panel suy nghĩ
    private IEnumerator FadeOutThoughtPanel()
    {
        // Lấy component CanvasGroup
        CanvasGroup canvasGroup = thoughtPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = thoughtPanel.AddComponent<CanvasGroup>();
        }

        // Bắt đầu từ không trong suốt
        float startAlpha = canvasGroup.alpha;

        // Fade out trong 0.5 giây
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo alpha cuối cùng là 0
        canvasGroup.alpha = 0f;

        // Ẩn panel
        thoughtPanel.SetActive(false);

        // Reset coroutine
        thoughtDisplayCoroutine = null;
    }

    public void StopInteract()
    {
        // Implement StopInteract to handle when player exits interaction area
        if (isDialogActive)
        {
            EndDialog();
        }
    }
}