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
    public GameObject feedingPanel;
    public Transform foodItemsContainer;
    public GameObject foodItemPrefab;
    public Button closeButton;
    public ScrollRect scrollView;
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
    [SerializeField] private int expRewardPerFeed = 10;
    [Header("No Food Message")]
    public GameObject noFoodMessage;
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
    private PetInfoUIManager petInfoManager;
    private int currentPlayerId;
    private List<FoodItem> foodItems = new List<FoodItem>();
    void Start()
    {
        petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager == null)
        {
            Debug.LogError("PetInfoUIManager not found in the scene. FeedingManager will not work properly.");
        }
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseFeedingPanel);
        }
        if (feedingPanel != null)
        {
            feedingPanel.SetActive(false);
        }
        if (gridPadding == null)
        {
            gridPadding = new RectOffset(10, 10, 10, 10);
        }
        if (foodItemsContainer == null)
        {
            Debug.LogWarning("FeedingManager: foodItemsContainer is not assigned in the Inspector.");
        }
    }
    public void ShowFeedingPanel(int playerId, int customCareAmount = 0)
    {
        Debug.Log($"ShowFeedingPanel called with playerId={playerId}, customCareAmount={customCareAmount}");
        if (enableDependencyCheck && petInfoManager != null)
        {
            var blockReason = petInfoManager.CanPerformAction(PetAction.ActionType.Feed);
            if (blockReason != PetInfoUIManager.ActionBlockReason.None)
            {
                string message = petInfoManager.GetBlockReasonMessage(blockReason, PetAction.ActionType.Feed);
                Debug.LogWarning($"Cannot show feeding panel: {message}");
                return;
            }
        }
        currentPlayerId = playerId;
        if (petInfoManager != null)
        {
            petInfoManager.pendingFeedAmount = customCareAmount > 0 ? customCareAmount : defaultFeedIncreaseAmount;
        }
        if (feedingPanel != null)
        {
            feedingPanel.SetActive(true);
            Debug.Log("Feeding panel set active. Starting LoadFoodItems coroutine.");
            StartCoroutine(LoadFoodItems(playerId));
        }
    }
    private void ShowFeedingBlockedMessage(string message)
    {
        Debug.LogWarning($"?? FEEDING BLOCKED: {message}");
        StartCoroutine(ShowTemporaryMessage(message));
    }
    private IEnumerator ShowTemporaryMessage(string message)
    {
        GameObject messagePanel = new GameObject("FeedingBlockedMessage");
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            messagePanel.transform.SetParent(canvas.transform, false);
            Image bg = messagePanel.AddComponent<Image>();
            bg.color = new Color(1f, 0.2f, 0.2f, 0.8f);
            GameObject textObj = new GameObject("MessageText");
            textObj.transform.SetParent(messagePanel.transform, false);
            TMP_Text text = textObj.AddComponent<TMP_Text>();
            text.text = message;
            text.color = Color.white;
            text.fontSize = 18;
            text.alignment = TextAlignmentOptions.Center;
            RectTransform rect = messagePanel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 100);
            rect.anchoredPosition = Vector2.zero;
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(380, 80);
            textRect.anchoredPosition = Vector2.zero;
            yield return new WaitForSeconds(3f);
            if (messagePanel != null)
                Destroy(messagePanel);
        }
    }
    public void CloseFeedingPanel()
    {
        if (feedingPanel != null)
        {
            feedingPanel.SetActive(false);
        }
    }
    private IEnumerator LoadFoodItems(int playerId)
    {
        string url = $"{apiBaseUrl}/PlayerInventory/FoodItems/{playerId}";
        Debug.Log($"Requesting food items from: {url}");
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading food items: {request.error}");
                DisplayErrorMessage($"Error loading food items: {request.error}");
            }
            else
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"Food items API response: {responseText}");
                try
                {
                    foodItems = JsonConvert.DeserializeObject<List<FoodItem>>(responseText);
                    Debug.Log($"Fetched {foodItems?.Count ?? 0} food items from PlayerInventory.");
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
    private void DisplayErrorMessage(string message)
    {
        foreach (Transform child in foodItemsContainer)
        {
            Destroy(child.gameObject);
        }
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
    private void PopulateFeedingPanel()
    {
        foreach (Transform child in foodItemsContainer)
        {
            Destroy(child.gameObject);
        }
        var availableFoodItems = foodItems.Where(f => f.Quantity > 0).ToList();
        if (noFoodMessage != null)
        {
            noFoodMessage.SetActive(availableFoodItems.Count == 0);
        }
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
        if (scrollView != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollView.normalizedPosition = new Vector2(0, 1);
        }
    }
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
    private void OnFoodItemClicked(int shopProductId)
    {
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
        FoodItem selectedItem = foodItems.Find(item => item.ShopProductId == shopProductId);
        if (selectedItem != null && selectedItem.Quantity > 0)
        {
            Debug.Log($"Feeding pet with {selectedItem.ProductInfo.Name}");
            PlayFeedingAudio();
            InstantiateFeedingEffect();
            if (petInfoManager != null)
            {
                petInfoManager.OnFeedButtonClicked();
                RecordFeedingHistory();
            }
            AddExperienceForFeeding();
            StartCoroutine(UpdateInventory(selectedItem));
        }
    }
    private void InstantiateFeedingEffect()
    {
        if (!enableFeedingEffect || feedingEffectPrefab == null)
        {
            Debug.LogWarning("Feeding effect is disabled or prefab is not assigned");
            return;
        }
        GameObject currentPet = FindCurrentPetGameObject();
        if (currentPet == null)
        {
            Debug.LogWarning("Could not find current pet GameObject for feeding effect");
            return;
        }
        Vector3 effectPosition = currentPet.transform.position;
        effectPosition.y += effectHeightOffset;
        if (randomEffectPosition)
        {
            effectPosition.x += UnityEngine.Random.Range(-effectPositionRange.x, effectPositionRange.x);
            effectPosition.y += UnityEngine.Random.Range(0, effectPositionRange.y);
        }
        GameObject effectInstance = Instantiate(feedingEffectPrefab, effectPosition, Quaternion.identity);
        Vector3 pos = effectInstance.transform.position;
        pos.z = currentPet.transform.position.z - 0.1f;
        effectInstance.transform.position = pos;
        Debug.Log($"?? Instantiated feeding effect at position: {effectPosition}");
        StartCoroutine(DestroyEffectAfterDelay(effectInstance, effectDuration));
    }
    private GameObject FindCurrentPetGameObject()
    {
        if (petInfoManager != null)
        {
            var (playerPetId, playerId) = petInfoManager.GetCurrentPetAndPlayerId();
            if (playerPetId > 0)
            {
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
        PetDataHolder firstPet = FindObjectOfType<PetDataHolder>();
        if (firstPet != null)
        {
            return firstPet.gameObject;
        }
        GameObject taggedPet = GameObject.FindGameObjectWithTag("Pet");
        if (taggedPet != null)
        {
            return taggedPet;
        }
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
    private IEnumerator DestroyEffectAfterDelay(GameObject effectInstance, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effectInstance != null)
        {
            Debug.Log("??? Destroying feeding effect");
            Destroy(effectInstance);
        }
    }
    private void PlayFeedingAudio()
    {
        if (!enableFeedingAudio) return;
        try
        {
            SoundEffectManager.Play(feedingSoundName, randomPitch);
            Debug.Log($"?? Played feeding sound: {feedingSoundName}");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"SoundEffectManager not available or sound '{feedingSoundName}' not found: {ex.Message}");
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
    private void PlayFeedingAudioFallback()
    {
        if (feedingAudioClip == null) return;
        GameObject tempAudioGO = new GameObject("TempFeedingAudio");
        tempAudioGO.transform.position = transform.position;
        AudioSource audioSource = tempAudioGO.AddComponent<AudioSource>();
        audioSource.clip = feedingAudioClip;
        audioSource.volume = 0.8f;
        audioSource.spatialBlend = 0f;
        if (randomPitch)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.3f);
        }
        audioSource.Play();
        Destroy(tempAudioGO, feedingAudioClip.length + 0.1f);
        Debug.Log("?? Played feeding sound using fallback method");
    }
    private void RecordFeedingHistory()
    {
        if (CareHistoryRecorder.Instance != null)
        {
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            if (currentUser != null)
            {
                var pets = APIPlayerPet.GetPetsByPlayerId(currentUser.id);
                if (pets != null && pets.Count > 0)
                {
                    int playerPetId = pets[0].playerPetID;
                    CareHistoryRecorder.Instance.RecordFeedingHistory(playerPetId, currentUser.id);
                }
            }
        }
    }
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
    private IEnumerator UpdateInventory(FoodItem foodItem)
    {
        PlayerInventory inventory = new PlayerInventory
        {
            playerID = currentPlayerId,
            shopProductID = foodItem.ShopProductId,
            quantity = -1
        };
        bool apiCallSuccess = false;
        yield return StartCoroutine(APIPlayerInventory.UpdatePlayerInventoryCoroutine(inventory, success =>
        {
            apiCallSuccess = success;
        }));
        if (apiCallSuccess)
        {
            Debug.Log("Successfully updated player inventory after feeding");
            foodItem.Quantity--;
            if (foodItem.Quantity <= 0)
            {
                foodItems.Remove(foodItem);
                StartCoroutine(APIPlayerInventory.DeletePlayerInventoryCoroutine(
                    currentPlayerId,
                    foodItem.ShopProductId,
                    null
                ));
            }
            PopulateFeedingPanel();
        }
        else
        {
            Debug.LogError("Failed to update player inventory after feeding");
            DisplayErrorMessage("Failed to update inventory. Please try again.");
        }
    }
    [System.Serializable]
    public class FoodItem
    {
        public int PlayerId { get; set; }
        public int ShopProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AcquiredAt { get; set; }
        public ProductInfo ProductInfo { get; set; }
    }
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