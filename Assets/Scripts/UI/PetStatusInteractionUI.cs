using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles UI interactions for feeding, playing with, and sleeping pets
/// </summary>
public class PetStatusInteractionUI : MonoBehaviour
{
    [Header("Button Prefab")]
    public GameObject buttonPrefab; // Reference to your single button prefab
    public Transform buttonContainer; // Parent transform to hold the buttons
    
    [Header("Status Increase Settings")]
    [Range(1f, 50f)]
    public float feedAmount = 20f;
    
    [Range(1f, 50f)]
    public float playAmount = 15f;
    
    [Range(1f, 50f)]
    public float sleepAmount = 10f;
    
    [Header("Cooldown Settings")]
    [Range(5f, 600f)]
    public float actionCooldown = 300f; // 5 minutes
    
    [Header("Effects")]
    public ParticleSystem feedEffect;
    public ParticleSystem playEffect;
    public ParticleSystem sleepEffect;
    
    [Header("Dialogue Integration")]
    public bool useDialogueForInteractions = true; // Set to true by default now
    
    // Reference to buttons (will be assigned at runtime)
    private Button feedButton;
    private Button playButton;
    private Button sleepButton;
    
    // References to text components
    private TMP_Text feedButtonText;
    private TMP_Text playButtonText;
    private TMP_Text sleepButtonText;
    
    // References to cooldown elements
    private List<TMP_Text> cooldownTexts = new List<TMP_Text>();
    private List<Image> cooldownOverlays = new List<Image>();
    
    // Current pet being displayed
    private int currentPetID = -1;
    
    // Reference to the pet status manager
    private PetStatusManager statusManager;
    
    // Reference to dialogue controller
    private DialogueController dialogueController;
    
    // Cooldown tracking
    private Dictionary<string, Dictionary<int, float>> cooldowns = new Dictionary<string, Dictionary<int, float>>()
    {
        {"feed", new Dictionary<int, float>()},
        {"play", new Dictionary<int, float>()},
        {"sleep", new Dictionary<int, float>()}
    };
    
    // Button original colors
    private Color feedOriginalColor;
    private Color playOriginalColor;
    private Color sleepOriginalColor;
    
    private void Start()
    {
        // Find the PetStatusManager in the scene
        statusManager = Object.FindAnyObjectByType<PetStatusManager>();
        
        if (statusManager == null)
        {
            Debug.LogWarning("PetStatusManager not found in scene. Creating one...");
            GameObject managerObj = new GameObject("PetStatusManager");
            statusManager = managerObj.AddComponent<PetStatusManager>();
        }
        
        // Find dialogue controller
        dialogueController = DialogueController.Instance;
        if (dialogueController == null && useDialogueForInteractions)
        {
            Debug.LogWarning("DialogueController not found, but dialogue interactions are enabled.");
        }
        
        // Create buttons from the prefab
        CreateButtons();
    }
    
    private void CreateButtons()
    {
        if (buttonPrefab == null || buttonContainer == null)
        {
            Debug.LogError("Button prefab or container not assigned!");
            return;
        }
        
        // Clear any existing buttons
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create Feed Button
        GameObject feedObj = Instantiate(buttonPrefab, buttonContainer);
        feedObj.name = "FeedButton";
        feedButton = feedObj.GetComponent<Button>();
        feedButtonText = feedObj.GetComponentInChildren<TMP_Text>();
        if (feedButtonText != null)
            feedButtonText.text = $"Feed (+{feedAmount}%)";
        feedButton.onClick.AddListener(OnFeedButtonClicked);
        
        // Create Play Button
        GameObject playObj = Instantiate(buttonPrefab, buttonContainer);
        playObj.name = "PlayButton";
        playButton = playObj.GetComponent<Button>();
        playButtonText = playObj.GetComponentInChildren<TMP_Text>();
        if (playButtonText != null)
            playButtonText.text = $"Play (+{playAmount}%)";
        playButton.onClick.AddListener(OnPlayButtonClicked);
        
        // Create Sleep Button
        GameObject sleepObj = Instantiate(buttonPrefab, buttonContainer);
        sleepObj.name = "SleepButton";
        sleepButton = sleepObj.GetComponent<Button>();
        sleepButtonText = sleepObj.GetComponentInChildren<TMP_Text>();
        if (sleepButtonText != null)
            sleepButtonText.text = $"Sleep (+{sleepAmount}%)";
        sleepButton.onClick.AddListener(OnSleepButtonClicked);
        
        // Store original colors
        if (feedButton.GetComponent<Image>() != null)
            feedOriginalColor = feedButton.GetComponent<Image>().color;
        
        if (playButton.GetComponent<Image>() != null)
            playOriginalColor = playButton.GetComponent<Image>().color;
        
        if (sleepButton.GetComponent<Image>() != null)
            sleepOriginalColor = sleepButton.GetComponent<Image>().color;
        
        // Find and store cooldown elements from each button
        SetupCooldownElements(feedObj, 0);
        SetupCooldownElements(playObj, 1);
        SetupCooldownElements(sleepObj, 2);
    }
    
    private void SetupCooldownElements(GameObject buttonObj, int index)
    {
        // Look for cooldown text
        TMP_Text cooldownText = buttonObj.transform.Find("CooldownText")?.GetComponent<TMP_Text>();
        if (cooldownText != null)
        {
            cooldownTexts.Add(cooldownText);
            cooldownText.gameObject.SetActive(false);
        }
        
        // Look for cooldown overlay
        Image cooldownOverlay = buttonObj.transform.Find("CooldownOverlay")?.GetComponent<Image>();
        if (cooldownOverlay != null)
        {
            cooldownOverlays.Add(cooldownOverlay);
            cooldownOverlay.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        // Update cooldown timers
        UpdateCooldowns();
    }
    
    /// <summary>
    /// Set the current pet ID being displayed
    /// </summary>
    public void SetCurrentPet(int petID)
    {
        currentPetID = petID;
        UpdateButtonStates();
        
        // If dialogue interactions are enabled, show the dialogue options
        if (useDialogueForInteractions && dialogueController != null)
        {
            ShowInteractionOptions();
        }
    }
    
    /// <summary>
    /// Shows interaction options using the DialogueController
    /// </summary>
    public void ShowInteractionOptions()
    {
        if (!useDialogueForInteractions || dialogueController == null || currentPetID <= 0)
            return;
            
        // Set up dialogue UI
        PlayerPet pet = statusManager.GetPetById(currentPetID);
        if (pet == null)
            return;
            
        dialogueController.ShowDialogueUI(true);
        dialogueController.SetNPCInfo(pet.petCustomName, null); // You might want to get the pet's portrait here
        dialogueController.SetDialogueText("What would you like to do with " + pet.petCustomName + "?");
        
        // Clear existing choices
        dialogueController.ClearChoices();
        
        // Create dialogue choice buttons
        bool canFeed = !IsOnCooldown("feed", currentPetID);
        bool canPlay = !IsOnCooldown("play", currentPetID);
        bool canSleep = !IsOnCooldown("sleep", currentPetID);
        
        if (canFeed)
        {
            // Use the new CreateChoiceButtonWithEffect if available
            if (dialogueController.GetType().GetMethod("CreateChoiceButtonWithEffect") != null)
            {
                DialogueEffect feedEffect = new DialogueEffect { 
                    effectType = EffectType.Hunger, 
                    amount = feedAmount 
                };
                
                dialogueController.CreateChoiceButtonWithEffect($"Feed (+{feedAmount}%)", () => {
                    SetCooldown("feed", currentPetID);
                    dialogueController.ShowDialogueUI(false);
                    
                    // Play feeding effect if available
                    if (this.feedEffect != null)
                        this.feedEffect.Play();
                }, feedEffect, currentPetID);
            }
            else
            {
                // Fallback to original method
                dialogueController.CreateChoiceButton($"Feed (+{feedAmount}%)", () => {
                    OnFeedButtonClicked();
                    dialogueController.ShowDialogueUI(false);
                });
            }
        }
        
        if (canPlay)
        {
            // Use the new CreateChoiceButtonWithEffect if available
            if (dialogueController.GetType().GetMethod("CreateChoiceButtonWithEffect") != null)
            {
                DialogueEffect playEffect = new DialogueEffect { 
                    effectType = EffectType.Happiness, 
                    amount = playAmount 
                };
                
                dialogueController.CreateChoiceButtonWithEffect($"Play (+{playAmount}%)", () => {
                    SetCooldown("play", currentPetID);
                    dialogueController.ShowDialogueUI(false);
                    
                    // Play effect if available
                    if (this.playEffect != null)
                        this.playEffect.Play();
                }, playEffect, currentPetID);
            }
            else
            {
                // Fallback to original method
                dialogueController.CreateChoiceButton($"Play (+{playAmount}%)", () => {
                    OnPlayButtonClicked();
                    dialogueController.ShowDialogueUI(false);
                });
            }
        }
        
        if (canSleep)
        {
            // Use the new CreateChoiceButtonWithEffect if available
            if (dialogueController.GetType().GetMethod("CreateChoiceButtonWithEffect") != null)
            {
                DialogueEffect sleepEffect = new DialogueEffect { 
                    effectType = EffectType.Energy, 
                    amount = sleepAmount 
                };
                
                dialogueController.CreateChoiceButtonWithEffect($"Sleep (+{sleepAmount}%)", () => {
                    SetCooldown("sleep", currentPetID);
                    dialogueController.ShowDialogueUI(false);
                    
                    // Play sleep effect if available
                    if (this.sleepEffect != null)
                        this.sleepEffect.Play();
                }, sleepEffect, currentPetID);
            }
            else
            {
                // Fallback to original method
                dialogueController.CreateChoiceButton($"Sleep (+{sleepAmount}%)", () => {
                    OnSleepButtonClicked();
                    dialogueController.ShowDialogueUI(false);
                });
            }
        }
        
        // Add a cancel option
        dialogueController.CreateChoiceButton("Cancel", () => {
            dialogueController.ShowDialogueUI(false);
        });
    }
    
    /// <summary>
    /// Called when the feed button is clicked
    /// </summary>
    public void OnFeedButtonClicked()
    {
        if (currentPetID <= 0 || IsOnCooldown("feed", currentPetID))
            return;
            
        statusManager.FeedPet(currentPetID, feedAmount);
        SetCooldown("feed", currentPetID);
        
        // Play feeding effect if available
        if (feedEffect != null)
            feedEffect.Play();
            
        UpdateButtonStates();
    }
    
    /// <summary>
    /// Called when the play button is clicked
    /// </summary>
    public void OnPlayButtonClicked()
    {
        if (currentPetID <= 0 || IsOnCooldown("play", currentPetID))
            return;
            
        statusManager.PlayWithPet(currentPetID, playAmount);
        SetCooldown("play", currentPetID);
        
        // Play play effect if available
        if (playEffect != null)
            playEffect.Play();
            
        UpdateButtonStates();
    }
    
    /// <summary>
    /// Called when the sleep button is clicked
    /// </summary>
    public void OnSleepButtonClicked()
    {
        if (currentPetID <= 0 || IsOnCooldown("sleep", currentPetID))
            return;
            
        statusManager.SleepPet(currentPetID, sleepAmount);
        SetCooldown("sleep", currentPetID);
        
        // Play sleep effect if available
        if (sleepEffect != null)
            sleepEffect.Play();
            
        UpdateButtonStates();
    }
    
    /// <summary>
    /// Check if an action is on cooldown for the given pet
    /// </summary>
    private bool IsOnCooldown(string action, int petID)
    {
        if (cooldowns.TryGetValue(action, out Dictionary<int, float> actionCooldowns))
        {
            if (actionCooldowns.TryGetValue(petID, out float timeRemaining))
            {
                return timeRemaining > 0;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Set a cooldown for the given action and pet
    /// </summary>
    private void SetCooldown(string action, int petID)
    {
        if (cooldowns.TryGetValue(action, out Dictionary<int, float> actionCooldowns))
        {
            actionCooldowns[petID] = actionCooldown;
        }
    }
    
    /// <summary>
    /// Update all cooldown timers
    /// </summary>
    private void UpdateCooldowns()
    {
        // Only process if we have a current pet
        if (currentPetID <= 0)
            return;
            
        // Decrease all cooldowns by deltaTime
        foreach (var actionPair in cooldowns)
        {
            string action = actionPair.Key;
            Dictionary<int, float> petCooldowns = actionPair.Value;
            
            if (petCooldowns.TryGetValue(currentPetID, out float timeRemaining))
            {
                // Decrease the cooldown
                timeRemaining -= Time.deltaTime;
                
                // Update the dictionary
                petCooldowns[currentPetID] = Mathf.Max(0, timeRemaining);
            }
        }
        
        // Update UI
        UpdateButtonStates();
    }
    
    /// <summary>
    /// Update button states based on cooldowns
    /// </summary>
    private void UpdateButtonStates()
    {
        if (currentPetID <= 0)
            return;
            
        // Feed button cooldown
        UpdateButtonCooldown(
            "feed", 
            feedButton, 
            cooldownTexts.Count > 0 ? cooldownTexts[0] : null,
            cooldownOverlays.Count > 0 ? cooldownOverlays[0] : null,
            feedOriginalColor
        );
        
        // Play button cooldown
        UpdateButtonCooldown(
            "play", 
            playButton, 
            cooldownTexts.Count > 1 ? cooldownTexts[1] : null,
            cooldownOverlays.Count > 1 ? cooldownOverlays[1] : null,
            playOriginalColor
        );
        
        // Sleep button cooldown
        UpdateButtonCooldown(
            "sleep", 
            sleepButton, 
            cooldownTexts.Count > 2 ? cooldownTexts[2] : null,
            cooldownOverlays.Count > 2 ? cooldownOverlays[2] : null,
            sleepOriginalColor
        );
    }
    
    /// <summary>
    /// Update button cooldown UI and states
    /// </summary>
    private void UpdateButtonCooldown(string action, Button button, TMP_Text cooldownText, Image cooldownOverlay, Color originalColor)
    {
        if (button == null) return;
        
        bool onCooldown = IsOnCooldown(action, currentPetID);
        button.interactable = !onCooldown;
        
        // If on cooldown, show remaining time and dim button
        if (onCooldown)
        {
            float timeLeft = cooldowns[action][currentPetID];
            
            // Update cooldown text if available
            if (cooldownText != null)
            {
                cooldownText.text = FormatTime(timeLeft);
                cooldownText.gameObject.SetActive(true);
            }
            
            // Update cooldown overlay if available
            if (cooldownOverlay != null)
            {
                cooldownOverlay.fillAmount = timeLeft / actionCooldown;
                cooldownOverlay.gameObject.SetActive(true);
            }
            
            // Dim button color
            if (button.GetComponent<Image>() != null)
            {
                Color dimColor = originalColor;
                dimColor.a = 0.5f;
                button.GetComponent<Image>().color = dimColor;
            }
        }
        else
        {
            // Hide cooldown text if available
            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(false);
            }
            
            // Hide cooldown overlay if available
            if (cooldownOverlay != null)
            {
                cooldownOverlay.gameObject.SetActive(false);
            }
            
            // Restore button color
            if (button.GetComponent<Image>() != null)
            {
                button.GetComponent<Image>().color = originalColor;
            }
        }
    }
    
    /// <summary>
    /// Format seconds into a MM:SS display
    /// </summary>
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:00}:{secs:00}";
    }
}