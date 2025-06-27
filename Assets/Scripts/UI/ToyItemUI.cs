using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class ToyItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    public GameObject toyPanel;
    public Image toyImage;
    public TMP_Text nameText;
    public TMP_Text quantityText;
    public TMP_Text descriptionText;
    public Button useButton;

    [Header("Visual Effects")]
    public Animation playAnimation;
    public AudioSource playSound;
    [Tooltip("The scale multiplier when hovering over this item")]
    public float hoverScaleMultiplier = 1.1f;
    [Tooltip("How fast the hover scaling happens")]
    public float hoverScaleSpeed = 0.1f;
    [Tooltip("Optional particle effect when playing")]
    public ParticleSystem playingEffect;

    [Header("Default Images")]
    [Tooltip("Placeholder image shown while loading or if image fails to load")]
    public Sprite placeholderImage;
    [Tooltip("Image shown when there's an error loading the toy image")]
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

    public void Setup(FeedingManager.FoodItem toyItem, Action<int> onClick)
    {
        shopProductId = toyItem.ShopProductId;
        onClickAction = onClick;

        if (nameText != null)
            nameText.text = toyItem.ProductInfo.Name;

        if (quantityText != null)
            quantityText.text = $"x{toyItem.Quantity}";

        if (descriptionText != null && !string.IsNullOrEmpty(toyItem.ProductInfo.Description))
            descriptionText.text = toyItem.ProductInfo.Description;

        if (toyImage != null)
        {
            if (placeholderImage != null)
                toyImage.sprite = placeholderImage;

            if (!string.IsNullOrEmpty(toyItem.ProductInfo.ImageUrl))
                StartCoroutine(LoadToyImage(toyItem.ProductInfo.ImageUrl));
        }

        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(() => OnUseButtonClicked());
            useButton.interactable = toyItem.Quantity > 0;
        }
    }

    private void OnUseButtonClicked()
    {
        if (playSound != null)
            playSound.Play();

        if (playingEffect != null)
            playingEffect.Play();

        onClickAction?.Invoke(shopProductId);
    }

    private IEnumerator LoadToyImage(string imageUrl)
    {
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(imageUrl))
        {
            request.timeout = 10;
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError ||
                request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading toy image: {request.error} for URL: {imageUrl}");
                if (errorImage != null)
                    toyImage.sprite = errorImage;
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

                    toyImage.sprite = sprite;
                    toyImage.preserveAspect = true;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error creating sprite from texture: {ex.Message}");
                    if (errorImage != null)
                        toyImage.sprite = errorImage;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        if (playAnimation != null && !playAnimation.isPlaying)
            playAnimation.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
