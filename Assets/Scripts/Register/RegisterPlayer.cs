using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class RegisterPlayer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public string Scenename;

    [Header("UI References")]
    [SerializeField] public TMP_InputField registerUsernameField;
    [SerializeField] public TMP_InputField registerPasswordField;
    [SerializeField] public TMP_InputField confirmPasswordField;
    [SerializeField] public Button registerButton;
    [SerializeField] public TextMeshProUGUI registerErrorText;

    public void RegisterNewUser()
    {
        string username = registerUsernameField.text;
        string password = registerPasswordField.text;
        string confirmPassword = confirmPasswordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(confirmPassword))
        {
            registerErrorText.text = "Fill all field please!";
            registerErrorText.color = Color.red;
            Debug.LogError("Fill all field please!");
            return;
        }

        if (password != confirmPassword)
        {
            registerErrorText.text = "Passwords do not match!";
            registerErrorText.color = Color.red;
            Debug.LogError("Ðang ký th?t b?i. M?t kh?u không kh?p.");
            return;
        }

        try
        {
            bool success = APIUser.RegisterAPI(username, password);

            if (success)
            {
                registerErrorText.text = "Registration successful!";
                registerErrorText.color = Color.green;

                registerUsernameField.text = "";
                registerPasswordField.text = "";
                confirmPasswordField.text = "";

                Debug.Log("Ðang ký thành công cho ngu?i dùng: " + username);

            }
            else
            {
                registerErrorText.text = "Username already exists!";
                registerErrorText.color = Color.red;
                Debug.LogError("Ðang ký th?t b?i. Tên ngu?i dùng có th? dã t?n t?i.");
            }
        }
        catch (System.Exception ex)
        {
            registerErrorText.text = "Registration error!";
            registerErrorText.color = Color.red;
            Debug.LogException(ex);
        }
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}