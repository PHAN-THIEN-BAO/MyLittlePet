using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;


public class APIPlayerAchievement : MonoBehaviour
{
    public static List<PlayerAchievement> GetAchievementByIdPlayer(int idPlayer)
    {
        // Create a request to the API endpoint with the player ID
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/PlayerAchievement/Player/" + idPlayer);
        // Set the method to GET
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        // Read the response stream and convert it to a string
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        // Parse the JSON response into a list of PlayerPet objects
        return JsonConvert.DeserializeObject<List<PlayerAchievement>>(jsonResponse);
    }

    // đàu tiên qua swagger chạy thử API put để lấy được uputsau đó cop url qua va kêu compilot 
    // viết code cho hàm gọi API UpdateAchievement có tham số là playerID, achievementID, isCollected
    // tạo 1 cái script mới tên là GetRewardAchievement.cs(để trong playerachievement folder)
    // script này a sẽ tạo 1 hàm GetRewardAchievement() để gọi API UpdateAchievement trong script này
    // hàm này sẽ nhận vào 3 tham số là playerID, achievementID, isCollected trong đó tạo
    // [SerializeField] public TMP_Text amountReward; và [SerializeField] public TMP_Text achievementId;
    // để lấy giá trị từ UI Text trong Unity sau đó gọi API UpdateAchievement với các tham số này và set isCollected là true
    // trong GetRewardAchievement.cs thêm [SerializeField] public gameObject rewardPanel; nếu chạy API UpdateAchievement thành công
    //thì sẽ hiện rewardPanel sau đó dùng PlayerInfomation.UpdatePlayerInfo() để cập nhật thông tin người chơi, cập nhật sẽ mặc định là cộng Diamond và amount cộng thêm 
    // là [SerializeField] public TMP_Text amountReward như ở trên, và đồng thời gọi APIUser.UpdateUser để cập nhật thông tin người chơi lên server.
    // cuối cùng là kéo thêm emty có tên là LoadAchievement vào Ready_Collected_Button On Click() để load lại danh sách thành tích đã nhận thưởng.
    // kéo các scipt trên cào Ready_Collected_Button cho đến hết danh sách thành tích



}
