using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class FoodItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    public GameObject foodPanel;           // The individual food panel
    public Image foodImage;                // Image component for the food icon
    public TMP_Text nameText;              // Text for food name
    public TMP_Text quantityText;          // Text for quantity
    public TMP_Text descriptionText;       // Text for description
    public Button useButton;               // Button to use this food item
    
    [Header("Visual Effects")]
    public Animation feedAnimation;         // Optional animation when hovering/selecting
    public AudioSource feedSound;           // Optional sound effect
    [Tooltip("The scale multiplier when hovering over this item")]
    public float hoverScaleMultiplier = 1.1f;
    [Tooltip("How fast the hover scaling happens")]
    public float hoverScaleSpeed = 0.1f;
    [Tooltip("Optional particle effect when feeding")]
    public ParticleSystem feedingEffect;
    
    [Header("Default Images")]
    [Tooltip("Placeholder image shown while loading or if image fails to load")]
    public Sprite placeholderImage;
    [Tooltip("Image shown when there's an error loading the food image")]
    public Sprite errorImage;
    
    // The shop product ID of this food item
    private int shopProductId;
    
    // The action to call when this food item is clicked
    private Action<int> onClickAction;
    
    // Original scale for hover effect
    private Vector3 originalScale;
    
    // Track if we're currently hovering
    private bool isHovering = false;
    
    // Start is called before the first frame update
    private void Start()
    {
        // Store original scale for hover effects
        originalScale = transform.localScale;
        
        // Add hover effects if not using Event Trigger component
        // (The IPointerEnterHandler and IPointerExitHandler interfaces handle this)
    }
    
    // Update is called once per frame
    private void Update()
    {
        // Handle hover scaling animation
        if (isHovering)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, 
                originalScale * hoverScaleMultiplier, Time.deltaTime * hoverScaleSpeed * 10);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, 
                originalScale, Time.deltaTime * hoverScaleSpeed * 10);
        }
    }
    
    // Set up the food item UI
    public void Setup(FeedingManager.FoodItem foodItem, Action<int> onClick)
    {
        // Store the data
        shopProductId = foodItem.ShopProductId;
        onClickAction = onClick;
        
        // Set the UI elements
        if (nameText != null)
        {
            nameText.text = foodItem.ProductInfo.Name;
        }
        
        if (quantityText != null)
        {
            quantityText.text = $"x{foodItem.Quantity}";
        }
        
        if (descriptionText != null && !string.IsNullOrEmpty(foodItem.ProductInfo.Description))
        {
            descriptionText.text = foodItem.ProductInfo.Description;
        }
        
        // Load the image
        if (foodImage != null)
        {
            // Set placeholder image while loading
            if (placeholderImage != null)
            {
                foodImage.sprite = placeholderImage;
            }
            
            if (!string.IsNullOrEmpty(foodItem.ProductInfo.ImageUrl))
            {
                StartCoroutine(LoadFoodImage(foodItem.ProductInfo.ImageUrl));
            }
        }
        
        // Set up the button
        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(() => OnUseButtonClicked());
            
            // Disable button if quantity is 0
            useButton.interactable = foodItem.Quantity > 0;
        }
        
        // Play animation if available
        if (feedAnimation != null)
        {
            // Animation can be triggered on hover via event trigger component
        }
    }
    
    // Handler for when the use button is clicked
    private void OnUseButtonClicked()
    {
        // Play feeding sound if available
        if (feedSound != null)
        {
            feedSound.Play();
        }
        
        // Play feeding particle effect if available
        if (feedingEffect != null)
        {
            feedingEffect.Play();
        }
        
        // Call the click action
        onClickAction?.Invoke(shopProductId);
    }
    
    // Load the food image from a URL
    private IEnumerator LoadFoodImage(string imageUrl)
    {
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(imageUrl))
        {
            // Optional: Add a timeout
            request.timeout = 10; // 10 seconds timeout
            
            // Show loading indicator or animation here if needed
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || 
                request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading food image: {request.error} for URL: {imageUrl}");
                
                // Set error image if available, otherwise keep placeholder
                if (errorImage != null)
                {
                    foodImage.sprite = errorImage;
                }
            }
            else
            {
                try
                {
                    Texture2D texture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(request);
                    
                    // Create sprite with proper pivot and scaling
                    Sprite sprite = Sprite.Create(
                        texture, 
                        new Rect(0, 0, texture.width, texture.height), 
                        new Vector2(0.5f, 0.5f),
                        100f, // Pixels per unit
                        0,    // Extrude edges
                        SpriteMeshType.FullRect);
                    
                    // Apply the loaded sprite
                    foodImage.sprite = sprite;
                    
                    // Adjust image aspect ratio to match original if needed
                    foodImage.preserveAspect = true;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error creating sprite from texture: {ex.Message}");
                    
                    // Set error image if available, otherwise keep placeholder
                    if (errorImage != null)
                    {
                        foodImage.sprite = errorImage;
                    }
                }
            }
        }
    }
    
    // Implemented from IPointerEnterHandler - Called when pointer enters this UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        
        // Play animation if available
        if (feedAnimation != null && !feedAnimation.isPlaying)
        {
            feedAnimation.Play();
        }
    }
    
    // Implemented from IPointerExitHandler - Called when pointer exits this UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
