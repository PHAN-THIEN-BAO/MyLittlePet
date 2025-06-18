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
    [SerializeField] public TMP_InputField registerEmailField;
    [SerializeField] public TMP_InputField registerPasswordField;
    [SerializeField] public TMP_InputField confirmPasswordField;
    [SerializeField] public Button registerButton;
    [SerializeField] public TextMeshProUGUI registerErrorText;

    public void RegisterNewUser()
    {
        string username = registerUsernameField.text;
        string email = registerEmailField.text;
        string password = registerPasswordField.text;
        string confirmPassword = confirmPasswordField.text;

        // Kiểm tra các trường trống
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(email))
        {
            registerErrorText.text = "Vui lòng điền đầy đủ thông tin.";
            registerErrorText.color = Color.red;
            Debug.LogError("Đăng ký thất bại. Vui lòng điền đầy đủ thông tin.");
            return;
        }

        // Kiểm tra mật khẩu khớp nhau
        if (password != confirmPassword)
        {
            registerErrorText.text = "Mật khẩu không khớp.";
            registerErrorText.color = Color.red;
            Debug.LogError("Đăng ký thất bại. Mật khẩu không khớp.");
            return;
        }

        try
        {
            // Gọi API để đăng ký người dùng
            bool success = APIUser.RegisterAPI(username, password, email);

            if (success)
            {
                // Đăng ký thành công
                registerErrorText.text = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
                registerErrorText.color = Color.green;

                // Xóa các trường nhập liệu
                registerUsernameField.text = "";
                registerPasswordField.text = "";
                confirmPasswordField.text = "";
                registerEmailField.text = "";

                Debug.Log("Đăng ký thành công cho người dùng: " + username);

                // Tùy chọn: chuyển đến màn hình đăng nhập sau khi đăng ký thành công
                // StartCoroutine(LoadSceneAfterDelay(Scenename, 2.0f));
            }
            else
            {
                // Đăng ký thất bại
                registerErrorText.text = "Tên người dùng đã tồn tại hoặc đăng ký thất bại.";
                registerErrorText.color = Color.red;
                Debug.LogError("Đăng ký thất bại. Tên người dùng có thể đã tồn tại.");
            }
        }
        catch (System.Exception ex)
        {
            // Bắt bất kỳ ngoại lệ nào trong quá trình gọi API
            registerErrorText.text = "Lỗi đăng ký. Vui lòng thử lại sau.";
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