using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginPlayer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public string Scenename;

    [Header("New Player Scene Settings")]
    [SerializeField] private string beginnerSceneName = "Tutorial";
    [SerializeField] private bool useSceneTransition = true;
    [SerializeField] private Vector3 playerSpawnPosition = Vector3.zero;

    [Header("UI References")]
    [SerializeField] public TMP_InputField usernameField;
    [SerializeField] public TMP_InputField passwordField;
    [SerializeField] public Button loginButton;
    [SerializeField] public TextMeshProUGUI errorText;
    [SerializeField] public GameObject currentPanel;
    [SerializeField] public GameObject successPanel;
    [SerializeField] public GameObject tapToPlayPanel;
    [SerializeField] public GameObject newGamePanel;

    public void LogInPlayer()
    {
        
        string username = usernameField.text;
        string password = passwordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Please fill in all fields.";
            errorText.color = Color.red;
            Debug.Log("Login failed. Please fill in all fields.");
            return;
        }

        try
        {
            User user = APIUser.LoginAPI(username, password);

            Debug.Log("API returned: " + (user == null ? "null" : "user object"));

            if (user != null)
            {
                PlayerInfomation.SavePlayerInfo(user);
                Debug.Log("User info: " + JsonUtility.ToJson(user));

                currentPanel.SetActive(false);
                successPanel.SetActive(false);
                Debug.Log("Login successful! User ID: " + user.id);

                StartCoroutine(CheckAndRedirectPlayer(user));
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
            if (response != null)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
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

    private IEnumerator CheckAndRedirectPlayer(User user)
    {
        yield return new WaitForSeconds(1.5f);

        try
        {
            int petCount = APIUser.GetPlayerPetCount(user.id.ToString());

            if (petCount == 0)
            {
                successPanel.SetActive(false);
                tapToPlayPanel.SetActive(false);
                newGamePanel.SetActive(true);
                Debug.Log($"New player detected. Redirecting to beginner scene: {beginnerSceneName}");
                RedirectToBeginnerScene();
            }
            else
            {
                successPanel.SetActive(false);
                tapToPlayPanel.SetActive(false);
                newGamePanel.SetActive(true);
                Debug.Log($"Existing player detected. Redirecting to main scene: {Scenename}");
                RedirectToMainScene();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error checking player pet count: " + ex.Message);
            RedirectToMainScene();
        }
    }

    private void RedirectToBeginnerScene()
    {
        if (string.IsNullOrEmpty(beginnerSceneName))
        {
            Debug.LogError("Beginner scene name is not set! Using main scene instead.");

            RedirectToMainScene();
            return;
        }

        if (useSceneTransition && SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(beginnerSceneName, playerSpawnPosition);
        }
        else
        {
            SceneManager.LoadScene(beginnerSceneName);
        }
    }

    private void RedirectToMainScene()
    {
        if (string.IsNullOrEmpty(Scenename))
        {
            Debug.LogError("Main scene name is not set!");
            return;
        }

        if (useSceneTransition && SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(Scenename, Vector3.zero);
        }
        else
        {
            SceneManager.LoadScene(Scenename);
        }
    }
}