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
    public Button closeButton;

    private PetInfoUIManager petInfoManager;
    private FeedingManager feedingManager;
    private PlayingManager playingManager;
    private PetSleepManager petSleepManager;

    [Header("Sleep Settings")]
    [SerializeField] private float dialogueSleepDuration = 8f;
    [SerializeField] private bool showSleepMessage = true;

    private NPC currentNPC;

    public enum PetCareAction
    {
        None,
        Feed,
        Play,
        Sleep,
        CareForAll
    }

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
        InitializeCloseButton();

        petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager == null)
        {
            Debug.LogWarning("PetInfoUIManager not found in the scene. Pet care dialogue options will not work.");
        }
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
        
        petSleepManager = FindObjectOfType<PetSleepManager>();
        if (petSleepManager == null)
        {
            GameObject sleepManagerObj = new GameObject("PetSleepManager");
            petSleepManager = sleepManagerObj.AddComponent<PetSleepManager>();
            Debug.Log("PetSleepManager created automatically.");
        }
    }

    private void InitializeCloseButton()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseDialogue);
            
            TooltipTrigger tooltip = closeButton.gameObject.GetComponent<TooltipTrigger>();
            if (tooltip == null)
            {
                tooltip = closeButton.gameObject.AddComponent<TooltipTrigger>();
            }
            tooltip.GetDynamicTooltip = () => "Close dialogue";
            tooltip.SetTooltipColors(new Color(0.3f, 0.3f, 0.3f, 0.9f), Color.white);
            
            Debug.Log("? Close button initialized for DialogueController");
        }
        else
        {
            Debug.LogWarning("?? Close button not assigned in DialogueController!");
        }
    }

    public void CloseDialogue()
    {
        if (dialoguePanel != null && dialoguePanel.activeInHierarchy)
        {
            if (currentNPC != null)
            {
                currentNPC.EndDialogue();
                currentNPC = null;
                Debug.Log("?? NPC dialogue state reset");
            }

            ClearChoices();

            ShowDialogueUI(false);

            if (dialogueText != null)
                dialogueText.text = "";

            OnDialogueClosed?.Invoke();

            Debug.Log("?? Dialogue closed by user");
        }
    }

    public void SetCurrentNPC(NPC npc)
    {
        currentNPC = npc;
        Debug.Log($"?? Current NPC set: {(npc != null ? npc.name : "null")}");
    }

    public void ClearCurrentNPC()
    {
        currentNPC = null;
        Debug.Log("?? Current NPC cleared");
    }

    public System.Action OnDialogueClosed;

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);
        
        if (!show && TooltipSystem.Instance != null)
        {
            TooltipSystem.Instance.HideTooltip();
        }

        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(show);
        }

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

    public GameObject CreatePetCareChoiceButton(string choiceText, PetCareAction careAction, UnityEngine.Events.UnityAction additionalAction = null, int customCareAmount = 0)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        Button button = choiceButton.GetComponent<Button>();
        
        bool disableButton = false;
        PetInfoUIManager.ActionBlockReason blockReason = PetInfoUIManager.ActionBlockReason.None;
        
        if (petInfoManager != null)
        {
            PetAction.ActionType actionType = ConvertToPetActionType(careAction);
            
            blockReason = petInfoManager.CanPerformAction(actionType);
            
            if (blockReason != PetInfoUIManager.ActionBlockReason.None)
            {
                disableButton = true;
            }
            else
            {
                if (careAction == PetCareAction.Sleep && petSleepManager != null)
                {
                    int currentPetId = GetCurrentPetId();
                    if (currentPetId != -1 && petSleepManager.IsPetSleeping(currentPetId))
                    {
                        disableButton = true;
                        blockReason = PetInfoUIManager.ActionBlockReason.TooEnergetic;
                    }
                }
                
                switch (careAction)
                {
                    case PetCareAction.Feed:
                        disableButton = petInfoManager.IsHungerAtMax();
                        break;
                        
                    case PetCareAction.Play:
                        disableButton = petInfoManager.IsHappinessAtMax();
                        break;
                        
                    case PetCareAction.Sleep:
                        if (!disableButton)
                            disableButton = petInfoManager.IsEnergyAtMax();
                        break;
                        
                    case PetCareAction.CareForAll:
                        disableButton = petInfoManager.IsAllStatusAtMax();
                        break;
                }
            }
        }
        
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        button.interactable = !disableButton;
        
        AddTooltipToDialogueButton(button, careAction, blockReason);
        
        button.onClick.AddListener(() => {
            if (TooltipSystem.Instance != null)
            {
                TooltipSystem.Instance.HideTooltip();
            }
            
            PerformPetCareAction(careAction, customCareAmount);
            ShowDialogueUI(false);
        });
        
        if (additionalAction != null)
        {
            button.onClick.AddListener(additionalAction);
        }
        
        return choiceButton;
    }

    private void AddTooltipToDialogueButton(Button button, PetCareAction careAction, PetInfoUIManager.ActionBlockReason blockReason)
    {
        TooltipTrigger tooltip = button.gameObject.AddComponent<TooltipTrigger>();
        
        tooltip.GetDynamicTooltip = () => GetDialogueButtonTooltipText(careAction, blockReason);
        
        Color bgColor, textColor;
        GetDialogueTooltipColors(blockReason, out bgColor, out textColor);
        tooltip.SetTooltipColors(bgColor, textColor);
    }

    private string GetDialogueButtonTooltipText(PetCareAction careAction, PetInfoUIManager.ActionBlockReason blockReason)
    {
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
            string icon = blockReason == PetInfoUIManager.ActionBlockReason.Critical ? "" : "";
            return $"{icon} {GetShortBlockReason(blockReason)}";
        }
    }

    private void GetDialogueTooltipColors(PetInfoUIManager.ActionBlockReason reason, out Color backgroundColor, out Color textColor)
    {
        switch (reason)
        {
            case PetInfoUIManager.ActionBlockReason.None:
                backgroundColor = new Color(0.2f, 0.5f, 0.2f, 0.9f);
                textColor = Color.white;
                break;
                
            case PetInfoUIManager.ActionBlockReason.Critical:
                backgroundColor = new Color(0.7f, 0.1f, 0.1f, 0.9f);
                textColor = Color.white;
                break;
                
            case PetInfoUIManager.ActionBlockReason.HappinessAtMax:
                backgroundColor = new Color(0.1f, 0.3f, 0.6f, 0.9f);
                textColor = Color.white;
                break;
                
            default:
                backgroundColor = new Color(0.7f, 0.5f, 0.1f, 0.9f);
                textColor = Color.white;
                break;
        }
    }

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

    private int GetCurrentPetId()
    {
        if (petInfoManager != null)
        {
            var (currentPetId, _) = petInfoManager.GetCurrentPetAndPlayerId();
            if (currentPetId != -1)
            {
                return currentPetId;
            }
        }
        
        PetDataHolder[] petHolders = FindObjectsOfType<PetDataHolder>();
        foreach (var holder in petHolders)
        {
            if (holder.petData != null && holder.petData.playerPetID > 0)
            {
                return holder.petData.playerPetID;
            }
        }
        
        return -1;
    }

    private void PerformPetCareAction(PetCareAction action, int customCareAmount = 0)
    {
        if (petInfoManager == null)
        {
            Debug.LogWarning("Cannot perform pet care action: PetInfoUIManager is not found");
            return;
        }

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
        int currentPetId = GetCurrentPetId();

        bool petWasAwakened = false;
        if (petSleepManager != null && currentPetId != -1)
        {
            petWasAwakened = petSleepManager.WakeUpPetForCareAction(currentPetId, actionType);
            if (petWasAwakened)
            {
                petInfoManager.ShowStatusMessage($"Pet woke up for {action.ToString().ToLower()}! ??", Color.cyan);
            }
        }

        switch (action)
        {
            case PetCareAction.Feed:
                if (petWasAwakened)
                {
                    Debug.Log($"?? Pet {currentPetId} was awakened for feeding via dialogue");
                }

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
                petInfoManager.OnFeedButtonClickedWithHistory();
                break;

            case PetCareAction.Play:
                if (petWasAwakened)
                {
                    Debug.Log($"?? Pet {currentPetId} was awakened for playing via dialogue");
                }

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
                //petInfoManager.OnPlayButtonClickedWithHistory();
                break;

            case PetCareAction.Sleep:
                if (petSleepManager != null)
                {
                    if (currentPetId != -1)
                    {
                        if (petSleepManager.IsPetSleeping(currentPetId))
                        {
                            float remainingTime = petSleepManager.GetRemainingSleepTime(currentPetId);
                            if (showSleepMessage)
                            {
                                petInfoManager.ShowStatusMessage($"Pet is sleeping", Color.yellow);
                            }
                            return;
                        }
                        petSleepManager.PutPetToSleep(currentPetId, dialogueSleepDuration);

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
                    else
                    {
                        Debug.LogWarning("Could not find current pet ID for sleep action");
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
                petInfoManager.OnSleepButtonClickedWithHistory();
                break;
            case PetCareAction.None:
            default:
                break;
        }
    }

    public void CloseDialogueExternal()
    {
        CloseDialogue();
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeInHierarchy;
    }

    public void ForceCloseDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        
        if (currentNPC != null)
        {
            currentNPC.EndDialogue();
            currentNPC = null;
        }
        
        ClearChoices();
        if (dialogueText != null) dialogueText.text = "";
        if (nameText != null) nameText.text = "";
        if (portraitImage != null) portraitImage.sprite = null;
        
        Debug.Log("?? Dialogue force closed");
    }

    public bool StartDialogueWithNPC(NPC npc)
    {
        if (npc == null)
        {
            Debug.LogError("Cannot start dialogue: NPC is null");
            return false;
        }

        if (!npc.CanInteract())
        {
            Debug.LogWarning($"Cannot start dialogue: NPC {npc.name} is busy");
            return false;
        }

        if (!npc.HasDialogueData())
        {
            Debug.LogWarning($"Cannot start dialogue: NPC {npc.name} has no dialogue data");
            return false;
        }

        if (IsDialogueActive())
        {
            Debug.LogWarning("Cannot start dialogue: Another dialogue is already active");
            return false;
        }

        try
        {
            npc.StartDialogueExternal();
            Debug.Log($"? Successfully started external dialogue with NPC: {npc.name}");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to start dialogue with NPC {npc.name}: {ex.Message}");
            return false;
        }
    }

    public bool StartDialogueWithAnyNPC()
    {
        NPC[] npcs = FindObjectsOfType<NPC>();
        
        if (npcs == null || npcs.Length == 0)
        {
            Debug.LogWarning("No NPCs found in the scene");
            return false;
        }

        foreach (NPC npc in npcs)
        {
            if (npc.CanInteract() && npc.HasDialogueData())
            {
                return StartDialogueWithNPC(npc);
            }
        }

        Debug.LogWarning("No available NPCs with dialogue data found");
        return false;
    }
}