using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    [Header("UI Controls")]
    public Button closeButton; // Add close button reference

    // Reference to the PetInfoUIManager to interact with pet care functionality
    private PetInfoUIManager petInfoManager;
    private FeedingManager feedingManager; // Add FeedingManager reference
    private PlayingManager playingManager; // Add PlayingManager reference
    private PetSleepManager petSleepManager; // Add PetSleepManager reference

    [Header("Sleep Settings")]
    [SerializeField] private float dialogueSleepDuration = 8f;
    [SerializeField] private bool showSleepMessage = true;

    // ========== FIX: TRACK CURRENT NPC ==========
    private NPC currentNPC; // Track which NPC is currently in dialogue

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
        // Initialize close button
        InitializeCloseButton();

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
        
        // Find or create PetSleepManager
        petSleepManager = FindObjectOfType<PetSleepManager>();
        if (petSleepManager == null)
        {
            GameObject sleepManagerObj = new GameObject("PetSleepManager");
            petSleepManager = sleepManagerObj.AddComponent<PetSleepManager>();
            Debug.Log("PetSleepManager created automatically.");
        }
    }

    /// <summary>
    /// Initialize the close button functionality
    /// </summary>
    private void InitializeCloseButton()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseDialogue);
            
            // Add tooltip to close button
            TooltipTrigger tooltip = closeButton.gameObject.GetComponent<TooltipTrigger>();
            if (tooltip == null)
            {
                tooltip = closeButton.gameObject.AddComponent<TooltipTrigger>();
            }
            tooltip.GetDynamicTooltip = () => "Close dialogue";
            tooltip.SetTooltipColors(new Color(0.3f, 0.3f, 0.3f, 0.9f), Color.white);
            
            Debug.Log("✅ Close button initialized for DialogueController");
        }
        else
        {
            Debug.LogWarning("⚠️ Close button not assigned in DialogueController!");
        }
    }

    /// <summary>
    /// Close the dialogue panel and cleanup - FIX: Notify NPC
    /// </summary>
    public void CloseDialogue()
    {
        if (dialoguePanel != null && dialoguePanel.activeInHierarchy)
        {
            // ========== FIX: NOTIFY CURRENT NPC TO END DIALOGUE ==========
            if (currentNPC != null)
            {
                currentNPC.EndDialogue();
                currentNPC = null; // Clear reference
                Debug.Log("🔄 NPC dialogue state reset");
            }
            
            // Clear any existing choices
            ClearChoices();
            
            // Hide the dialogue UI
            ShowDialogueUI(false);
            
            // Clear dialogue text
            if (dialogueText != null)
                dialogueText.text = "";
            
            // Fire close event if needed
            OnDialogueClosed?.Invoke();
            
            Debug.Log("🚪 Dialogue closed by user");
        }
    }

    /// <summary>
    /// Set current NPC - called when NPC starts dialogue
    /// </summary>
    public void SetCurrentNPC(NPC npc)
    {
        currentNPC = npc;
        Debug.Log($"🎭 Current NPC set: {(npc != null ? npc.name : "null")}");
    }

    /// <summary>
    /// Clear current NPC reference - called when dialogue ends normally
    /// </summary>
    public void ClearCurrentNPC()
    {
        currentNPC = null;
        Debug.Log("🎭 Current NPC cleared");
    }

    /// <summary>
    /// Event fired when dialogue is closed
    /// </summary>
    public System.Action OnDialogueClosed;

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);
        
        // ========== CLEANUP TOOLTIP KHI ĐÓNG ==========
        if (!show && TooltipSystem.Instance != null)
        {
            TooltipSystem.Instance.HideTooltip();
        }

        // ========== SETUP CLOSE BUTTON VISIBILITY ==========
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(show);
        }

        // ========== FIX: CLEAR NPC REFERENCE WHEN HIDING ==========
        if (!show && currentNPC != null)
        {
            ClearCurrentNPC();
        }
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
        // ========== HIDE TOOLTIP TRƯỚC KHI DESTROY BUTTONS ==========
        if (TooltipSystem.Instance != null)
        {
            TooltipSystem.Instance.HideTooltip();
        }
        
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
        
        // ========== KHAI BÁO blockReason Ở ĐẦU ==========
        bool disableButton = false;
        PetInfoUIManager.ActionBlockReason blockReason = PetInfoUIManager.ActionBlockReason.None;
        
        if (petInfoManager != null)
        {
            // Convert DialogueController.PetCareAction to PetAction.ActionType
            PetAction.ActionType actionType = ConvertToPetActionType(careAction);
            
            // Check dependency
            blockReason = petInfoManager.CanPerformAction(actionType);
            
            if (blockReason != PetInfoUIManager.ActionBlockReason.None)
            {
                disableButton = true;
            }
            else
            {
                // ========== CHECK SLEEP STATUS ==========
                if (careAction == PetCareAction.Sleep && petSleepManager != null)
                {
                    int currentPetId = GetCurrentPetId();
                    if (currentPetId != -1 && petSleepManager.IsPetSleeping(currentPetId))
                    {
                        disableButton = true;
                        blockReason = PetInfoUIManager.ActionBlockReason.TooEnergetic; // Reuse existing reason
                    }
                }
                
                // Original max status checks
                switch (careAction)
                {
                    case PetCareAction.Feed:
                        disableButton = petInfoManager.IsHungerAtMax();
                        break;
                        
                    case PetCareAction.Play:
                        disableButton = petInfoManager.IsHappinessAtMax();
                        break;
                        
                    case PetCareAction.Sleep:
                        if (!disableButton) // Only check if not already disabled by sleep status
                            disableButton = petInfoManager.IsEnergyAtMax();
                        break;
                        
                    case PetCareAction.CareForAll:
                        disableButton = petInfoManager.IsAllStatusAtMax();
                        break;
                }
            }
        }
        
        // ========== GIỮ NGUYÊN ORIGINAL TEXT ==========
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        button.interactable = !disableButton;
        
        // ========== TOOLTIP VẪN HOẠT ĐỘNG BÌNH THƯỜNG ==========
        AddTooltipToDialogueButton(button, careAction, blockReason);
        
        // Modified: Add action to perform pet care and close dialogue
        button.onClick.AddListener(() => {
            // ========== HIDE TOOLTIP TRƯỚC KHI THỰC HIỆN ACTION ==========
            if (TooltipSystem.Instance != null)
            {
                TooltipSystem.Instance.HideTooltip();
            }
            
            PerformPetCareAction(careAction, customCareAmount);
            // Close the dialogue panel after performing care action
            ShowDialogueUI(false);
        });
        
        if (additionalAction != null)
        {
            button.onClick.AddListener(additionalAction);
        }
        
        return choiceButton;
    }

    // Method mới để add tooltip
    private void AddTooltipToDialogueButton(Button button, PetCareAction careAction, PetInfoUIManager.ActionBlockReason blockReason)
    {
        TooltipTrigger tooltip = button.gameObject.AddComponent<TooltipTrigger>();
        
        // Set dynamic tooltip content
        tooltip.GetDynamicTooltip = () => GetDialogueButtonTooltipText(careAction, blockReason);
        
        // Set colors based on block reason
        Color bgColor, textColor;
        GetDialogueTooltipColors(blockReason, out bgColor, out textColor);
        tooltip.SetTooltipColors(bgColor, textColor);
    }

    private string GetDialogueButtonTooltipText(PetCareAction careAction, PetInfoUIManager.ActionBlockReason blockReason)
    {
        // ========== CHECK SLEEP STATUS FOR TOOLTIP ==========
        if (careAction == PetCareAction.Sleep && petSleepManager != null)
        {
            int currentPetId = GetCurrentPetId();
            if (currentPetId != -1 && petSleepManager.IsPetSleeping(currentPetId))
            {
                float remainingTime = petSleepManager.GetRemainingSleepTime(currentPetId);
                return $"Pet is sleeping";
            }
        }
        
        if (blockReason == PetInfoUIManager.ActionBlockReason.None)
        {
            return $"Ready to {careAction.ToString().ToLower()}";
        }
        else
        {
            // ========== FORMAT ĐẸP VỚI ICON ==========
            string icon = blockReason == PetInfoUIManager.ActionBlockReason.Critical ? "" : "";
            return $"{icon} {GetShortBlockReason(blockReason)}";
        }
    }

    private void GetDialogueTooltipColors(PetInfoUIManager.ActionBlockReason reason, out Color backgroundColor, out Color textColor)
    {
        switch (reason)
        {
            case PetInfoUIManager.ActionBlockReason.None:
                backgroundColor = new Color(0.2f, 0.5f, 0.2f, 0.9f); // Green
                textColor = Color.white;
                break;
                
            case PetInfoUIManager.ActionBlockReason.Critical:
                backgroundColor = new Color(0.7f, 0.1f, 0.1f, 0.9f); // Red
                textColor = Color.white;
                break;
                
            case PetInfoUIManager.ActionBlockReason.HappinessAtMax:
                backgroundColor = new Color(0.1f, 0.3f, 0.6f, 0.9f); // Blue (same as satisfied states)
                textColor = Color.white;
                break;
                
            default:
                backgroundColor = new Color(0.7f, 0.5f, 0.1f, 0.9f); // Orange
                textColor = Color.white;
                break;
        }
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
            case PetInfoUIManager.ActionBlockReason.Critical: return "Critical!";
            case PetInfoUIManager.ActionBlockReason.TooFull: return "Full";
            case PetInfoUIManager.ActionBlockReason.TooEnergetic: return "Too Energetic";
            case PetInfoUIManager.ActionBlockReason.HappinessAtMax: return "Very Happy";
            default: return "Blocked";
        }
    }

    // ========== GET CURRENT PET ID ==========
    private int GetCurrentPetId()
    {
        // Try to get current pet ID from PetInfoUIManager
        if (petInfoManager != null)
        {
            // Access private field through reflection or add public getter
            var currentPetDetails = petInfoManager.GetComponent<PetInfoUIManager>();
            // For now, return player's first pet or use a different method
            User user = PlayerInfomation.LoadPlayerInfo();
            var pets = APIPlayerPet.GetPetsByPlayerId(user.id);
            if (pets != null && pets.Count > 0)
            {
                return pets[0].petID;
            }
        }
        return -1;
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
                // ========== ENHANCED SLEEP ACTION ==========
                if (petSleepManager != null)
                {
                    int currentPetId = GetCurrentPetId();
                    if (currentPetId != -1)
                    {
                        // Check if pet is already sleeping
                        if (petSleepManager.IsPetSleeping(currentPetId))
                        {
                            float remainingTime = petSleepManager.GetRemainingSleepTime(currentPetId);
                            if (showSleepMessage)
                            {
                                petInfoManager.ShowStatusMessage($"Pet is sleeping", Color.yellow);
                            }
                            return;
                        }
                        
                        // Put pet to sleep
                        petSleepManager.PutPetToSleep(currentPetId, dialogueSleepDuration);
                        
                        // Update pet status (energy)
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
                        
                        // Show sleep message
                        //if (showSleepMessage)
                        //{
                        //    petInfoManager.ShowStatusMessage($"Pet is now sleeping for {dialogueSleepDuration} seconds", Color.blue);
                        //}
                    }
                    else
                    {
                        Debug.LogWarning("Could not find current pet ID for sleep action");
                        // Fallback to normal sleep
                        if (customCareAmount > 0)
                        {
                            petInfoManager.UpdatePetStatus(2, customCareAmount);
                        }
                        else
                        {
                            petInfoManager.SleepPet();
                        }
                    }
                }
                else
                {
                    // Fallback to normal sleep if PetSleepManager not available
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

    /// <summary>
    /// Alternative method to close dialogue (can be called from external scripts)
    /// </summary>
    public void CloseDialogueExternal()
    {
        CloseDialogue();
    }

    /// <summary>
    /// Check if dialogue is currently active
    /// </summary>
    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeInHierarchy;
    }

    /// <summary>
    /// Force close dialogue (for emergency situations)
    /// </summary>
    public void ForceCloseDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        
        // ========== FIX: RESET NPC STATE ==========
        if (currentNPC != null)
        {
            currentNPC.EndDialogue();
            currentNPC = null;
        }
        
        // Clear everything
        ClearChoices();
        if (dialogueText != null) dialogueText.text = "";
        if (nameText != null) nameText.text = "";
        if (portraitImage != null) portraitImage.sprite = null;
        
        Debug.Log("🚪 Dialogue force closed");
    }
}