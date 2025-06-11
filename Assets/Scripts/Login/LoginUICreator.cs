using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to create login UI elements programmatically
/// Attach this to an empty GameObject and run CreateLoginUI() to auto-generate the login interface
/// </summary>
public class LoginUICreator : MonoBehaviour
{
    [Header("UI Creation Settings")]
    [SerializeField] private bool createOnStart = false;
    [SerializeField] private Canvas targetCanvas;

    [ContextMenu("Create Login UI")]
    public void CreateLoginUI()
    {
        // Find or create canvas
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogError("No Canvas found. Please create a Canvas first.");
                return;
            }
        }

        // Create main login panel
        GameObject loginPanel = CreatePanel("LoginPanel", targetCanvas.transform);
        
        // Create title
        CreateText("LoginTitle", "My Little Pet - Login", loginPanel.transform, new Vector2(0, 150), 24);
        
        // Create username field
        CreateInputField("UsernameField", "Username", loginPanel.transform, new Vector2(0, 50));
        
        // Create password field
        var passwordField = CreateInputField("PasswordField", "Password", loginPanel.transform, new Vector2(0, 0));
        passwordField.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Password;
        
        // Create login button
        CreateButton("LoginButton", "Login", loginPanel.transform, new Vector2(-75, -70));
        
        // Create register button
        CreateButton("RegisterButton", "Register", loginPanel.transform, new Vector2(75, -70));
        
        // Create status text
        CreateText("StatusText", "", loginPanel.transform, new Vector2(0, -120), 14, Color.green);
        
        // Create error text
        CreateText("ErrorText", "", loginPanel.transform, new Vector2(0, -140), 14, Color.red);
        
        // Create loading panel
        CreateLoadingPanel(loginPanel.transform);
        
        Debug.Log("Login UI created successfully! Don't forget to assign references in the Login script.");
    }

    private GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 300);
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        return panel;
    }

    private GameObject CreateInputField(string name, string placeholder, Transform parent, Vector2 position)
    {
        GameObject inputFieldObj = new GameObject(name);
        inputFieldObj.transform.SetParent(parent, false);
        
        RectTransform rect = inputFieldObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 40);
        rect.anchoredPosition = position;
        
        Image image = inputFieldObj.AddComponent<Image>();
        image.color = Color.white;
        
        TMP_InputField inputField = inputFieldObj.AddComponent<TMP_InputField>();
        
        // Create text area
        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(inputFieldObj.transform, false);
        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.sizeDelta = new Vector2(-20, -13);
        textAreaRect.offsetMin = new Vector2(10, 6);
        textAreaRect.offsetMax = new Vector2(-10, -7);
        
        // Create placeholder
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(textArea.transform, false);
        RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
        placeholderRect.sizeDelta = Vector2.zero;
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        
        TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholderText.text = placeholder;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholderText.fontSize = 14;
        
        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(textArea.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = Vector2.zero;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "";
        text.color = Color.black;
        text.fontSize = 14;
        
        inputField.textViewport = textAreaRect;
        inputField.textComponent = text;
        inputField.placeholder = placeholderText;
        
        return inputFieldObj;
    }

    private GameObject CreateButton(string name, string text, Transform parent, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(140, 40);
        rect.anchoredPosition = position;
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.6f, 1f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = Vector2.zero;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.color = Color.white;
        buttonText.fontSize = 16;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        return buttonObj;
    }

    private GameObject CreateText(string name, string text, Transform parent, Vector2 position, int fontSize, Color? color = null)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(350, 30);
        rect.anchoredPosition = position;
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color ?? Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        return textObj;
    }

    private void CreateLoadingPanel(Transform parent)
    {
        GameObject loadingPanel = new GameObject("LoadingPanel");
        loadingPanel.transform.SetParent(parent, false);
        
        RectTransform rect = loadingPanel.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 300);
        rect.anchoredPosition = Vector2.zero;
        
        Image image = loadingPanel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.7f);
        
        // Create loading text
        CreateText("LoadingText", "Loading...", loadingPanel.transform, Vector2.zero, 18, Color.white);
        
        // Disable by default
        loadingPanel.SetActive(false);
    }

    void Start()
    {
        if (createOnStart)
        {
            CreateLoginUI();
        }
    }
}
