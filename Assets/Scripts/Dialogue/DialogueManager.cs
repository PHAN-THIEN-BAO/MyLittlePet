using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public delegate void DialogueEvent();
    public static event DialogueEvent OnDialogueEnd;
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
                    Debug.LogError("DialogueManager không t?n t?i trong scene!");
                }
            }
            return _instance;
        }
    }
    public Image characterIcon;
    public TMP_Text characterName;
    public TMP_Text dialogueText;
    public GameObject dialogPanel;
    private Queue<DialogueLine> line = new Queue<DialogueLine>();
    public bool isDialogueActive = false;
    public float typingSpeed = 0.05f;
    public Animator animator;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
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
        if (dialogPanel != null)
            dialogPanel.SetActive(true);
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
            yield return new WaitForSeconds(2f);
            DisplayNextDialogueLine();
        }
    }
    void EndDialogue()
    {
        isDialogueActive = false;
        if (animator != null)
            animator.SetBool("IsOpen", false);
        if (OnDialogueEnd != null)
            OnDialogueEnd();
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }
    public void ContinueDialogue()
    {
        if (isDialogueActive)
        {
            DisplayNextDialogueLine();
        }
    }
}