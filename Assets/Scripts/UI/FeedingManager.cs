using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;

public class FeedingManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject feedingPanel;             // The main outer panel
    public Transform foodItemsContainer;        // Container for food item panels
    public GameObject foodItemPrefab;           // Prefab for individual food items
    public Button closeButton;                  // Button to close the feeding panel
    public ScrollRect scrollView;               // Optional scroll view for many food items

    [Header("Panel Layout Settings")]
    [Tooltip("The number of columns in the grid layout")]
    public int gridColumns = 3;
    [Tooltip("Spacing between food item panels")]
    public Vector2 itemSpacing = new Vector2(10f, 10f);
    [Tooltip("Padding around the grid of food items")]
    public RectOffset gridPadding;

    [Header("API Settings")]
    [SerializeField] private string apiBaseUrl = "https://localhost:7035";

    [Header("Feeding Settings")]
    [SerializeField] private int defaultFeedIncreaseAmount = 15;
    [SerializeField] private int expRewardPerFeed = 10; // Experience gained per feeding

    [Header("No Food Message")]
    public GameObject noFoodMessage; // Kéo object này từ Inspector vào

    [Header("Dependency Check Settings")]
    [Tooltip("Check pet status dependencies before feeding")]
    public bool enableDependencyCheck = true;

    [Header("Audio Settings")]
    [Tooltip("Enable feeding sound effects")]
    public bool enableFeedingAudio = true;
    [Tooltip("Sound name for feeding action (must match SoundEffectLibrary)")]
    public string feedingSoundName = "feeding";
    [Tooltip("Enable random pitch variation for feeding sounds")]
    public bool randomPitch = false;
    [Tooltip("Direct audio clip for feeding (fallback if SoundEffectManager not available)")]
    public AudioClip feedingAudioClip;

    [Header("Feeding Effect Settings")]
    [Tooltip("Enable feeding visual effects")]
    public bool enableFeedingEffect = true;
    [Tooltip("Prefab to instantiate above pet when feeding")]
    public GameObject feedingEffectPrefab;
    [Tooltip("Height offset above pet for the effect")]
    public float effectHeightOffset = 1.5f;
    [Tooltip("Duration before destroying the effect")]
    public float effectDuration = 2.0f;
    [Tooltip("Enable random position variation for effect")]
    public bool randomEffectPosition = true;
    [Tooltip("Random position range for effect")]
    public Vector2 effectPositionRange = new Vector2(0.5f, 0.3f);

    // Reference to the PetInfoUIManager to handle feeding effects
    private PetInfoUIManager petInfoManager;

    // Current player ID
    private int currentPlayerId;

    // List of food items
    private List<FoodItem> foodItems = new List<FoodItem>();

    // Start is called before the first frame update
    void Start()
    {
        // Find the PetInfoUIManager
        petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager == null)
        {
            Debug.LogError("PetInfoUIManager not found in the scene. FeedingManager will not work properly.");
        }

        // Set up the close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseFeedingPanel);
        }

        // Initially hide the panel
        if (feedingPanel != null)
        {
            feedingPanel.SetActive(false);
        }

        // Set default gridPadding if not set in Inspector
        if (gridPadding == null)
        {
            gridPadding = new RectOffset(10, 10, 10, 10);
        }

        // No more grid layout configuration here!
        if (foodItemsContainer == null)
        {
            Debug.LogWarning("FeedingManager: foodItemsContainer is not assigned in the Inspector.");
        }
    }

    /// <summary>
    /// Shows the feeding panel and loads food items for the specified player
    /// </summary>
    /// <param name="playerId">The ID of the player whose food items to display</param>
    public void ShowFeedingPanel(int playerId, int customCareAmount = 0)
    {
        Debug.Log($"ShowFeedingPanel called with playerId={playerId}, customCareAmount={customCareAmount}");

        // CHECK DEPENDENCY BEFORE SHOWING PANEL
        if (enableDependencyCheck && petInfoManager != null)
        {
            var blockReason = petInfoManager.CanPerformAction(PetAction.ActionType.Feed);
            if (blockReason != PetInfoUIManager.ActionBlockReason.None)
            {
                string message = petInfoManager.GetBlockReasonMessage(blockReason, PetAction.ActionType.Feed);
                Debug.LogWarning($"Cannot show feeding panel: {message}");

                // Disable the button instead of showing message
                return;
            }
        }

        currentPlayerId = playerId;

        // Set the pending feed amount in the pet info manager
        if (petInfoManager != null)
        {
            petInfoManager.pendingFeedAmount = customCareAmount > 0 ? customCareAmount : defaultFeedIncreaseAmount;
        }

        // Show the panel
        if (feedingPanel != null)
        {
            feedingPanel.SetActive(true);
            Debug.Log("Feeding panel set active. Starting LoadFoodItems coroutine.");
            // Load food items
            StartCoroutine(LoadFoodItems(playerId));
        }
    }

    /// <summary>
    /// Shows message when feeding is blocked due to dependencies
    /// </summary>
    private void ShowFeedingBlockedMessage(string message)
    {
        // You can implement this as a popup, toast, or temporary UI element
        Debug.LogWarning($"🚫 FEEDING BLOCKED: {message}");

        // Example: Show temporary message panel
        StartCoroutine(ShowTemporaryMessage(message));
    }

    private IEnumerator ShowTemporaryMessage(string message)
    {
        // Create temporary message UI
        GameObject messagePanel = new GameObject("FeedingBlockedMessage");
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            messagePanel.transform.SetParent(canvas.transform, false);

            // Add background
            Image bg = messagePanel.AddComponent<Image>();
            bg.color = new Color(1f, 0.2f, 0.2f, 0.8f); // Red background

            // Add text
            GameObject textObj = new GameObject("MessageText");
            textObj.transform.SetParent(messagePanel.transform, false);
            TMP_Text text = textObj.AddComponent<TMP_Text>();
            text.text = message;
            text.color = Color.white;
            text.fontSize = 18;
            text.alignment = TextAlignmentOptions.Center;

            // Set size
            RectTransform rect = messagePanel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 100);
            rect.anchoredPosition = Vector2.zero;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(380, 80);
            textRect.anchoredPosition = Vector2.zero;

            // Auto-close after 3 seconds
            yield return new WaitForSeconds(3f);

            if (messagePanel != null)
                Destroy(messagePanel);
        }
    }

    /// <summary>
    /// Closes the feeding panel
    /// </summary>
    public void CloseFeedingPanel()
    {
        if (feedingPanel != null)
        {
            feedingPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Loads food items from the API
    /// </summary>
    /// <param name="playerId">The ID of the player whose food items to load</param>
    private IEnumerator LoadFoodItems(int playerId)
    {
        string url = $"{apiBaseUrl}/PlayerInventory/FoodItems/{playerId}";
        Debug.Log($"Requesting food items from: {url}");
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Set request headers if needed (e.g., authorization)
            // request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("AuthToken"));

            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading food items: {request.error}");

                // Show error message in the panel
                DisplayErrorMessage($"Error loading food items: {request.error}");
            }
            else
            {
                // Parse the response
                string responseText = request.downloadHandler.text;
                Debug.Log($"Food items API response: {responseText}");
                try
                {
                    foodItems = JsonConvert.DeserializeObject<List<FoodItem>>(responseText);
                    Debug.Log($"Fetched {foodItems?.Count ?? 0} food items from PlayerInventory.");

                    // Populate the UI
                    PopulateFeedingPanel();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing food items: {ex.Message}");
                    DisplayErrorMessage($"Error parsing food items: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Displays an error message in the feeding panel
    /// </summary>
    private void DisplayErrorMessage(string message)
    {
        // Clear existing items
        foreach (Transform child in foodItemsContainer)
        {
            Destroy(child.gameObject);
        }

        // Create error message
        GameObject messageObj = new GameObject("ErrorMessage");
        messageObj.transform.SetParent(foodItemsContainer, false);

        TMP_Text errorText = messageObj.AddComponent<TMP_Text>();
        errorText.text = message;
        errorText.color = Color.red;
        errorText.alignment = TextAlignmentOptions.Center;
        errorText.fontSize = 24;

        RectTransform rect = messageObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 100);
    }

    /// <summary>
    /// Populates the feeding panel with food items
    /// </summary>
    private void PopulateFeedingPanel()
    {
        // Clear existing items
        foreach (Transform child in foodItemsContainer)
        {
            Destroy(child.gameObject);
        }

        // Filter out items with 0 quantity
        var availableFoodItems = foodItems.Where(f => f.Quantity > 0).ToList();

        // Hiển thị hoặc ẩn thông báo "No food"
        if (noFoodMessage != null)
        {
            noFoodMessage.SetActive(availableFoodItems.Count == 0);
        }

        // Tạo các item mới nếu có
        foreach (var foodItem in availableFoodItems)
        {
            GameObject newFoodItemObj = Instantiate(foodItemPrefab, foodItemsContainer);
            FoodItemUI foodItemUI = newFoodItemObj.GetComponent<FoodItemUI>();

            if (foodItemUI != null)
            {
                foodItemUI.Setup(foodItem, OnFoodItemClicked);
            }
            else
            {
                Debug.LogWarning("FoodItemUI component not found on food item prefab.");

                // Fallback basic setup if FoodItemUI is not available
                // Note: This should not happen if prefab is set up correctly
                Image foodImage = newFoodItemObj.GetComponentInChildren<Image>();
                if (foodImage != null)
                {
                    StartCoroutine(LoadFoodImage(foodImage, foodItem.ProductInfo.ImageUrl));
                }

                TMP_Text[] texts = newFoodItemObj.GetComponentsInChildren<TMP_Text>();
                if (texts.Length > 0) texts[0].text = foodItem.ProductInfo.Name;
                if (texts.Length > 1) texts[1].text = $"x{foodItem.Quantity}";
                if (texts.Length > 2 && !string.IsNullOrEmpty(foodItem.ProductInfo.Description))
                    texts[2].text = foodItem.ProductInfo.Description;

                Button useButton = newFoodItemObj.GetComponentInChildren<Button>();
                if (useButton != null)
                {
                    int shopProductId = foodItem.ShopProductId;
                    useButton.onClick.AddListener(() => OnFoodItemClicked(shopProductId));
                }
            }
        }

        // Update scroll view if needed
        if (scrollView != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollView.normalizedPosition = new Vector2(0, 1); // Scroll to top
        }
    }

    /// <summary>
    /// Loads a food item image from a URL
    /// </summary>
    private IEnumerator LoadFoodImage(Image targetImage, string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogWarning("Food item has no image URL");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading food image: {request.error}");
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

                targetImage.sprite = sprite;
            }
        }
    }

    /// <summary>
    /// Enhanced food item click handler với care history recording và audio feedback
    /// </summary>
    private void OnFoodItemClicked(int shopProductId)
    {
        // DOUBLE-CHECK DEPENDENCY BEFORE FEEDING
        if (enableDependencyCheck && petInfoManager != null)
        {
            var blockReason = petInfoManager.CanPerformAction(PetAction.ActionType.Feed);
            if (blockReason != PetInfoUIManager.ActionBlockReason.None)
            {
                string message = petInfoManager.GetBlockReasonMessage(blockReason, PetAction.ActionType.Feed);
                Debug.LogWarning($"Cannot feed pet: {message}");
                return;
            }
        }

        // Find the selected food item
        FoodItem selectedItem = foodItems.Find(item => item.ShopProductId == shopProductId);

        if (selectedItem != null && selectedItem.Quantity > 0)
        {
            Debug.Log($"Feeding pet with {selectedItem.ProductInfo.Name}");

            // ========== PLAY FEEDING AUDIO ==========
            PlayFeedingAudio();

            // ========== INSTANTIATE FEEDING EFFECT ==========
            InstantiateFeedingEffect();

            // Feed the pet
            if (petInfoManager != null)
            {
                petInfoManager.OnFeedButtonClicked();
                
                // ========== RECORD CARE HISTORY ==========
                RecordFeedingHistory();
            }

            // ADD EXPERIENCE FOR FEEDING
            AddExperienceForFeeding();

            // Call API to update inventory (reduce quantity by 1)
            StartCoroutine(UpdateInventory(selectedItem));
        }
    }

    /// <summary>
    /// Instantiates a feeding effect prefab above the pet
    /// </summary>
    private void InstantiateFeedingEffect()
    {
        if (!enableFeedingEffect || feedingEffectPrefab == null)
        {
            Debug.LogWarning("Feeding effect is disabled or prefab is not assigned");
            return;
        }

        // Find the current pet GameObject
        GameObject currentPet = FindCurrentPetGameObject();
        if (currentPet == null)
        {
            Debug.LogWarning("Could not find current pet GameObject for feeding effect");
            return;
        }

        // Calculate effect position above the pet
        Vector3 effectPosition = currentPet.transform.position;
        effectPosition.y += effectHeightOffset;

        // Add random position variation if enabled
        if (randomEffectPosition)
        {
            effectPosition.x += UnityEngine.Random.Range(-effectPositionRange.x, effectPositionRange.x);
            effectPosition.y += UnityEngine.Random.Range(0, effectPositionRange.y);
        }

        // Instantiate the effect prefab
        GameObject effectInstance = Instantiate(feedingEffectPrefab, effectPosition, Quaternion.identity);
        
        // Ensure the effect is rendered in 2D (set Z position to appropriate layer)
        Vector3 pos = effectInstance.transform.position;
        pos.z = currentPet.transform.position.z - 0.1f; // Slightly in front of the pet
        effectInstance.transform.position = pos;

        Debug.Log($"🍎 Instantiated feeding effect at position: {effectPosition}");

        // Auto-destroy the effect after the specified duration
        StartCoroutine(DestroyEffectAfterDelay(effectInstance, effectDuration));
    }

    /// <summary>
    /// Finds the current pet GameObject in the scene
    /// </summary>
    private GameObject FindCurrentPetGameObject()
    {
        // Method 1: Try to find through PetInfoUIManager's current pet data
        if (petInfoManager != null)
        {
            var (playerPetId, playerId) = petInfoManager.GetCurrentPetAndPlayerId();
            if (playerPetId > 0)
            {
                // Look for pet with matching PetDataHolder
                PetDataHolder[] petDataHolders = FindObjectsOfType<PetDataHolder>();
                foreach (var dataHolder in petDataHolders)
                {
                    if (dataHolder.petData != null && dataHolder.petData.playerPetID == playerPetId)
                    {
                        return dataHolder.gameObject;
                    }
                }
            }
        }

        // Method 2: Fallback - find the first active pet in the scene
        PetDataHolder firstPet = FindObjectOfType<PetDataHolder>();
        if (firstPet != null)
        {
            return firstPet.gameObject;
        }

        // Method 3: Look for objects with "Pet" tag
        GameObject taggedPet = GameObject.FindGameObjectWithTag("Pet");
        if (taggedPet != null)
        {
            return taggedPet;
        }

        // Method 4: Look for objects with specific naming pattern
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("pet") && obj.activeInHierarchy)
            {
                return obj;
            }
        }

        Debug.LogWarning("Could not find any pet GameObject in the scene");
        return null;
    }

    /// <summary>
    /// Destroys the effect instance after a delay
    /// </summary>
    private IEnumerator DestroyEffectAfterDelay(GameObject effectInstance, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (effectInstance != null)
        {
            Debug.Log("🗑️ Destroying feeding effect");
            Destroy(effectInstance);
        }
    }

    /// <summary>
    /// Plays feeding audio using SoundEffectManager or fallback AudioSource
    /// </summary>
    private void PlayFeedingAudio()
    {
        if (!enableFeedingAudio) return;

        try
        {
            // Try to use SoundEffectManager first (preferred method)
            SoundEffectManager.Play(feedingSoundName, randomPitch);
            Debug.Log($"🔊 Played feeding sound: {feedingSoundName}");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SoundEffectManager not available or sound '{feedingSoundName}' not found: {ex.Message}");
            
            // Fallback: Use direct AudioClip with temporary AudioSource
            if (feedingAudioClip != null)
            {
                PlayFeedingAudioFallback();
            }
            else
            {
                Debug.LogWarning("No fallback audio clip assigned for feeding sound");
            }
        }
    }

    /// <summary>
    /// Fallback method to play feeding audio when SoundEffectManager is not available
    /// </summary>
    private void PlayFeedingAudioFallback()
    {
        if (feedingAudioClip == null) return;

        // Create temporary GameObject for audio playback
        GameObject tempAudioGO = new GameObject("TempFeedingAudio");
        tempAudioGO.transform.position = transform.position;
        
        AudioSource audioSource = tempAudioGO.AddComponent<AudioSource>();
        audioSource.clip = feedingAudioClip;
        audioSource.volume = 0.8f;
        audioSource.spatialBlend = 0f; // 2D sound
        
        // Add random pitch if enabled
        if (randomPitch)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.3f);
        }
        
        audioSource.Play();
        
        // Destroy temporary GameObject after audio finishes
        Destroy(tempAudioGO, feedingAudioClip.length + 0.1f);
        
        Debug.Log("🔊 Played feeding sound using fallback method");
    }
    
    /// <summary>
    /// Record feeding history cho FeedingManager
    /// </summary>
    private void RecordFeedingHistory()
    {
        if (CareHistoryRecorder.Instance != null)
        {
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            if (currentUser != null)
            {
                // Lấy current pet ID (có thể cần thêm logic để xác định pet nào đang được feed)
                var pets = APIPlayerPet.GetPetsByPlayerId(currentUser.id);
                if (pets != null && pets.Count > 0)
                {
                    // Sử dụng pet đầu tiên hoặc implement logic để xác định current active pet
                    int playerPetId = pets[0].playerPetID;
                    CareHistoryRecorder.Instance.RecordFeedingHistory(playerPetId, currentUser.id);
                }
            }
        }
    }

    /// <summary>
    /// Adds experience to the player when feeding a pet
    /// </summary>
    private void AddExperienceForFeeding()
    {
        PlayerLevel playerLevel = GameObject.Find("Player").GetComponent<PlayerLevel>();
        if (playerLevel != null)
        {
            playerLevel.AddExp(expRewardPerFeed);
            Debug.Log($"Added {expRewardPerFeed} experience for feeding pet");
        }
        else
        {
            Debug.LogWarning("PlayerLevel component not found on Player GameObject");
        }
    }

    /// <summary>
    /// Updates the player's inventory after using a food item
    /// </summary>
    /// <param name="foodItem">The food item that was used</param>
    private IEnumerator UpdateInventory(FoodItem foodItem)
    {
        PlayerInventory inventory = new PlayerInventory
        {
            playerID = currentPlayerId,
            shopProductID = foodItem.ShopProductId,
            quantity = -1 // Giam 1 quantitys
        };

        bool apiCallSuccess = false;

        // Goi API
        yield return StartCoroutine(APIPlayerInventory.UpdatePlayerInventoryCoroutine(inventory, success =>
        {
            apiCallSuccess = success;
        }));

        if (apiCallSuccess)
        {
            Debug.Log("Successfully updated player inventory after feeding");

            // Cap nhat so luong
            foodItem.Quantity--;

            // Neu het xoa khoi inventory
            if (foodItem.Quantity <= 0)
            {
                foodItems.Remove(foodItem);
                // Goi api va xoa
                StartCoroutine(APIPlayerInventory.DeletePlayerInventoryCoroutine(
                    currentPlayerId,
                    foodItem.ShopProductId,
                    null
                ));
            }

            // Lam moi panel
            PopulateFeedingPanel();
        }
        else
        {
            Debug.LogError("Failed to update player inventory after feeding");
            DisplayErrorMessage("Failed to update inventory. Please try again.");
        }
    }

    /// <summary>
    /// Class representing a food item in the player's inventory
    /// </summary>
    [System.Serializable]
    public class FoodItem
    {
        public int PlayerId { get; set; }
        public int ShopProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AcquiredAt { get; set; }
        public ProductInfo ProductInfo { get; set; }
    }

    /// <summary>
    /// Class representing product information for a food item
    /// </summary>
    [System.Serializable]
    public class ProductInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}