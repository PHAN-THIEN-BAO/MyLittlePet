using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LogoutUser : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public string LoadScene;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.9f, 0.9f, 0.9f); // Slightly different color

    private Image buttonImage;

    private void Awake()
    {
        // Get the Image component (button background)
        buttonImage = GetComponent<Image>();

        // If no Image component found, try to find it in children
        if (buttonImage == null)
        {
            buttonImage = GetComponentInChildren<Image>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }

    public void LogOut()
    {
        // Clear player information
        PlayerInfomation.ClearPlayerInfo();
        // go to the login scene
        SceneManager.LoadScene(LoadScene);
    }
}