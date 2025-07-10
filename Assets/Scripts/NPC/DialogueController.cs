using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    // Reference to the PetInfoUIManager to interact with pet care functionality
    private PetInfoUIManager petInfoManager;
    private FeedingManager feedingManager; // Add FeedingManager reference
    private PlayingManager playingManager; // Add PlayingManager reference

    // Types of pet care actions that can be performed via dialogue
    public enum PetCareAction
    {
        None,
        Feed,
        Play,
        Sleep,
        CareForAll
    }

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
        // Find the PetInfoUIManager in the scene
        petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager == null)
        {
            Debug.LogWarning("PetInfoUIManager not found in the scene. Pet care dialogue options will not work.");
        }
        // Find the FeedingManager in the scene
        feedingManager = FindObjectOfType<FeedingManager>();
        if (feedingManager == null)
        {
            Debug.LogWarning("FeedingManager not found in the scene. Feeding panel will not show food items.");
        }
        playingManager = FindObjectOfType<PlayingManager>();
        if (playingManager == null)
        {
            Debug.LogWarning("PlayingManager not found in the scene. Playing panel will not show toy items.");
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

    // Create a choice button that performs a pet care action
    public GameObject CreatePetCareChoiceButton(string choiceText, PetCareAction careAction, UnityEngine.Events.UnityAction additionalAction = null, int customCareAmount = 0)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        Button button = choiceButton.GetComponent<Button>();
        
        // ========== THÊM DEPENDENCY CHECK ==========
        bool disableButton = false;
        string modifiedText = choiceText;
        
        if (petInfoManager != null)
        {
            // Convert DialogueController.PetCareAction to PetAction.ActionType
            PetAction.ActionType actionType = ConvertToPetActionType(careAction);
            
            // Check dependency
            var blockReason = petInfoManager.CanPerformAction(actionType);
            
            if (blockReason != PetInfoUIManager.ActionBlockReason.None)
            {
                disableButton = true;
                string reason = GetShortBlockReason(blockReason);
                modifiedText = $"{choiceText} ({reason})";
            }
            else
            {
                // Original max status checks
                switch (careAction)
                {
                    case PetCareAction.Feed:
                        disableButton = petInfoManager.IsHungerAtMax();
                        if (disableButton) modifiedText = choiceText + " (Full)";
                        break;
                        
                    case PetCareAction.Play:
                        disableButton = petInfoManager.IsHappinessAtMax();
                        if (disableButton) modifiedText = choiceText + " (Happy)";
                        break;
                        
                    case PetCareAction.Sleep:
                        disableButton = petInfoManager.IsEnergyAtMax();
                        if (disableButton) modifiedText = choiceText + " (Energetic)";
                        break;
                        
                    case PetCareAction.CareForAll:
                        disableButton = petInfoManager.IsAllStatusAtMax();
                        if (disableButton) modifiedText = choiceText + " (Not Needed)";
                        break;
                }
            }
        }
        
        choiceButton.GetComponentInChildren<TMP_Text>().text = modifiedText;
        button.interactable = !disableButton;
        
        // Rest of existing code...
        button.onClick.AddListener(() => PerformPetCareAction(careAction, customCareAmount));
        if (additionalAction != null)
        {
            button.onClick.AddListener(additionalAction);
        }
        
        return choiceButton;
    }

    // Helper method to convert action types
    private PetAction.ActionType ConvertToPetActionType(PetCareAction careAction)
    {
        switch (careAction)
        {
            case PetCareAction.Feed: return PetAction.ActionType.Feed;
            case PetCareAction.Play: return PetAction.ActionType.Play;
            case PetCareAction.Sleep: return PetAction.ActionType.Sleep;
            case PetCareAction.CareForAll: return PetAction.ActionType.CareForAll;
            default: return PetAction.ActionType.Feed;
        }
    }

    // Helper method for short block reasons
    private string GetShortBlockReason(PetInfoUIManager.ActionBlockReason reason)
    {
        switch (reason)
        {
            case PetInfoUIManager.ActionBlockReason.TooHungry: return "Need Food";
            case PetInfoUIManager.ActionBlockReason.TooTired: return "Need Sleep";
            case PetInfoUIManager.ActionBlockReason.Critical: return "CRITICAL!";
            case PetInfoUIManager.ActionBlockReason.TooFull: return "Full";
            case PetInfoUIManager.ActionBlockReason.TooEnergetic: return "Too Energetic";
            default: return "Blocked";
        }
    }

    // Execute the selected pet care action
    private void PerformPetCareAction(PetCareAction action, int customCareAmount = 0)
    {
        if (petInfoManager == null)
        {
            Debug.LogWarning("Cannot perform pet care action: PetInfoUIManager is not found");
            return;
        }

        // ========== THÊM DEPENDENCY CHECK ==========
        PetAction.ActionType actionType = ConvertToPetActionType(action);
        var blockReason = petInfoManager.CanPerformAction(actionType);
        
        if (blockReason != PetInfoUIManager.ActionBlockReason.None)
        {
            string message = petInfoManager.GetBlockReasonMessage(blockReason, actionType);
            Debug.LogWarning($"Dialogue action blocked: {message}");
            petInfoManager.ShowStatusMessage(message, Color.red);
            return;
        }

        int playerId = PlayerInfomation.LoadPlayerInfo().id;

        switch (action)
        {
            case PetCareAction.Feed:
                if (feedingManager != null)
                {
                    feedingManager.ShowFeedingPanel(playerId, customCareAmount);
                    Debug.Log("Showing feeding panel (with FeedingManager)");
                }
                else
                {
                    petInfoManager.ShowFeedingPanel(customCareAmount);
                    Debug.Log("Showing feeding panel (PetInfoUIManager fallback)");
                }
                break;

            case PetCareAction.Play:
                if (playingManager != null)
                {
                    playingManager.ShowPlayingPanel(playerId);
                    Debug.Log("Showing playing panel (with PlayingManager)");
                }
                else
                {
                    if (customCareAmount > 0)
                    {
                        petInfoManager.UpdatePetStatus(1, customCareAmount);
                        Debug.Log($"Dialogue choice: Play with pet with custom amount: {customCareAmount}");
                    }
                    else
                    {
                        petInfoManager.PlayWithPet();
                        Debug.Log("Dialogue choice: Play with pet");
                    }
                }
                break;

            case PetCareAction.Sleep:
                if (customCareAmount > 0)
                {
                    petInfoManager.UpdatePetStatus(2, customCareAmount);
                    Debug.Log($"Dialogue choice: Pet sleeps with custom amount: {customCareAmount}");
                }
                else
                {
                    petInfoManager.SleepPet();
                    Debug.Log("Dialogue choice: Pet sleeps");
                }
                break;

            case PetCareAction.CareForAll:
                if (customCareAmount > 0)
                {
                    // Smart care với custom amount
                    petInfoManager.ScheduleSmartCare();
                    Debug.Log($"Dialogue choice: Smart care for all pet needs");
                }
                else
                {
                    petInfoManager.OnCareForAllButtonClicked();
                    Debug.Log("Dialogue choice: Care for all pet needs");
                }
                break;

            case PetCareAction.None:
            default:
                break;
        }
    }
}