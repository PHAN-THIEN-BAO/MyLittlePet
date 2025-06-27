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
        
        // Configure the grid layout if one exists
        ConfigureGridLayout();
    }
    
    // Configure the grid layout component for the food items container
    private void ConfigureGridLayout()
    {
        if (foodItemsContainer != null)
        {
            // Try to get GridLayoutGroup component
            GridLayoutGroup gridLayout = foodItemsContainer.GetComponent<GridLayoutGroup>();
            
            // If no grid layout, add one
            if (gridLayout == null)
            {
                gridLayout = foodItemsContainer.gameObject.AddComponent<GridLayoutGroup>();
            }
            
            // Configure the grid layout
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = gridColumns;
            gridLayout.spacing = itemSpacing;
            gridLayout.padding = gridPadding;
            
            // Calculate cell size based on container width and columns
            if (foodItemsContainer is RectTransform rectTransform)
            {
                float availableWidth = rectTransform.rect.width - gridPadding.left - gridPadding.right - (gridColumns - 1) * itemSpacing.x;
                float cellWidth = availableWidth / gridColumns;
                gridLayout.cellSize = new Vector2(cellWidth, cellWidth * 1.2f); // Make cells slightly taller than wide
            }
        }
    }
    
    /// <summary>
    /// Shows the feeding panel and loads food items for the specified player
    /// </summary>
    /// <param name="playerId">The ID of the player whose food items to display</param>
    /// <param name="customCareAmount">Custom care amount to set</param>
    /// <param name="playerPetID">The ID of the player's pet</param>
    public void ShowFeedingPanel(int playerId, int customCareAmount = 0, int playerPetID = -1)
    {
        Debug.Log($"ShowFeedingPanel called with playerId={playerId}, customCareAmount={customCareAmount}, playerPetID={playerPetID}");
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

        // Trigger care for all pets
        if (petInfoManager != null)
        {
            petInfoManager.OnCareForAllButtonClicked(playerPetID);
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
        
        // Create new items
        foreach (var foodItem in availableFoodItems)
        {
            // Instantiate the food item prefab
            GameObject newFoodItemObj = Instantiate(foodItemPrefab, foodItemsContainer);
            FoodItemUI foodItemUI = newFoodItemObj.GetComponent<FoodItemUI>();
            
            if (foodItemUI != null)
            {
                // Set up the food item UI using the FoodItemUI component
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
        
        // Display a message if no food items are available
        if (availableFoodItems.Count == 0)
        {
            GameObject messageObj = new GameObject("NoFoodMessage");
            messageObj.transform.SetParent(foodItemsContainer, false);
            
            TMP_Text message = messageObj.AddComponent<TMP_Text>();
            message.text = "No food items available. Visit the shop to buy some!";
            message.alignment = TextAlignmentOptions.Center;
            message.fontSize = 24;
            
            RectTransform rect = messageObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 100);
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
    /// Handles food item click events
    /// </summary>
    /// <param name="shopProductId">The ID of the shop product that was clicked</param>
    private void OnFoodItemClicked(int shopProductId)
    {
        // Find the selected food item
        FoodItem selectedItem = foodItems.Find(item => item.ShopProductId == shopProductId);
        
        if (selectedItem != null && selectedItem.Quantity > 0)
        {
            Debug.Log($"Feeding pet with {selectedItem.ProductInfo.Name}");
            
            // Feed the pet
            if (petInfoManager != null)
            {
                petInfoManager.OnFeedButtonClicked();
            }
            
            // Call API to update inventory (reduce quantity by 1)
            StartCoroutine(UpdateInventory(selectedItem));
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
