using TMPro;
using UnityEngine;




public class PhayerInfomation : MonoBehaviour
{
    public TextMeshProUGUI Id;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Pass;

    public void NewUser()
    {
        User user = APIUser.GetUser();
        Id.text = user.id.ToString();
        Name.text = user.userName;
        Pass.text = user.password;
    }
}
