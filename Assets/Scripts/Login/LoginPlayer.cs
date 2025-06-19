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
    [SerializeField] public GameObject currentPanel;
    [SerializeField] public GameObject successPanel;


    public void LogInPlayer()
    {
        string username = usernameField.text;
        string password = passwordField.text;

        // Check for empty fields first
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Please fill in all fields.";
            errorText.color = Color.red;
            Debug.Log("Login failed. Please fill in all fields.");
            return;
        }

        try
        {
            // Attempt to login
            User user = APIUser.LoginAPI(username, password);

            Debug.Log("API returned: " + (user == null ? "null" : "user object"));

            if (user != null)
            {
                //SceneManager.LoadScene(Scenename);
                currentPanel.SetActive(false);
                successPanel.SetActive(true);
                Debug.Log("Login successful! User ID: " + user.id);
            }
            else
            {
                errorText.color = Color.red;
                errorText.text = "Incorrect pass or player name.";
                Debug.Log("Login failed. Please check your credentials.");
            }
        }
        catch (System.Net.WebException webEx)
        {
            var response = webEx.Response as System.Net.HttpWebResponse;
            if (response != null && response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                //handle 404 error
                errorText.color = Color.red;
                errorText.text = "User not found (404).";
                Debug.LogWarning("User not found (404).");
            }
            else
            {
                errorText.color = Color.red;
                errorText.text = "Server error. Please try again.";
                Debug.LogException(webEx);
            }
            
        }
        catch (System.Exception ex)
        {
            errorText.color = Color.red;
            errorText.text = "Login failed.";
            Debug.LogException(ex);
            
        }
    }


}
