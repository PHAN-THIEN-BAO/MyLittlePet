using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class PlayingManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject playingPanel;
    public Transform toyItemsContainer;
    public GameObject toyItemPrefab;
    public Button closeButton;
    public ScrollRect scrollView;

    [Header("No Toy Message")]
    public GameObject noToyMessage;

    [Header("API Settings")]
    [SerializeField] private string apiBaseUrl = "https://localhost:7035";

    [Header("Dependency Check Settings")]
    [Tooltip("Check pet status dependencies before playing")]
    public bool enableDependencyCheck = true;

    private int currentPlayerId;
    private List<FeedingManager.FoodItem> toyItems = new List<FeedingManager.FoodItem>();
    private PetInfoUIManager petInfoManager;

    void Start()
    {
        petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePlayingPanel);

        if (playingPanel != null)
            playingPanel.SetActive(false);
    }

    public void ShowPlayingPanel(int playerId)
    {
        // CHECK DEPENDENCY BEFORE SHOWING PANEL
        if (enableDependencyCheck && petInfoManager != null)
        {
            var blockReason = petInfoManager.CanPerformAction(PetAction.ActionType.Play);
            if (blockReason != PetInfoUIManager.ActionBlockReason.None)
            {
                string message = petInfoManager.GetBlockReasonMessage(blockReason, PetAction.ActionType.Play);
                Debug.LogWarning($"Cannot show playing panel: {message}");

                // Show message to user instead of opening panel
                ShowPlayingBlockedMessage(message);
                return;
            }
        }

        currentPlayerId = playerId;
        if (playingPanel != null)
        {
            playingPanel.SetActive(true);
            StartCoroutine(LoadToyItems(playerId));
        }
    }

    /// <summary>
    /// Shows message when playing is blocked due to dependencies
    /// </summary>
    private void ShowPlayingBlockedMessage(string message)
    {
        Debug.LogWarning($"🚫 PLAYING BLOCKED: {message}");
        StartCoroutine(ShowTemporaryMessage(message));
    }

    private IEnumerator ShowTemporaryMessage(string message)
    {
        // Create temporary message UI (similar to FeedingManager)
        GameObject messagePanel = new GameObject("PlayingBlockedMessage");
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            messagePanel.transform.SetParent(canvas.transform, false);

            // Add background
            Image bg = messagePanel.AddComponent<Image>();
            bg.color = new Color(1f, 0.8f, 0.2f, 0.8f); // Yellow background for play

            // Add text
            GameObject textObj = new GameObject("MessageText");
            textObj.transform.SetParent(messagePanel.transform, false);
            TMP_Text text = textObj.AddComponent<TMP_Text>();
            text.text = message;
            text.color = Color.black;
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

    public void ClosePlayingPanel()
    {
        if (playingPanel != null)
            playingPanel.SetActive(false);
    }

    private IEnumerator LoadToyItems(int playerId)
    {
        string url = $"{apiBaseUrl}/PlayerInventory/ToyItems/{playerId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                DisplayErrorMessage($"Error loading toy items: {request.error}");
            }
            else
            {
                string responseText = request.downloadHandler.text;
                try
                {
                    toyItems = JsonConvert.DeserializeObject<List<FeedingManager.FoodItem>>(responseText);
                    PopulatePlayingPanel();
                }
                catch (System.Exception ex)
                {
                    DisplayErrorMessage($"Error parsing toy items: {ex.Message}");
                }
            }
        }
    }

    private void DisplayErrorMessage(string message)
    {
        foreach (Transform child in toyItemsContainer)
            Destroy(child.gameObject);

        if (noToyMessage != null)
        {
            noToyMessage.SetActive(true);
            var text = noToyMessage.GetComponent<TMP_Text>();
            if (text != null) text.text = message;
        }
    }

    private void PopulatePlayingPanel()
    {
        foreach (Transform child in toyItemsContainer)
            Destroy(child.gameObject);

        var availableToyItems = toyItems.Where(t => t.Quantity > 0).ToList();

        if (noToyMessage != null)
            noToyMessage.SetActive(availableToyItems.Count == 0);

        foreach (var toyItem in availableToyItems)
        {
            GameObject newToyItemObj = Instantiate(toyItemPrefab, toyItemsContainer);
            ToyItemUI toyItemUI = newToyItemObj.GetComponent<ToyItemUI>();
            if (toyItemUI != null)
            {
                toyItemUI.Setup(toyItem, OnToyItemUsed);
            }
        }

        if (scrollView != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollView.normalizedPosition = new Vector2(0, 1);
        }
    }

    /// <summary>
    /// Enhanced toy item handler with dependency check
    /// </summary>
    private void OnToyItemUsed(int shopProductId)
    {
        // DOUBLE-CHECK DEPENDENCY BEFORE PLAYING
        if (enableDependencyCheck && petInfoManager != null)
        {
            var blockReason = petInfoManager.CanPerformAction(PetAction.ActionType.Play);
            if (blockReason != PetInfoUIManager.ActionBlockReason.None)
            {
                string message = petInfoManager.GetBlockReasonMessage(blockReason, PetAction.ActionType.Play);
                Debug.LogWarning($"Cannot play with pet: {message}");
                ShowPlayingBlockedMessage(message);
                return;
            }
        }

        Debug.Log($"Played with toy ShopProductId={shopProductId}");

        // Find the toy item and reduce quantity
        var toyItem = toyItems.Find(t => t.ShopProductId == shopProductId);
        if (toyItem != null && toyItem.Quantity > 0)
        {
            // Play with pet
            if (petInfoManager != null)
            {
                petInfoManager.PlayWithPet();
            }

            // Update inventory (reduce toy quantity)
            StartCoroutine(UpdateToyInventory(toyItem));
        }
    }

    /// <summary>
    /// Update toy inventory after use
    /// </summary>
    private IEnumerator UpdateToyInventory(FeedingManager.FoodItem toyItem)
    {
        PlayerInventory inventory = new PlayerInventory
        {
            playerID = currentPlayerId,
            shopProductID = toyItem.ShopProductId,
            quantity = -1 // Reduce by 1
        };

        bool apiCallSuccess = false;

        yield return StartCoroutine(APIPlayerInventory.UpdatePlayerInventoryCoroutine(inventory, success =>
        {
            apiCallSuccess = success;
        }));

        if (apiCallSuccess)
        {
            Debug.Log("Successfully updated toy inventory after playing");

            // Update local quantity
            toyItem.Quantity--;

            // Remove if quantity reaches 0
            if (toyItem.Quantity <= 0)
            {
                toyItems.Remove(toyItem);
                StartCoroutine(APIPlayerInventory.DeletePlayerInventoryCoroutine(
                    currentPlayerId,
                    toyItem.ShopProductId,
                    null
                ));
            }

            // Refresh panel
            PopulatePlayingPanel();
        }
        else
        {
            Debug.LogError("Failed to update toy inventory after playing");
        }
    }
}
