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

    private int dialogIndex;
    private bool isTyping, isDialogActive;

    public bool CanInteract()
    {
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
        else 
        {
            dialogIndex++; // Tăng index trước khi chuyển dòng tiếp theo
            if (dialogIndex < DialogData.dialogLines.Length)
            {
                StartCoroutine(TypeLine());
            }
            else
            {
                EndDialog();
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogText.SetText("");

        foreach (char letter in DialogData.dialogLines[dialogIndex])
        {
            dialogText.text += letter;
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

        // Phá hủy target GameObject nếu nó tồn tại
        if (targetToDestroy != null)
        {
            Destroy(targetToDestroy);
        }
    }
    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
