using UnityEngine;
using UnityEngine.UI;

public class Close_Dialouge : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The dialogue panel to close")]
    public GameObject dialoguePanel;

    [Tooltip("Optional button component that will trigger the close action")]
    public Button closeButton;

    [Header("Settings")]
    [Tooltip("Close dialogue when player presses this key")]
    public KeyCode closeKey = KeyCode.Escape;

    [Tooltip("If true, will close dialogue when button is clicked")]
    public bool closeOnButtonClick = true;

    [Tooltip("If true, will force close dialogue even if not completed")]
    public bool forceClose = false;

    void Start()
    {
        if (dialoguePanel == null)
            dialoguePanel = transform.parent.gameObject;

        if (closeButton != null && closeOnButtonClick)
        {
            closeButton.onClick.AddListener(CloseDialogue);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(closeKey))
        {
            CloseDialogue();
        }
    }

    public void CloseDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);

            if (forceClose && DialogueManager.Instance != null)
            {
                if (typeof(DialogueManager).GetMethod("EndDialogue") != null)
                {
                    DialogueManager.Instance.SendMessage("EndDialogue", SendMessageOptions.DontRequireReceiver);
                }
            }

            Debug.Log("Dialogue closed");
        }
        else
        {
            Debug.LogWarning("No dialogue panel assigned to close");
        }
    }
}