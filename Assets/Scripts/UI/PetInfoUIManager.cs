using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
public class PetInfoUIManager : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject petsInfoPanel;
    public GameObject feedingPanel;
    public Button closeButton;
    [Header("Pet Details UI")]
    public TMP_Text petNameText;
    public TMP_Text petLevelText;
    public TMP_Text petCustomNameText;
    public TMP_Text petAdoptedDateText;
    public Image petImage;
    [Header("Pet Sprites")]
    public Sprite[] petSprites;
    [Header("Status Bar Manager")]
    public PetStatusBarManager statusBarManager;
    [Header("Dialogue Integration")]
    [Tooltip("Button to start dialogue with NPC")]
    public Button talkToNPCButton;
    [Tooltip("NPC to interact with when talk button is pressed")]
    public NPC defaultNPC;
    [Tooltip("Text displayed on the talk button")]
    public string talkButtonText = "Talk to Caregiver";
    [Header("Status Decay Settings")]
    [Tooltip("Time in seconds between status decay updates")]
    public float decayInterval = 300f;
    [Tooltip("Amount to reduce hunger by on decay")]
    public int hungerDecayAmount = 5;
    [Tooltip("Amount to reduce happiness by on decay")]
    public int happinessDecayAmount = 5;
    [Tooltip("Amount to reduce energy by on decay")]
    public int energyDecayAmount = 5;
    [Tooltip("Minimum status value before decay stops")]
    public int minStatusValue = 10;
    [Header("Pet Care Settings")]
    [Tooltip("Amount to increase hunger when feeding pet")]
    public int feedIncreaseAmount = 15;
    [Tooltip("Amount to increase happiness when playing with pet")]
    public int playIncreaseAmount = 15;
    [Tooltip("Amount to increase energy when pet sleeps")]
    public int sleepIncreaseAmount = 15;
    [Tooltip("Maximum value for any status")]
    public int maxStatusValue = 100;
    [Header("Pet Need Thresholds")]
    [Tooltip("Minimum hunger level required to play")]
    public int minHungerForPlay = 30;
    [Tooltip("Minimum energy level required to play")]
    public int minEnergyForPlay = 20;
    [Tooltip("Minimum hunger level required for effective sleep")]
    public int minHungerForSleep = 25;
    [Tooltip("Maximum energy level before sleep becomes less effective")]
    public int maxEnergyForSleep = 80;
    [Tooltip("Critical level - pet requires immediate attention")]
    public int criticalThreshold = 15;
    public enum ActionBlockReason
    {
        None,
        TooHungry,
        TooTired,
        TooFull,
        TooEnergetic,
        Critical,
        HappinessAtMax
    }
    [Header("Action Management")]
    public PetActionManager actionManager;
    public bool useActionSystem = true;
    private int currentPetID = -1;
    private PlayerPet currentPetDetails;
    private Coroutine decayCoroutine;
    private void Start()
    {
        if (statusBarManager == null)
        {
            statusBarManager = GetComponent<PetStatusBarManager>();
            if (statusBarManager == null && petsInfoPanel != null)
            {
                statusBarManager = petsInfoPanel.GetComponentInChildren<PetStatusBarManager>();
            }
            if (statusBarManager == null)
            {
                Debug.LogWarning("No PetStatusBarManager found. Status bars will not be updated.");
            }
        }
        InitializeCloseButton();
        InitializeTalkButton();
        InitializeActionManager();
        if (PetActionManager.Instance != null)
        {
            PetActionManager.Instance.SetPetInfoUIManager(this);
            Debug.Log("? PetInfoUIManager registered with PetActionManager at Start");
        }

        
        if (GlobalPetStatusManager.Instance != null)
        {
            GlobalPetStatusManager.Instance.OnPetStatusDecayed += OnGlobalPetStatusDecayed;
        }
    }
    private void OnEnable()
    {
        if (PetActionManager.Instance != null)
        {
            PetActionManager.Instance.SetPetInfoUIManager(this);
            Debug.Log("? PetInfoUIManager re-registered with PetActionManager on scene load");
        }
    }
    private void InitializeCloseButton()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        else
        {
            Debug.LogWarning("Close button is not assigned in the inspector!");
        }
    }
    private void InitializeTalkButton()
    {
        if (talkToNPCButton != null)
        {
            talkToNPCButton.onClick.RemoveAllListeners();
            talkToNPCButton.onClick.AddListener(OnTalkToNPCButtonClicked);
            var buttonText = talkToNPCButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = talkButtonText;
            }
            TooltipTrigger tooltip = talkToNPCButton.gameObject.GetComponent<TooltipTrigger>();
            if (tooltip == null)
            {
                tooltip = talkToNPCButton.gameObject.AddComponent<TooltipTrigger>();
            }
            tooltip.GetDynamicTooltip = () => GetTalkButtonTooltip();
            tooltip.SetTooltipColors(new Color(0.2f, 0.4f, 0.7f, 0.9f), Color.white);
            Debug.Log("? Talk to NPC button initialized for PetInfoUIManager");
        }
        else
        {
            Debug.LogWarning("?? Talk to NPC button not assigned in PetInfoUIManager inspector!");
        }
    }
    public void OnTalkToNPCButtonClicked()
    {
        if (defaultNPC == null)
        {
            defaultNPC = FindObjectOfType<NPC>();
            if (defaultNPC == null)
            {
                Debug.LogWarning("No NPC found in scene to start dialogue with!");
                ShowStatusMessage("No caregiver available to talk to!", Color.yellow);
                return;
            }
        }
        if (!defaultNPC.CanInteract())
        {
            Debug.LogWarning("NPC is currently busy and cannot start dialogue");
            ShowStatusMessage("Caregiver is currently busy, please try again later!", Color.yellow);
            return;
        }
        try
        {
            defaultNPC.Interact();
            Debug.Log($"?? Started dialogue with NPC: {defaultNPC.name} from pet info panel");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to start dialogue with NPC: {ex.Message}");
            ShowStatusMessage("Failed to start conversation with caregiver!", Color.red);
        }
    }
    private string GetTalkButtonTooltip()
    {
        if (defaultNPC == null)
        {
            return "Caring Options";
        }
        if (!defaultNPC.CanInteract())
        {
            return "Caring Options";
        }
        return "Caring Options";
    }
    private void InitializeActionManager()
    {
        if (useActionSystem)
        {
            actionManager = PetActionManager.Instance;
            if (actionManager == null)
            {
                GameObject actionManagerObj = new GameObject("PetActionManager");
                actionManager = actionManagerObj.AddComponent<PetActionManager>();
                actionManagerObj.AddComponent<PetActionScheduler>();
                Debug.Log("Created new PetActionManager");
            }
            else
            {
                Debug.Log("Using existing PetActionManager");
            }
        }
    }
    private void OnDestroy()
    {
        StopDecaySystem();
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
        if (talkToNPCButton != null)
        {
            talkToNPCButton.onClick.RemoveListener(OnTalkToNPCButtonClicked);
        }

        
        if (GlobalPetStatusManager.Instance != null)
        {
            GlobalPetStatusManager.Instance.OnPetStatusDecayed -= OnGlobalPetStatusDecayed;
        }
    }
    public void ToggleInfoPanel(int petID)
    {
        Debug.Log("ToggleInfoPanel called with petID: " + petID);
        bool isActive = petsInfoPanel.activeSelf;
        if (isActive && currentPetID == petID)
        {
            petsInfoPanel.SetActive(false);
            return;
        }
        currentPetID = petID;
        petsInfoPanel.SetActive(true);
        DisplayPetDetails(petID);
        UpdateTalkButtonState();
    }
    private void UpdateTalkButtonState()
    {
        if (talkToNPCButton == null) return;
        if (defaultNPC == null)
        {
            defaultNPC = FindObjectOfType<NPC>();
        }
        bool canTalk = (defaultNPC != null);
        talkToNPCButton.interactable = canTalk;
        var colors = talkToNPCButton.colors;
        if (canTalk)
        {
            colors.normalColor = Color.white;
            colors.disabledColor = Color.gray;
        }
        else
        {
            colors.normalColor = Color.yellow;
            colors.disabledColor = Color.yellow * 0.7f;
        }
        talkToNPCButton.colors = colors;
    }
    public void CloseInfoPanel()
    {
        petsInfoPanel.SetActive(false);
    }
    public void OnCloseButtonClicked()
    {
        CloseInfoPanel();
        Debug.Log("Pet info panel closed via close button");
    }
    public bool IsPanelActive()
    {
        return petsInfoPanel != null && petsInfoPanel.activeSelf;
    }
    private void DisplayPetDetails(int petID)
    {
        try
        {
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            if (currentUser != null)
            {
                PlayerPet petDetails = APIPlayerPet.GetPlayerPetById(petID);
                if (petDetails != null && petDetails.playerID == currentUser.id)
                {
                    UpdatePetInfo(petDetails);
                    Debug.Log($"Pet {petID} status: {petDetails.status}");
                }
                else
                {
                    Debug.LogWarning($"Pet does not belong to the current user or pet details not found. Pet ID: {petID}, User ID: {currentUser.id}");
                }
            }
            else
            {
                Debug.LogWarning("No user is currently logged in.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error displaying pet details: " + ex.Message);
        }
    }
    private void UpdatePetInfo(PlayerPet petDetails)
    {
        if (petDetails == null) return;
        currentPetDetails = petDetails;
        if (petNameText != null)
            petNameText.text = petDetails.petCustomName;

      

        if (petAdoptedDateText != null)
            petAdoptedDateText.text = "Adopted: " + petDetails.adoptedAt.ToString("MM/dd/yyyy");
        if (petCustomNameText != null)
            petCustomNameText.text = "Custom Name: " + petDetails.petCustomName;
        if (petImage != null && petSprites != null && petDetails.petID >= 0 && petDetails.petID < petSprites.Length)
        {
            petImage.sprite = petSprites[petDetails.petID];
        }
        if (statusBarManager != null)
        {
           
            statusBarManager.UpdatePetStatus(petDetails.status);
        }
        UpdateCareButtonStates();
        UpdateTalkButtonState(); 

      
        if (GlobalPetStatusManager.Instance != null)
        {
            GlobalPetStatusManager.Instance.RegisterPetForDecay(petDetails.playerPetID);
        }
    }

    
    private void OnGlobalPetStatusDecayed(int petID, string newStatus)
    {
        if (currentPetDetails != null && currentPetDetails.playerPetID == petID)
        {
            currentPetDetails.status = newStatus;
            
            if (statusBarManager != null)
            {
                statusBarManager.UpdatePetStatus(newStatus);
            }
            
            UpdateCareButtonStates();
            UpdateTalkButtonState();
        }
    }
    public void FeedPet()
    {
        if (currentPetDetails == null) return;

        bool petWasAwakened = false;
        if (PetSleepManager.Instance != null)
        {
            petWasAwakened = PetSleepManager.Instance.WakeUpPetForFeeding(currentPetDetails.playerPetID);
            if (petWasAwakened)
            {
                ShowStatusMessage("Pet woke up to eat! 🍎😊", Color.cyan);
            }
        }

        ActionBlockReason blockReason = CanPerformAction(PetAction.ActionType.Feed);
        if (blockReason != ActionBlockReason.None)
        {
            string message = GetBlockReasonMessage(blockReason, PetAction.ActionType.Feed);
            Debug.LogWarning(message);
            ShowStatusMessage(message, Color.red);
            return;
        }
        if (useActionSystem && actionManager != null)
        {
            actionManager.FeedPet(feedIncreaseAmount);
        }
        else
        {
            UpdatePetStatus(0, feedIncreaseAmount);
        }

        Debug.Log($"Fed pet {currentPetDetails.playerPetID}, increasing hunger by {feedIncreaseAmount}{(petWasAwakened ? " (pet was awakened)" : "")}");
        ShowStatusMessage("Pet has been fed! 🍎", Color.green);
    }
    public void PlayWithPet()
    {
        if (currentPetDetails == null) return;

        bool petWasAwakened = false;
        if (PetSleepManager.Instance != null)
        {
            petWasAwakened = PetSleepManager.Instance.WakeUpPetForCareAction(currentPetDetails.playerPetID, PetAction.ActionType.Play);
            if (petWasAwakened)
            {
                ShowStatusMessage("Pet woke up to play! 🎾😊", Color.cyan);
            }
        }

        ActionBlockReason blockReason = CanPerformAction(PetAction.ActionType.Play);
        if (blockReason != ActionBlockReason.None)
        {
            string message = GetBlockReasonMessage(blockReason, PetAction.ActionType.Play);
            Debug.LogWarning(message);
            ShowStatusMessage(message, Color.red);
            return;
        }
        if (IsHappinessAtMax())
        {
            Debug.Log($"Pet {currentPetDetails.playerPetID} is already completely happy");
            ShowStatusMessage("Pet is already very happy! ??", Color.yellow);
            return;
        }
        if (useActionSystem && actionManager != null)
        {
            actionManager.PlayWithPet(playIncreaseAmount);
        }
        else
        {
            UpdatePetStatus(1, playIncreaseAmount);
        }

        Debug.Log($"Played with pet {currentPetDetails.playerPetID}, increasing happiness by {playIncreaseAmount}{(petWasAwakened ? " (pet was awakened)" : "")}");
        ShowStatusMessage("Pet enjoyed playing! 🎾", Color.green);
    }
    public void SleepPet()
    {
        if (currentPetDetails == null) return;
        ActionBlockReason blockReason = CanPerformAction(PetAction.ActionType.Sleep);
        if (blockReason != ActionBlockReason.None)
        {
            string message = GetBlockReasonMessage(blockReason, PetAction.ActionType.Sleep);
            Debug.LogWarning(message);
            ShowStatusMessage(message, Color.red);
            return;
        }
        if (IsEnergyAtMax())
        {
            Debug.Log($"Pet {currentPetDetails.playerPetID} is already full of energy");
            ShowStatusMessage("Pet is already fully energized! ?", Color.yellow);
            return;
        }
        if (useActionSystem && actionManager != null)
        {
            actionManager.PetSleep(currentPetDetails.playerPetID, sleepIncreaseAmount);
        }
        else
        {
            UpdatePetStatus(2, sleepIncreaseAmount);
        }
        Debug.Log($"Pet {currentPetDetails.playerPetID} slept, increasing energy by {sleepIncreaseAmount}");
        ShowStatusMessage("Pet had a good rest! ??", Color.green);
    }
    public void ScheduleComplexCare()
    {
        if (!useActionSystem || actionManager == null)
        {
            Debug.LogWarning("Action system not available, using direct care");
            OnCareForAllButtonClicked();
            return;
        }
        string sequenceId = $"complex_care_{Time.time}";
        var checkStatusAction = new PetAction($"{sequenceId}_check", PetAction.ActionType.StatusDecay, PetAction.ActionPriority.High);
        var feedAction = new PetAction($"{sequenceId}_feed", PetAction.ActionType.Feed, PetAction.ActionPriority.Normal);
        feedAction.SetParameter("amount", feedIncreaseAmount);
        feedAction.AddDependency(checkStatusAction.actionId);
        var playAction = new PetAction($"{sequenceId}_play", PetAction.ActionType.Play, PetAction.ActionPriority.Normal);
        playAction.SetParameter("amount", playIncreaseAmount);
        playAction.AddDependency(feedAction.actionId);
        var sleepAction = new PetAction($"{sequenceId}_sleep", PetAction.ActionType.Sleep, PetAction.ActionPriority.Normal);
        sleepAction.SetParameter("amount", sleepIncreaseAmount);
        sleepAction.AddDependency(playAction.actionId);
        var updateDbAction = new PetAction($"{sequenceId}_update", PetAction.ActionType.UpdateDatabase, PetAction.ActionPriority.Critical);
        updateDbAction.AddDependency(sleepAction.actionId);
        actionManager.AddAction(checkStatusAction);
        actionManager.AddAction(feedAction);
        actionManager.AddAction(playAction);
        actionManager.AddAction(sleepAction);
        actionManager.AddAction(updateDbAction);
        Debug.Log("Scheduled complex care sequence with dependencies");
    }
    public void ScheduleEmergencyCare()
    {
        if (!useActionSystem || actionManager == null) return;
        string emergencyId = $"emergency_{Time.time}";
        if (!IsHungerAtMax())
        {
            var emergencyFeed = new PetAction($"{emergencyId}_feed", PetAction.ActionType.Feed, PetAction.ActionPriority.Critical);
            emergencyFeed.SetParameter("amount", feedIncreaseAmount * 2);
            actionManager.AddAction(emergencyFeed);
        }
        if (!IsHappinessAtMax())
        {
            var emergencyPlay = new PetAction($"{emergencyId}_play", PetAction.ActionType.Play, PetAction.ActionPriority.Critical);
            emergencyPlay.SetParameter("amount", playIncreaseAmount * 2);
            actionManager.AddAction(emergencyPlay);
        }
        if (!IsEnergyAtMax())
        {
            var emergencySleep = new PetAction($"{emergencyId}_sleep", PetAction.ActionType.Sleep, PetAction.ActionPriority.Critical);
            emergencySleep.SetParameter("amount", sleepIncreaseAmount * 2);
            actionManager.AddAction(emergencySleep);
        }
        Debug.Log("Scheduled emergency care with high priority");
    }
    public void ScheduleMaintenanceCare(bool feedNeeded = true, bool playNeeded = true, bool sleepNeeded = true)
    {
        if (!useActionSystem || actionManager == null) return;
        string maintenanceId = $"maintenance_{Time.time}";
        var actions = new List<PetAction>();
        if (feedNeeded && !IsHungerAtMax())
        {
            var feedAction = new PetAction($"{maintenanceId}_feed", PetAction.ActionType.Feed);
            feedAction.SetParameter("amount", feedIncreaseAmount);
            actions.Add(feedAction);
        }
        if (playNeeded && !IsHappinessAtMax())
        {
            var playAction = new PetAction($"{maintenanceId}_play", PetAction.ActionType.Play);
            playAction.SetParameter("amount", playIncreaseAmount);
            actions.Add(playAction);
        }
        if (sleepNeeded && !IsEnergyAtMax())
        {
            var sleepAction = new PetAction($"{maintenanceId}_sleep", PetAction.ActionType.Sleep);
            sleepAction.SetParameter("amount", sleepIncreaseAmount);
            actions.Add(sleepAction);
        }
        if (actions.Count > 0)
        {
            var updateAction = new PetAction($"{maintenanceId}_update", PetAction.ActionType.UpdateDatabase, PetAction.ActionPriority.Normal);
            foreach (var action in actions)
            {
                updateAction.AddDependency(action.actionId);
            }
            foreach (var action in actions)
            {
                actionManager.AddAction(action);
            }
            actionManager.AddAction(updateAction);
        }
        Debug.Log($"Scheduled maintenance care for {actions.Count} statuses");
    }
    public void ScheduleSmartCare()
    {
        if (!useActionSystem || actionManager == null)
        {
            PerformDirectSmartCare();
            return;
        }
        var (hunger, happiness, energy) = GetCurrentStatusValues();
        string smartCareId = $"smart_care_{Time.time}";
        var actions = new List<PetAction>();
        if (hunger <= criticalThreshold)
        {
            var emergencyFeed = new PetAction($"{smartCareId}_emergency_feed",
                PetAction.ActionType.Feed, PetAction.ActionPriority.Critical);
            emergencyFeed.SetParameter("amount", feedIncreaseAmount * 2);
            actions.Add(emergencyFeed);
        }
        if (hunger < minHungerForPlay && hunger > criticalThreshold)
        {
            var feedAction = new PetAction($"{smartCareId}_feed",
                PetAction.ActionType.Feed, PetAction.ActionPriority.High);
            feedAction.SetParameter("amount", feedIncreaseAmount);
            actions.Add(feedAction);
        }
        if (happiness < maxStatusValue)
        {
            var playAction = new PetAction($"{smartCareId}_play",
                PetAction.ActionType.Play, PetAction.ActionPriority.Normal);
            playAction.SetParameter("amount", playIncreaseAmount);
            if (hunger < minHungerForPlay)
            {
                var dependentFeedAction = actions.Find(a => a.type == PetAction.ActionType.Feed);
                if (dependentFeedAction != null)
                {
                    playAction.AddDependency(dependentFeedAction.actionId);
                }
            }
            actions.Add(playAction);
        }
        if (energy < maxStatusValue)
        {
            var sleepAction = new PetAction($"{smartCareId}_sleep",
                PetAction.ActionType.Sleep, PetAction.ActionPriority.Normal);
            sleepAction.SetParameter("amount", sleepIncreaseAmount);
            if (hunger < minHungerForSleep)
            {
                var dependentFeedAction = actions.Find(a => a.type == PetAction.ActionType.Feed);
                if (dependentFeedAction != null)
                {
                    sleepAction.AddDependency(dependentFeedAction.actionId);
                }
            }
            var playAction = actions.Find(a => a.type == PetAction.ActionType.Play);
            if (playAction != null)
            {
                sleepAction.AddDependency(playAction.actionId);
            }
            actions.Add(sleepAction);
        }
        if (actions.Count > 0)
        {
            var updateAction = new PetAction($"{smartCareId}_update",
                PetAction.ActionType.UpdateDatabase, PetAction.ActionPriority.Critical);
            foreach (var action in actions)
            {
                updateAction.AddDependency(action.actionId);
            }
            actions.Add(updateAction);
        }
        foreach (var action in actions)
        {
            actionManager.AddAction(action);
        }
        Debug.Log($"Scheduled smart care with {actions.Count} actions and proper dependencies");
        ShowStatusMessage($"Scheduled {actions.Count - 1} care actions in optimal order!", Color.blue);
    }
    private void PerformDirectSmartCare()
    {
        var (hunger, happiness, energy) = GetCurrentStatusValues();
        var actionsPerformed = new List<string>();
        if (hunger <= criticalThreshold ||
            (hunger < minHungerForPlay && happiness < maxStatusValue) ||
            (hunger < minHungerForSleep && energy < maxStatusValue))
        {
            if (CanPerformAction(PetAction.ActionType.Feed) == ActionBlockReason.None)
            {
                UpdatePetStatus(0, feedIncreaseAmount);
                actionsPerformed.Add("Fed");
                (hunger, _, _) = GetCurrentStatusValues();
            }
        }
        if (happiness < maxStatusValue &&
            CanPerformAction(PetAction.ActionType.Play) == ActionBlockReason.None)
        {
            UpdatePetStatus(1, playIncreaseAmount);
            actionsPerformed.Add("Played");
        }
        if (energy < maxStatusValue &&
            CanPerformAction(PetAction.ActionType.Sleep) == ActionBlockReason.None)
        {
            UpdatePetStatus(2, sleepIncreaseAmount);
            actionsPerformed.Add("Slept");
        }
        if (actionsPerformed.Count > 0)
        {
            string message = $"Smart care completed: {string.Join(", ", actionsPerformed)}";
            Debug.Log(message);
            ShowStatusMessage(message, Color.green);
        }
        else
        {
            ShowStatusMessage("Pet doesn't need care right now or some dependencies aren't met!", Color.yellow);
        }
    }
    public void UpdatePetStatus(int statusIndex, int increaseAmount)
    {
        if (currentPetDetails == null) return;

        bool isPetSleeping = false;
        if (PetSleepManager.Instance != null)
        {
            isPetSleeping = PetSleepManager.Instance.IsPetSleeping(currentPetDetails.playerPetID);
        }

        string[] statusValues = currentPetDetails.status.Split('%');
        if (statusValues.Length < 3)
        {
            Debug.LogError($"Invalid status format: {currentPetDetails.status}");
            return;
        }
        int statusValue;
        if (!int.TryParse(statusValues[statusIndex], out statusValue))
        {
            Debug.LogError($"Failed to parse status value: {statusValues[statusIndex]}");
            return;
        }
        statusValue += increaseAmount;
        statusValue = Mathf.Min(statusValue, maxStatusValue);
        statusValues[statusIndex] = statusValue.ToString();
        string newStatus = string.Join("%", statusValues);
        currentPetDetails.status = newStatus;
        if (statusBarManager != null)
        {
            statusBarManager.UpdatePetStatus(newStatus);
        }
        UpdateCareButtonStates();
        UpdateTalkButtonState(); 

        if (!isPetSleeping)
        {
            StartCoroutine(UpdatePetInDatabase());
        }
        else
        {
            Debug.Log($"Pet {currentPetDetails.playerPetID} is sleeping - database update from UpdatePetStatus skipped to prevent conflicts");
        }

        Debug.Log($"Updated pet status to: {newStatus}");
    }
    public void OnFeedButtonClicked()
    {
        if (feedingPanel != null && feedingPanel.activeSelf)
        {
            if (useActionSystem && actionManager != null)
            {
                var feedAction = new PetAction($"panel_feed_{Time.time}", PetAction.ActionType.Feed, PetAction.ActionPriority.High);
                feedAction.SetParameter("amount", pendingFeedAmount);
                actionManager.AddAction(feedAction);
            }
            else
            {
                UpdatePetStatus(0, pendingFeedAmount);
            }
            feedingPanel.SetActive(false);
            Debug.Log($"Pet fed with {pendingFeedAmount} amount of food");
        }
        else
        {
            FeedPet();
        }
    }
    public void OnPlayButtonClicked()
    {
        PlayWithPet();
    }
    public void OnSleepButtonClicked()
    {
        SleepPet();
    }
    public void OnCareForAllButtonClicked()
    {
        if (currentPetDetails == null) return;
        ScheduleSmartCare();
        Debug.Log($"Initiated smart care sequence for pet {currentPetDetails.playerPetID}");
    }
    public void OnEmergencyCareButtonClicked()
    {
        var (hunger, happiness, energy) = GetCurrentStatusValues();
        if (hunger <= criticalThreshold || energy <= criticalThreshold)
        {
            UpdatePetStatus(0, feedIncreaseAmount * 2);
            ShowStatusMessage("Emergency feeding performed! ??", Color.red);
            ScheduleSmartCare();
        }
        else
        {
            ShowStatusMessage("Pet is not in critical condition.", Color.yellow);
        }
    }
    public void OnComplexCareButtonClicked()
    {
        ScheduleComplexCare();
    }
    public void OnSmartCareButtonClicked()
    {
        ScheduleSmartCare();
    }
    public void ToggleActionSystem(bool enabled)
    {
        useActionSystem = enabled;
        Debug.Log($"Action system {(enabled ? "enabled" : "disabled")}");
    }
    public void ClearAllActions()
    {
        if (actionManager != null)
        {
            actionManager.ClearAllActions();
            Debug.Log("Cleared all pending actions");
        }
    }
    private System.Collections.IEnumerator StatusDecayCoroutine()
    {
        yield return new WaitForSeconds(decayInterval);
        while (true)
        {
            if (currentPetDetails != null)
            {
                bool isPetSleeping = false;
                if (PetSleepManager.Instance != null)
                {
                    isPetSleeping = PetSleepManager.Instance.IsPetSleeping(currentPetDetails.playerPetID);
                }

                bool wasDecayed = false;
                string[] statusValues = currentPetDetails.status.Split('%');
                if (statusValues.Length >= 3)
                {
                    int hunger, happiness, energy;
                    if (int.TryParse(statusValues[0], out hunger) &&
                        int.TryParse(statusValues[1], out happiness) &&
                        int.TryParse(statusValues[2], out energy))
                    {
                        if (hunger > minStatusValue)
                        {
                            hunger -= hungerDecayAmount;
                            wasDecayed = true;
                        }
                        if (happiness > minStatusValue)
                        {
                            happiness -= happinessDecayAmount;
                            wasDecayed = true;
                        }

                        if (!isPetSleeping && energy > minStatusValue)
                        {
                            energy -= energyDecayAmount;
                            wasDecayed = true;
                        }
                        if (wasDecayed)
                        {
                            string newStatus = $"{hunger}%{happiness}%{energy}";
                            currentPetDetails.status = newStatus;
                            if (statusBarManager != null)
                            {
                                statusBarManager.UpdatePetStatus(newStatus);
                            }
                            UpdateCareButtonStates();
                            UpdateTalkButtonState(); 

                         
                            if (!isPetSleeping)
                            {
                                StartCoroutine(UpdatePetInDatabase());
                            }
                            else
                            {
                                Debug.Log($"Pet {currentPetDetails.playerPetID} is sleeping - database update skipped to prevent conflicts");
                            }

                           
                            if (isPetSleeping)
                            {
                                Debug.Log($"Pet {currentPetDetails.playerPetID} status decayed (sleeping - energy preserved, DB update skipped): {newStatus}");
                            }
                            else
                            {
                                Debug.Log($"Pet {currentPetDetails.playerPetID} status decayed: {newStatus}");
                            }
                        }
                        else if (isPetSleeping)
                        {
                            Debug.Log($"Pet {currentPetDetails.playerPetID} is sleeping - energy decay prevented, DB update skipped");
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to parse status values: " + currentPetDetails.status);
                    }
                }
                else
                {
                    Debug.LogError("Invalid status format: " + currentPetDetails.status);
                }
            }
            yield return new WaitForSeconds(decayInterval);
        }
    }
    public System.Collections.IEnumerator UpdatePetInDatabase()
    {
        if (currentPetDetails != null)
        {
            yield return APIPlayerPet.UpdatePlayerPetCoroutine(currentPetDetails, success =>
            {
                if (success)
                {
                    Debug.Log($"Successfully updated pet {currentPetDetails.playerPetID} in database");
                }
                else
                {
                    Debug.LogError($"Failed to update pet {currentPetDetails.playerPetID} in database");
                }
            });
        }
    }
    private void StartDecaySystem()
    {
        StopDecaySystem();
        decayCoroutine = StartCoroutine(StatusDecayCoroutine());
        Debug.Log("Started pet status decay system");
    }
    private void StopDecaySystem()
    {
        if (decayCoroutine != null)
        {
            StopCoroutine(decayCoroutine);
            decayCoroutine = null;
            Debug.Log("Stopped pet status decay system");
        }
    }
    private void UpdateCareButtonStates()
    {
        Button[] buttons = petsInfoPanel.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.name.Contains("Feed") || button.tag == "FeedButton")
            {
                ActionBlockReason reason = CanPerformAction(PetAction.ActionType.Feed);
                button.interactable = (reason == ActionBlockReason.None);
                UpdateButtonVisual(button, reason, PetAction.ActionType.Feed);
            }
            else if (button.name.Contains("Play") || button.tag == "PlayButton")
            {
                ActionBlockReason reason = CanPerformAction(PetAction.ActionType.Play);
                button.interactable = (reason == ActionBlockReason.None && !IsHappinessAtMax());
                UpdateButtonVisual(button, reason, PetAction.ActionType.Play);
            }
            else if (button.name.Contains("Sleep") || button.tag == "SleepButton")
            {
                ActionBlockReason reason = CanPerformAction(PetAction.ActionType.Sleep);
                button.interactable = (reason == ActionBlockReason.None);
                UpdateButtonVisual(button, reason, PetAction.ActionType.Sleep);
            }
            else if (button.name.Contains("CareAll") || button.tag == "CareAllButton")
            {
                button.interactable = !IsAllStatusAtMax();
            }
        }
    }
    private void UpdateButtonVisual(Button button, ActionBlockReason reason, PetAction.ActionType actionType)
    {
        var colors = button.colors;
        switch (reason)
        {
            case ActionBlockReason.None:
                colors.normalColor = Color.white;
                colors.disabledColor = Color.gray;
                break;
            case ActionBlockReason.Critical:
                colors.normalColor = Color.red;
                colors.disabledColor = Color.red * 0.7f;
                break;
            case ActionBlockReason.TooHungry:
            case ActionBlockReason.TooTired:
                colors.normalColor = Color.yellow;
                colors.disabledColor = Color.yellow * 0.7f;
                break;
            case ActionBlockReason.TooFull:
            case ActionBlockReason.TooEnergetic:
                colors.normalColor = Color.green;
                colors.disabledColor = Color.green * 0.7f;
                break;
        }
        button.colors = colors;
        UpdateButtonTooltip(button, reason, actionType);
    }
    private void UpdateButtonTooltip(Button button, ActionBlockReason reason, PetAction.ActionType actionType)
    {
        TooltipTrigger tooltip = button.GetComponent<TooltipTrigger>();
        if (tooltip == null)
        {
            tooltip = button.gameObject.AddComponent<TooltipTrigger>();
        }
        tooltip.GetDynamicTooltip = () => GetButtonTooltipText(button, reason, actionType);
        Color bgColor, textColor;
        GetTooltipColors(reason, out bgColor, out textColor);
        tooltip.SetTooltipColors(bgColor, textColor);
    }
    private string GetButtonTooltipText(Button button, ActionBlockReason reason, PetAction.ActionType actionType)
    {
        string baseText = $"{actionType} Pet";
        if (reason == ActionBlockReason.None)
        {
            var (hunger, happiness, energy) = GetCurrentStatusValues();
            string statusInfo = "";
            switch (actionType)
            {
                case PetAction.ActionType.Feed:
                    statusInfo = $"Current Hunger: {hunger}/{maxStatusValue}\nWill increase by: {feedIncreaseAmount}";
                    break;
                case PetAction.ActionType.Play:
                    statusInfo = $"Current Happiness: {happiness}/{maxStatusValue}\nWill increase by: {playIncreaseAmount}";
                    break;
                case PetAction.ActionType.Sleep:
                    statusInfo = $"Current Energy: {energy}/{maxStatusValue}\nWill increase by: {sleepIncreaseAmount}";
                    break;
            }
            return $"{baseText}\n\n{statusInfo}";
        }
        else
        {
            string reasonMessage = GetBlockReasonMessage(reason, actionType);
            return $"{baseText}\n\n? {reasonMessage}";
        }
    }
    private void GetTooltipColors(ActionBlockReason reason, out Color backgroundColor, out Color textColor)
    {
        switch (reason)
        {
            case ActionBlockReason.None:
                backgroundColor = new Color(0.2f, 0.4f, 0.2f, 0.9f);
                textColor = Color.white;
                break;
            case ActionBlockReason.Critical:
                backgroundColor = new Color(0.6f, 0.1f, 0.1f, 0.9f);
                textColor = Color.white;
                break;
            case ActionBlockReason.TooHungry:
            case ActionBlockReason.TooTired:
                backgroundColor = new Color(0.6f, 0.4f, 0.1f, 0.9f);
                textColor = Color.white;
                break;
            case ActionBlockReason.TooFull:
            case ActionBlockReason.TooEnergetic:
            case ActionBlockReason.HappinessAtMax:
                backgroundColor = new Color(0.1f, 0.3f, 0.6f, 0.9f);
                textColor = Color.white;
                break;
            default:
                backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
                textColor = Color.white;
                break;
        }
    }
    public bool IsHungerAtMax()
    {
        if (currentPetDetails == null) return false;
        string[] statusValues = currentPetDetails.status.Split('%');
        if (statusValues.Length >= 1 && int.TryParse(statusValues[0], out int hunger))
        {
            return hunger >= maxStatusValue;
        }
        return false;
    }
    public bool IsHappinessAtMax()
    {
        if (currentPetDetails == null) return false;
        string[] statusValues = currentPetDetails.status.Split('%');
        if (statusValues.Length >= 2 && int.TryParse(statusValues[1], out int happiness))
        {
            return happiness >= maxStatusValue;
        }
        return false;
    }
    public bool IsEnergyAtMax()
    {
        if (currentPetDetails == null) return false;
        string[] statusValues = currentPetDetails.status.Split('%');
        if (statusValues.Length >= 3 && int.TryParse(statusValues[2], out int energy))
        {
            return energy >= maxStatusValue;
        }
        return false;
    }
    public bool IsAllStatusAtMax()
    {
        return IsHungerAtMax() && IsHappinessAtMax() && IsEnergyAtMax();
    }
    [HideInInspector]
    public int pendingFeedAmount = 0;
    public void ShowFeedingPanel(int customCareAmount = 0)
    {
        if (feedingPanel != null)
        {
            pendingFeedAmount = customCareAmount > 0 ? customCareAmount : feedIncreaseAmount;
            feedingPanel.SetActive(true);
            UpdateFeedingPanelUI();
        }
        else
        {
            Debug.LogWarning("Feeding panel is not assigned in the inspector!");
        }
    }
    private void UpdateFeedingPanelUI()
    {
    }
    private System.Collections.IEnumerator HideFeedingPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedingPanel != null)
        {
            feedingPanel.SetActive(false);
        }
    }
    public (int hunger, int happiness, int energy) GetCurrentStatusValues()
    {
        if (currentPetDetails == null || string.IsNullOrEmpty(currentPetDetails.status))
            return (0, 0, 0);
        string[] statusValues = currentPetDetails.status.Split('%');
        if (statusValues.Length < 3) return (0, 0, 0);
        int.TryParse(statusValues[0], out int hunger);
        int.TryParse(statusValues[1], out int happiness);
        int.TryParse(statusValues[2], out int energy);
        return (hunger, happiness, energy);
    }
    public ActionBlockReason CanPerformAction(PetAction.ActionType actionType)
    {
        var (hunger, happiness, energy) = GetCurrentStatusValues();
        if ((hunger <= criticalThreshold || energy <= criticalThreshold) && actionType != PetAction.ActionType.Feed)
        {
            return ActionBlockReason.Critical;
        }
        switch (actionType)
        {
            case PetAction.ActionType.Feed:
                if (hunger >= maxStatusValue)
                    return ActionBlockReason.TooFull;
                break;
            case PetAction.ActionType.Play:
                if (happiness >= maxStatusValue)
                    return ActionBlockReason.HappinessAtMax;
                if (hunger < minHungerForPlay)
                    return ActionBlockReason.TooHungry;
                if (energy < minEnergyForPlay)
                    return ActionBlockReason.TooTired;
                break;
            case PetAction.ActionType.Sleep:
                if (hunger < minHungerForSleep)
                    return ActionBlockReason.TooHungry;
                if (energy >= maxStatusValue)
                    return ActionBlockReason.TooEnergetic;
                break;
        }
        return ActionBlockReason.None;
    }
    public string GetBlockReasonMessage(ActionBlockReason reason, PetAction.ActionType actionType)
    {
        switch (reason)
        {
            case ActionBlockReason.TooHungry:
                return $"Pet is too hungry to {actionType.ToString().ToLower()}! Feed it first (minimum {(actionType == PetAction.ActionType.Play ? minHungerForPlay : minHungerForSleep)} hunger).";
            case ActionBlockReason.TooTired:
                return $"Pet is too tired to play! Let it sleep first (minimum {minEnergyForPlay} energy).";
            case ActionBlockReason.TooFull:
                return "Pet is already full and doesn't need food right now.";
            case ActionBlockReason.TooEnergetic:
                return "Pet has too much energy to sleep effectively. Play with it first!";
            case ActionBlockReason.Critical:
                return "?? CRITICAL: Pet is in emergency condition! Feed it immediately before doing anything else!";
            case ActionBlockReason.HappinessAtMax:
                return "Pet is already completely happy! ?? No need to play right now.";
            default:
                return "";
        }
    }
    public void ShowStatusMessage(string message, Color color)
    {
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
    }
    public (int playerPetId, int playerId) GetCurrentPetAndPlayerId()
    {
        if (currentPetDetails != null)
        {
            return (currentPetDetails.playerPetID, currentPetDetails.playerID);
        }
        User currentUser = PlayerInfomation.LoadPlayerInfo();
        if (currentUser != null)
        {
            return (currentPetID, currentUser.id);
        }
        return (-1, -1);
    }
    public void FeedPetWithHistory()
    {
        this.RecordFeedingAction();
    }
    public void PlayWithPetWithHistory()
    {
        PlayWithPet();
        this.RecordPlayingAction();
    }
    public void SleepPetWithHistory()
    {
        this.RecordSleepingAction();
    }
    public void OnFeedButtonClickedWithHistory()
    {
        if (feedingPanel != null && feedingPanel.activeSelf)
        {
            if (useActionSystem && actionManager != null)
            {
                var feedAction = new PetAction($"panel_feed_{Time.time}", PetAction.ActionType.Feed, PetAction.ActionPriority.High);
                feedAction.SetParameter("amount", pendingFeedAmount);
                actionManager.AddAction(feedAction);
            }
            else
            {
                UpdatePetStatus(0, pendingFeedAmount);
            }
            feedingPanel.SetActive(false);
            this.RecordFeedingAction();
            Debug.Log($"Pet fed with {pendingFeedAmount} amount of food - History recorded");
        }
        else
        {
            FeedPetWithHistory();
        }
    }
    public void OnPlayButtonClickedWithHistory()
    {
        PlayWithPetWithHistory();
    }
    public void OnSleepButtonClickedWithHistory()
    {
        SleepPetWithHistory();
    }
}