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

//    [Header("Close Button Integration")]
//    public Button closeButton; // Assign the close button in inspector

//    private int dialogIndex;
//    private bool isTyping, isDialogActive;

//    private void Start()
//    {
//        // Setup close button if assigned
//        if (closeButton != null)
//        {
//            closeButton.onClick.AddListener(OnCloseButtonClicked);
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
//        else if(++dialogIndex < DialogData.dialogLines.Length)
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

//        //Đánh dấu đã nói chuyện với NPC Bob
//        if (DialogData.npcName == "Bob")  // Tên phải khớp với phần Inspector
//        {
//            DialogManager.Instance.hasTalkedToBob = true;
//        }


//        // Phá hủy target GameObject nếu nó tồn tại
//        if (targetToDestroy != null)
//        {
//            Destroy(targetToDestroy);
//        }
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

    [Header("Close Button Integration")]
    public Button closeButton; // Assign the close button in inspector

    private int dialogIndex;
    private bool isTyping, isDialogActive;

    private void Start()
    {
        // Setup close button if assigned
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        // Ẩn dấu chấm than của Boss nếu chưa nói chuyện với Bob
        if (bossExclamationMark != null && !DialogManager.Instance.hasTalkedToBob)
        {
            bossExclamationMark.SetActive(false);
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

    public void StopInteract()
    {
        // Implement StopInteract to handle when player exits interaction area
        if (isDialogActive)
        {
            EndDialog();
        }
    }
}