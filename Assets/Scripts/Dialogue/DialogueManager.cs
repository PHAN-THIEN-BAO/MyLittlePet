using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // Event được kích hoạt khi dialogue kết thúc
    public delegate void DialogueEvent();
    public static event DialogueEvent OnDialogueEnd;

    // Singleton pattern
    private static DialogueManager _instance;
    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogueManager>();
                if (_instance == null)
                {
                    Debug.LogError("DialogueManager không tồn tại trong scene!");
                }
            }
            return _instance;
        }
    }

    // UI References
    public Image characterIcon;
    public TMP_Text characterName;
    public TMP_Text dialogueText;

    // Dialogue data
    private Queue<DialogueLine> line = new Queue<DialogueLine>();
    public bool isDialogueActive = false;
    public float typingSpeed = 0.05f;

    // Optional animator
    public Animator animator;

    private void Awake()
    {
        // Singleton setup
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartDialog(Dialogue dialogue)
    {
        if (dialogue == null || dialogue.dialogueLines.Count == 0)
        {
            Debug.LogWarning("Tried to start dialogue with no lines!");
            return;
        }

        isDialogueActive = true;

        Debug.Log("Starting dialogue with " + dialogue.dialogueLines.Count + " lines.");

        // Play animation if animator exists
        if (animator != null)
            animator.SetBool("IsOpen", true);

        line.Clear();
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            line.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (line.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = line.Dequeue();

        // Update UI elements
        if (characterIcon != null && currentLine.character != null && currentLine.character.icon != null)
            characterIcon.sprite = currentLine.character.icon;

        if (characterName != null && currentLine.character != null)
            characterName.text = currentLine.character.name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
            foreach (char letter in dialogueLine.line.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            // Tùy chọn: Chờ người dùng nhấn để tiếp tục
            // yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            // DisplayNextDialogueLine();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;

        // Play animation if animator exists
        if (animator != null)
            animator.SetBool("IsOpen", false);

        // Trigger event
        if (OnDialogueEnd != null)
            OnDialogueEnd();
    }

    // Phương thức công khai để tiếp tục đoạn hội thoại
    public void ContinueDialogue()
    {
        if (isDialogueActive)
        {
            DisplayNextDialogueLine();
        }
    }
}
