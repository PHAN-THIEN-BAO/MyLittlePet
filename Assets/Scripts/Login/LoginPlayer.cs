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

        // Check for empty fields first
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Please fill in all fields.";
            errorText.color = Color.red;
            Debug.LogError("Login failed. Please fill in all fields.");
            return;
        }

        try
        {
            // Attempt to login
            User user = APIUser.LoginAPI(username, password);

            // Debug what's returned
            Debug.Log("API returned: " + (user == null ? "null" : "user object"));

            if (user != null)
            {
                SceneManager.LoadScene(Scenename); // Load the main menu scene after successful login
                Debug.Log("Login successful! User ID: " + user.id);
            }
            else
            {
                // Make sure this block is reached when authentication fails
                errorText.color = Color.red;
                errorText.text = "Incorrect pass or player name.";
                Debug.LogError("Login failed. Please check your credentials.");
            }
        }
        catch (System.Exception ex)
        {
            // Catch any exceptions during the API call
            errorText.color = Color.red;
            errorText.text = "Incorrect pass or player name.";
            Debug.LogException(ex);
        }
    }

}
