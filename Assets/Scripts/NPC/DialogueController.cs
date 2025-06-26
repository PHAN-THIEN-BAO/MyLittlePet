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
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        
        Button button = choiceButton.GetComponent<Button>();
        button.onClick.AddListener(() => PerformPetCareAction(careAction, customCareAmount));
        
        // Add any additional action that should be performed (like advancing dialogue)
        if (additionalAction != null)
        {
            button.onClick.AddListener(additionalAction);
        }
        
        // Disable the button if the corresponding status is already at max
        if (petInfoManager != null)
        {
            bool disableButton = false;
            
            switch (careAction)
            {
                case PetCareAction.Feed:
                    disableButton = petInfoManager.IsHungerAtMax();
                    if (disableButton)
                    {
                        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText + " (Full)";
                    }
                    break;
                    
                case PetCareAction.Play:
                    disableButton = petInfoManager.IsHappinessAtMax();
                    if (disableButton)
                    {
                        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText + " (Happy)";
                    }
                    break;
                    
                case PetCareAction.Sleep:
                    disableButton = petInfoManager.IsEnergyAtMax();
                    if (disableButton)
                    {
                        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText + " (Energetic)";
                    }
                    break;
                    
                case PetCareAction.CareForAll:
                    disableButton = petInfoManager.IsAllStatusAtMax();
                    if (disableButton)
                    {
                        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText + " (Not Needed)";
                    }
                    break;
            }
            
            button.interactable = !disableButton;
        }
        
        return choiceButton;
    }
    
    // Execute the selected pet care action
    private void PerformPetCareAction(PetCareAction action, int customCareAmount = 0)
    {
        if (petInfoManager == null)
        {
            Debug.LogWarning("Cannot perform pet care action: PetInfoUIManager is not found");
            return;
        }
        
        switch (action)
        {
            case PetCareAction.Feed:
                // Show feeding panel and fetch food items
                int playerId = PlayerInfomation.LoadPlayerInfo().id; // Replace with your player ID getter
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
                if (customCareAmount > 0)
                {
                    petInfoManager.UpdatePetStatus(1, customCareAmount); // 1 is happiness index
                    Debug.Log($"Dialogue choice: Play with pet with custom amount: {customCareAmount}");
                }
                else
                {
                    petInfoManager.PlayWithPet();
                    Debug.Log("Dialogue choice: Play with pet");
                }
                break;
                
            case PetCareAction.Sleep:
                if (customCareAmount > 0)
                {
                    petInfoManager.UpdatePetStatus(2, customCareAmount); // 2 is energy index
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
                    if (!petInfoManager.IsHungerAtMax())
                        petInfoManager.UpdatePetStatus(0, customCareAmount); // 0 is hunger index
                    
                    if (!petInfoManager.IsHappinessAtMax())
                        petInfoManager.UpdatePetStatus(1, customCareAmount); // 1 is happiness index
                    
                    if (!petInfoManager.IsEnergyAtMax())
                        petInfoManager.UpdatePetStatus(2, customCareAmount); // 2 is energy index
                    
                    Debug.Log($"Dialogue choice: Care for all pet needs with custom amount: {customCareAmount}");
                }
                else
                {
                    petInfoManager.OnCareForAllButtonClicked();
                    Debug.Log("Dialogue choice: Care for all pet needs");
                }
                break;
                
            case PetCareAction.None:
            default:
                // No pet care action to perform
                break;
        }
    }
}
