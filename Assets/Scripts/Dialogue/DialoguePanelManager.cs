using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanelManager : MonoBehaviour
{
    public static DialoguePanelManager Instance;

    public Image dialogueImage;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public GameObject dialoguePanel;

    public Animator animator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(Sprite image, string title, string description)
    {
        if (dialogueImage != null)
            dialogueImage.sprite = image;

        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (animator != null)
            animator.SetBool("IsOpen", true);
    }

    public void HideDialogue()
    {
        if (animator != null)
        {
            animator.SetBool("IsOpen", false);
        }
        else
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }
    }

    public void OnCloseButtonClick()
    {
        HideDialogue();
    }
}