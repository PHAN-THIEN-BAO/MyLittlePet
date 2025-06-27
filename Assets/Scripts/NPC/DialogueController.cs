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
    private int currentPlayerPetID = -1; // Add this line

    private const int maxStatusValue = 100; // Maximum value for pet statuses
    private const int feedIncreaseAmount = 10; // Amount to increase hunger when feeding
    private const int playIncreaseAmount = 10; // Amount to increase happiness when playing
    private const int sleepIncreaseAmount = 10; // Amount to increase energy when sleeping

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
                    disableButton = petInfoManager.IsHungerAtMax(currentPlayerPetID);
                    if (disableButton)
                    {
                        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText + " (Full)";
                    }
                    break;

                case PetCareAction.Play:
                    disableButton = petInfoManager.IsHappinessAtMax(currentPlayerPetID);
                    if (disableButton)
                    {
                        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText + " (Happy)";
                    }
                    break;

                case PetCareAction.Sleep:
                    disableButton = petInfoManager.IsEnergyAtMax(currentPlayerPetID);
                    if (disableButton)
                    {
                        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText + " (Energetic)";
                    }
                    break;

                case PetCareAction.CareForAll:
                    disableButton = petInfoManager.IsAllStatusAtMax(currentPlayerPetID);
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
                int playerId = PlayerInfomation.LoadPlayerInfo().id;
                if (feedingManager != null)
                {
                    // Truyền đủ 3 tham số cho FeedingManager
                    feedingManager.ShowFeedingPanel(playerId, customCareAmount, currentPlayerPetID);
                }
                else
                {
                    // PetInfoUIManager chỉ nhận customCareAmount
                    petInfoManager.ShowFeedingPanel(customCareAmount);
                }
                break;
            case PetCareAction.Play:
                if (customCareAmount > 0)
                    petInfoManager.UpdatePetStatus(1, customCareAmount, currentPlayerPetID);
                else
                    petInfoManager.PlayWithPet(currentPlayerPetID);
                break;
            case PetCareAction.Sleep:
                if (customCareAmount > 0)
                    petInfoManager.UpdatePetStatus(2, customCareAmount, currentPlayerPetID);
                else
                    petInfoManager.SleepPet(currentPlayerPetID);
                break;
            case PetCareAction.CareForAll:
                if (customCareAmount > 0)
                {
                    if (!petInfoManager.IsHungerAtMax(currentPlayerPetID))
                        petInfoManager.UpdatePetStatus(0, customCareAmount, currentPlayerPetID);
                    if (!petInfoManager.IsHappinessAtMax(currentPlayerPetID))
                        petInfoManager.UpdatePetStatus(1, customCareAmount, currentPlayerPetID);
                    if (!petInfoManager.IsEnergyAtMax(currentPlayerPetID))
                        petInfoManager.UpdatePetStatus(2, customCareAmount, currentPlayerPetID);
                }
                else
                {
                    petInfoManager.OnCareForAllButtonClicked(currentPlayerPetID);
                }
                break;
            case PetCareAction.None:
            default:
                break;
        }
    }

    // Call this when starting a dialogue with a specific pet
    public void StartDialogueWithPet(int playerPetID)
    {
        currentPlayerPetID = playerPetID;
        // You can add more logic here if needed, e.g. update pet info UI
    }

    public bool IsHungerAtMax(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return false;
        string[] statusValues = pet.status.Split('%');
        if (statusValues.Length >= 1 && int.TryParse(statusValues[0], out int hunger))
        {
            return hunger >= maxStatusValue;
        }
        return false;
    }

    public bool IsHappinessAtMax(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return false;
        string[] statusValues = pet.status.Split('%');
        if (statusValues.Length >= 2 && int.TryParse(statusValues[1], out int happiness))
        {
            return happiness >= maxStatusValue;
        }
        return false;
    }

    public bool IsEnergyAtMax(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return false;
        string[] statusValues = pet.status.Split('%');
        if (statusValues.Length >= 3 && int.TryParse(statusValues[2], out int energy))
        {
            return energy >= maxStatusValue;
        }
        return false;
    }

    public bool IsAllStatusAtMax(int playerPetID)
    {
        return IsHungerAtMax(playerPetID) && IsHappinessAtMax(playerPetID) && IsEnergyAtMax(playerPetID);
    }

    public void FeedPet(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return;
        if (IsHungerAtMax(playerPetID)) return;
        UpdatePetStatus(0, feedIncreaseAmount, playerPetID);
    }

    public void PlayWithPet(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return;
        if (IsHappinessAtMax(playerPetID)) return;
        UpdatePetStatus(1, playIncreaseAmount, playerPetID);
    }

    public void SleepPet(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return;
        if (IsEnergyAtMax(playerPetID)) return;
        UpdatePetStatus(2, sleepIncreaseAmount, playerPetID);
    }

    public void UpdatePetStatus(int statusIndex, int increaseAmount, int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return;
        string[] statusValues = pet.status.Split('%');
        if (statusValues.Length < 3) return;
        if (!int.TryParse(statusValues[statusIndex], out int statusValue)) return;
        statusValue += increaseAmount;
        statusValue = Mathf.Min(statusValue, maxStatusValue);
        statusValues[statusIndex] = statusValue.ToString();
        string newStatus = string.Join("%", statusValues);
        pet.status = newStatus;
        // Cập nhật database
        StartCoroutine(APIPlayerPet.UpdatePlayerPetCoroutine(pet, null));
    }
}
