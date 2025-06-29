﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;


    private DialogueController dialogueUI;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;

    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null)
            return;
        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    public void StopInteract()
    {
        throw new System.NotImplementedException();
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);

        DisplayCurrentLine();

    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }


        dialogueUI.ClearChoices();

        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }
        if (dialogueIndex + 1 < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");
        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;
        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }
    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];

            // Check if this choice has a pet care action assigned
            if (choice.petCareOptions != null && i < choice.petCareOptions.Length &&
                choice.petCareOptions[i] != PetCareOptionType.None)
            {
                // Convert PetCareOptionType to DialogueController.PetCareAction
                DialogueController.PetCareAction careAction = DialogueController.PetCareAction.None;

                switch (choice.petCareOptions[i])
                {
                    case PetCareOptionType.Feed:
                        careAction = DialogueController.PetCareAction.Feed;
                        break;
                    case PetCareOptionType.Play:
                        careAction = DialogueController.PetCareAction.Play;
                        break;
                    case PetCareOptionType.Sleep:
                        careAction = DialogueController.PetCareAction.Sleep;
                        break;
                    case PetCareOptionType.CareForAll:
                        careAction = DialogueController.PetCareAction.CareForAll;
                        break;
                }

                // Get custom care amount if specified
                int customCareAmount = 0;
                if (choice.customCareAmount != null && i < choice.customCareAmount.Length)
                {
                    customCareAmount = choice.customCareAmount[i];
                }

                // Create a pet care choice button that also progresses the dialogue
                dialogueUI.CreatePetCareChoiceButton(
                    choice.choices[i],
                    careAction,
                    () => ChooseOption(nextIndex),
                    customCareAmount);
            }
            else
            {
                // Create a regular choice button
                dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex));
            }
        }
    }

    void ChooseOption(int nextIndex)
    {
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
    }

}