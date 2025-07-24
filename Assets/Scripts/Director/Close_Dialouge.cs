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
        // If no dialogue panel is assigned, try to find it from parent
        if (dialoguePanel == null)
            dialoguePanel = transform.parent.gameObject;

        // Set up the button click event if available
        if (closeButton != null && closeOnButtonClick)
        {
            closeButton.onClick.AddListener(CloseDialogue);
        }
    }

    void Update()
    {
        // Check for key press to close the dialogue
        if (Input.GetKeyDown(closeKey))
        {
            CloseDialogue();
        }
    }

    /// <summary>
    /// Closes the dialogue panel and notifies the DialogueManager
    /// </summary>
    public void CloseDialogue()
    {
        if (dialoguePanel != null)
        {
            // Hide the dialogue panel
            dialoguePanel.SetActive(false);

            // If force close is enabled and DialogueManager exists, end the dialogue
            if (forceClose && DialogueManager.Instance != null)
            {
                // Try to end the current dialogue through the DialogueManager
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