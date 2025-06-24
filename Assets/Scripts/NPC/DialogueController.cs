using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    // Reference to PetStatusManager
    private PetStatusManager petStatusManager;

    // Action event for choice effects
    public event Action<int, DialogueEffect> OnChoiceEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        petStatusManager = FindObjectOfType<PetStatusManager>();
        if (petStatusManager == null)
        {
            Debug.LogWarning("PetStatusManager not found. Pet status effects won't work.");
        }
    }

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait; 
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public GameObject CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
        return choiceButton;
    }

    // New method for creating choice buttons with pet status effects
    public GameObject CreateChoiceButtonWithEffect(string choiceText, UnityEngine.Events.UnityAction onClick, DialogueEffect effect, int petId = -1)
    {
        GameObject choiceButton = CreateChoiceButton(choiceText, () => {
            // First trigger the dialogue effect
            ApplyDialogueEffect(petId, effect);
            
            // Then continue with original onClick action
            onClick?.Invoke();
        });
        
        return choiceButton;
    }

    // Apply the dialogue effect to pet status
    private void ApplyDialogueEffect(int petId, DialogueEffect effect)
    {
        if (petStatusManager == null) return;

        // If petId is -1, try to get the first pet (could be expanded to target specific/active pet)
        if (petId == -1)
        {
            // Get first pet ID from your pet collection logic here
            // For now using a temporary petId of 1 if none specified
            petId = 1;
        }

        // Apply the effect to the pet
        switch (effect.effectType)
        {
            case EffectType.Hunger:
                petStatusManager.ModifyPetStatus(petId, effect.amount, 0, 0);
                break;
            case EffectType.Happiness:
                petStatusManager.ModifyPetStatus(petId, 0, effect.amount, 0);
                break;
            case EffectType.Energy:
                petStatusManager.ModifyPetStatus(petId, 0, 0, effect.amount);
                break;
            case EffectType.Multiple:
                petStatusManager.ModifyPetStatus(petId, effect.hungerAmount, effect.happinessAmount, effect.energyAmount);
                break;
        }

        // Trigger event for any listeners
        OnChoiceEffect?.Invoke(petId, effect);
    }
}

// New enums and classes to support dialogue effects
[System.Serializable]
public enum EffectType
{
    None,
    Hunger,
    Happiness,
    Energy,
    Multiple
}

[System.Serializable]
public class DialogueEffect
{
    public EffectType effectType = EffectType.None;
    public float amount = 0;
    
    // For multiple effects
    public float hungerAmount = 0;
    public float happinessAmount = 0;
    public float energyAmount = 0;
}
