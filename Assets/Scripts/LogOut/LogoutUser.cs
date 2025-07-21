using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class LogoutUser : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public string LoadScene;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.9f, 0.9f, 0.9f);
    private Image buttonImage;
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
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
        PlayerInfomation.ClearPlayerInfo();
        SceneManager.LoadScene(LoadScene);
    }
}