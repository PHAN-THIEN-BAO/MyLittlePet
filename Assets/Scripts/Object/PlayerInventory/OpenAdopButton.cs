using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAdopButton : MonoBehaviour
{
    private TMP_InputField nameInput;
    private Button adopButton;
    private Button currentButton;

    void Awake()
    {
        // Take the parent transform of this GameObject
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            // Find and assign components from the parent GameObject
            nameInput = parentTransform.Find("Name_Input").GetComponent<TMP_InputField>();
            adopButton = parentTransform.Find("Adop_Button").GetComponent<Button>();
            currentButton = parentTransform.Find("Open_Adop_Button").GetComponent<Button>();
        }
        else
        {
            Debug.LogError("Cannot find parent for OpenAdopButton");
        }
    }

    public void OpenAdopButtonDisplay()
    {
        if (currentButton != null)
            currentButton.gameObject.SetActive(false);

        if (adopButton != null)
            adopButton.gameObject.SetActive(true);

        if (nameInput != null)
            nameInput.gameObject.SetActive(true);
    }

    public void CloseAdopButtonDisplay()
    {
        if (currentButton != null)
            currentButton.gameObject.SetActive(true);

        if (adopButton != null)
            adopButton.gameObject.SetActive(false);

        if (nameInput != null)
            nameInput.gameObject.SetActive(false);
    }
}
