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
        currentPlayerId = playerId;
        if (playingPanel != null)
        {
            playingPanel.SetActive(true);
            StartCoroutine(LoadToyItems(playerId));
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

    private void OnToyItemUsed(int shopProductId)
    {
        Debug.Log($"Played with toy ShopProductId={shopProductId}");

        // Tăng happiness cho pet hiện tại
        if (petInfoManager != null)
        {
            petInfoManager.PlayWithPet();
        }
    }
}
