using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckFillInput : MonoBehaviour
{

    [SerializeField] public TMP_InputField inputField; // Input field to check
    [SerializeField] public Button submitButton; // Button to enable/disable

    private void Update()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            if (!submitButton.gameObject.activeSelf)
                submitButton.gameObject.SetActive(true);
        }
        else
        {
            if (submitButton.gameObject.activeSelf)
                submitButton.gameObject.SetActive(false);
        }
    }


}
