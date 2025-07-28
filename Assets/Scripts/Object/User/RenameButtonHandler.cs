// Tạo file mới RenameButtonHandler.cs
using UnityEngine;
using UnityEngine.UI;
public class RenameButtonHandler : MonoBehaviour
{
    public GameObject renamePanel;
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ToggleRenamePanel);
        }
    }
    void ToggleRenamePanel()
    {
        Debug.Log("ToggleRenamePanel được gọi");
        if (renamePanel != null)
        {
            renamePanel.SetActive(!renamePanel.activeSelf);
            Debug.Log("Rename panel active: " + renamePanel.activeSelf);
        }
    }
}