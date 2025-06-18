using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.SceneManagement;



public class LoginPlayer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public string Scenename;
    [Header("UI References")]
    [SerializeField] public TMP_InputField usernameField;
    [SerializeField] public TMP_InputField passwordField;
    [SerializeField] public Button loginButton;
    [SerializeField] public TextMeshProUGUI errorText;


    public void LogInPlayer()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        User user = APIUser.LoginAPI(username, password);
        if (user != null)
        {
            SceneManager.LoadScene(Scenename); // Load the main menu scene after successful login
            Debug.Log("Login successful! User ID: " + user.id);
            // You can now use the user object as needed, e.g., store it in a session or pass it to another scene
        }
        else
        {
            errorText.text = "Incorrect pass or player name.";
            Debug.LogError("Login failed. Please check your credentials.");
        }
    }

}
