using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Creates a simple test interface for demonstrating UnityWebRequest API calls
/// </summary>
public class UnityWebRequestTestUI : MonoBehaviour
{
    [Header("UI Creation")]
    [SerializeField] private bool createUIOnStart = false;
    [SerializeField] private Canvas targetCanvas;

    [ContextMenu("Create Test UI")]
    public void CreateTestUI()
    {
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogError("No Canvas found. Please create a Canvas first.");
                return;
            }
        }

        // Create main test panel
        GameObject testPanel = CreatePanel("UnityWebRequestTestPanel", targetCanvas.transform, new Vector2(600, 400));
        
        // Create title
        CreateText("Title", "UnityWebRequest API Test", testPanel.transform, new Vector2(0, 180), 20, Color.white);
        
        // Create input fields
        var usernameField = CreateInputField("UsernameField", "Username", testPanel.transform, new Vector2(-150, 120));
        var passwordField = CreateInputField("PasswordField", "Password", testPanel.transform, new Vector2(-150, 80));
        var emailField = CreateInputField("EmailField", "Email", testPanel.transform, new Vector2(-150, 40));
        
        // Set password field type
        passwordField.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Password;
        
        // Create buttons
        CreateButton("TestLoginBtn", "Test Login", testPanel.transform, new Vector2(150, 120));
        CreateButton("TestRegisterBtn", "Test Register", testPanel.transform, new Vector2(150, 80));
        CreateButton("CreateNewUserBtn", "Create New User", testPanel.transform, new Vector2(150, 40));
        
        // Create result text
        CreateText("ResultText", "Ready to test...", testPanel.transform, new Vector2(0, -20), 14, Color.yellow);
        
        // Create log panel
        GameObject logPanel = CreatePanel("LogPanel", testPanel.transform, new Vector2(580, 200));
        logPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -90);
        
        // Create scroll view for logs
        CreateScrollView(logPanel.transform);
        
        // Attach the demo script
        var demo = testPanel.AddComponent<UnityWebRequestDemo>();
        
        // Assign references
        demo.GetType().GetField("testUsernameField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(demo, usernameField.GetComponent<TMP_InputField>());
        demo.GetType().GetField("testPasswordField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(demo, passwordField.GetComponent<TMP_InputField>());
        demo.GetType().GetField("testEmailField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(demo, emailField.GetComponent<TMP_InputField>());
        
        var resultText = testPanel.transform.Find("ResultText").GetComponent<TextMeshProUGUI>();
        demo.GetType().GetField("resultText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(demo, resultText);
        
        // Add button listeners
        testPanel.transform.Find("TestLoginBtn").GetComponent<Button>().onClick.AddListener(demo.TestExistingUserLogin);
        testPanel.transform.Find("TestRegisterBtn").GetComponent<Button>().onClick.AddListener(demo.TestUserRegistration);
        testPanel.transform.Find("CreateNewUserBtn").GetComponent<Button>().onClick.AddListener(demo.CreateAndTestNewUser);
        
        Debug.Log("UnityWebRequest Test UI created successfully!");
    }

    private GameObject CreatePanel(string name, Transform parent, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        return panel;
    }

    private GameObject CreateInputField(string name, string placeholder, Transform parent, Vector2 position)
    {
        GameObject inputFieldObj = new GameObject(name);
        inputFieldObj.transform.SetParent(parent, false);
        
        RectTransform rect = inputFieldObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(280, 30);
        rect.anchoredPosition = position;
        
        Image image = inputFieldObj.AddComponent<Image>();
        image.color = Color.white;
        
        TMP_InputField inputField = inputFieldObj.AddComponent<TMP_InputField>();
        
        // Create text area
        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(inputFieldObj.transform, false);
        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.sizeDelta = new Vector2(-20, -6);
        textAreaRect.offsetMin = new Vector2(10, 3);
        textAreaRect.offsetMax = new Vector2(-10, -3);
        
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
        placeholderText.fontSize = 12;
        
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
        text.fontSize = 12;
        
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
        rect.sizeDelta = new Vector2(120, 30);
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
        buttonText.fontSize = 12;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        return buttonObj;
    }

    private GameObject CreateText(string name, string text, Transform parent, Vector2 position, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(550, 30);
        rect.anchoredPosition = position;
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        return textObj;
    }

    private void CreateScrollView(Transform parent)
    {
        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(parent, false);
        
        RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
        scrollRect.sizeDelta = new Vector2(560, 180);
        scrollRect.anchoredPosition = Vector2.zero;
        
        Image scrollImage = scrollView.AddComponent<Image>();
        scrollImage.color = new Color(0.05f, 0.05f, 0.05f, 1f);
        
        ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        
        // Create content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(scrollView.transform, false);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(540, 160);
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(0, 1);
        contentRect.pivot = new Vector2(0, 1);
        
        // Create log text
        GameObject logText = new GameObject("LogText");
        logText.transform.SetParent(content.transform, false);
        
        RectTransform logRect = logText.AddComponent<RectTransform>();
        logRect.sizeDelta = new Vector2(540, 160);
        logRect.anchorMin = Vector2.zero;
        logRect.anchorMax = Vector2.one;
        
        TextMeshProUGUI logTextComponent = logText.AddComponent<TextMeshProUGUI>();
        logTextComponent.text = "Log output will appear here...\n";
        logTextComponent.fontSize = 10;
        logTextComponent.color = Color.green;
        logTextComponent.alignment = TextAlignmentOptions.TopLeft;
        
        scroll.content = contentRect;
        scroll.viewport = scrollRect;
    }

    void Start()
    {
        if (createUIOnStart)
        {
            CreateTestUI();
        }
    }
}
