using TMPro;
using UnityEngine;

public class PlayerInfoMainScene : MonoBehaviour
{

    [SerializeField] public TMP_Text coin;
    [SerializeField] public TMP_Text diamond;
    [SerializeField] public TMP_Text gem;
    [SerializeField] public TMP_Text level;
    [SerializeField] public TMP_Text coinShop;
    [SerializeField] public TMP_Text diamondShop;
    [SerializeField] public TMP_Text gemShop;


    private void Start()
    {
        MainSceneInfo();
    }
    public void MainSceneInfo()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user != null)
        {
            coin.text = user.coin.ToString();
            coinShop.text = user.coin.ToString();
            diamond.text = user.diamond.ToString();
            diamondShop.text = user.diamond.ToString();
            gem.text = user.gem.ToString();
            gemShop.text = user.gem.ToString();
            level.text = "Level " + user.level.ToString();
        }
        else
        {
            Debug.LogError("User information is not available.");
        }
    }

    public void UpdateUI()
    {
        MainSceneInfo();
    }

}