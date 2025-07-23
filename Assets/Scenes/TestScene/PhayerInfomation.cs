using TMPro;
using UnityEngine;




public class PhayerInfomation : MonoBehaviour
{
    public TextMeshProUGUI Id;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Pass;
    public TextMeshProUGUI Email;
    public TextMeshProUGUI Level;
    public TMP_InputField InputName;
    public TMP_InputField InputPass;

    public void NewUser()
    {
        User user = APIUser.GetUser();
        Id.text = user.id.ToString();
        Name.text = user.userName;
        Pass.text = user.password;
    }
    public void Login()
    {

        string PhayerTypeName = InputName.text;
        string PlayerTypePass = InputPass.text;
        User user = APIUser.LoginAPI(PhayerTypeName, PlayerTypePass);
        Id.text = user.id.ToString();
        Name.text = user.userName;
        Pass.text = user.password;
        Level.text = user.level.ToString();
        Email.text = user.email;

    }
}
