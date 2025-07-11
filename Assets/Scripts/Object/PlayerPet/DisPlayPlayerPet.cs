using TMPro;
using UnityEngine;

public class DisPlayPlayerPet : MonoBehaviour
{

    [SerializeField] public TMP_Text playerId;
    [SerializeField] public GameObject pet;


    public void displayListPet()
    {

    }


    //lấy playerId nó có dạng là Id:số tách nó ra lấy số sau đó gọi API GetPlayerPetByPlayerId truyền vào
    //playerId và lấy ra danh sách Playerpet và gán  petCustomName vào Name_Player_Pet, Level vào level,
    //và lấy petID dò dò trong shopproduct lấy ra shopproduct có petId để lấy ảnh trong shopProduct gán vào Avatar là ok

}
