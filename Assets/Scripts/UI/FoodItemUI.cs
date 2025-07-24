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
    public GameObject foodPanel;
    public Image foodImage;
    public TMP_Text nameText;
    public TMP_Text quantityText;
    public TMP_Text descriptionText;
    public Button useButton;
    
    [Header("Visual Effects")]
    public Animation feedAnimation;
    public AudioSource feedSound;
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
    
    private int shopProductId;
    
    private Action<int> onClickAction;
    
    private Vector3 originalScale;
    
    private bool isHovering = false;
    
    private void Start()
    {
        originalScale = transform.localScale;
        
    }
    
    private void Update()
    {
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
    
    public void Setup(FeedingManager.FoodItem foodItem, Action<int> onClick)
    {
        shopProductId = foodItem.ShopProductId;
        onClickAction = onClick;
        
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
        
        if (foodImage != null)
        {
            if (placeholderImage != null)
            {
                foodImage.sprite = placeholderImage;
            }
            
            if (!string.IsNullOrEmpty(foodItem.ProductInfo.ImageUrl))
            {
                StartCoroutine(LoadFoodImage(foodItem.ProductInfo.ImageUrl));
            }
        }
        
        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(() => OnUseButtonClicked());
            
            useButton.interactable = foodItem.Quantity > 0;
        }
        
        if (feedAnimation != null)
        {
        }
    }
    
    private void OnUseButtonClicked()
    {
        if (feedSound != null)
        {
            feedSound.Play();
        }
        
        if (feedingEffect != null)
        {
            feedingEffect.Play();
        }
        
        onClickAction?.Invoke(shopProductId);
    }
    
    private IEnumerator LoadFoodImage(string imageUrl)
    {
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(imageUrl))
        {
            request.timeout = 10;
            
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || 
                request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading food image: {request.error} for URL: {imageUrl}");
                
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
                    
                    Sprite sprite = Sprite.Create(
                        texture, 
                        new Rect(0, 0, texture.width, texture.height), 
                        new Vector2(0.5f, 0.5f),
                        100f,
                        0,
                        SpriteMeshType.FullRect);
                    
                    foodImage.sprite = sprite;
                    
                    foodImage.preserveAspect = true;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error creating sprite from texture: {ex.Message}");
                    
                    if (errorImage != null)
                    {
                        foodImage.sprite = errorImage;
                    }
                }
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        
        if (feedAnimation != null && !feedAnimation.isPlaying)
        {
            feedAnimation.Play();
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}