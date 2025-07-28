using TMPro;
using UnityEngine;

public class GetSearchPlayer : MonoBehaviour
{
    [SerializeField] public TMP_Text playerName;
    [SerializeField] public TMP_Text playerId;
    [SerializeField] public TMP_Text playerLevel;

    public void GetPlayerSearchId()
    {
        Transform parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("Show_Detail_Button");
            return;
        }

        TMP_Text idText = parentTransform.Find("Id")?.GetComponent<TMP_Text>();
        if (idText == null)
        {
            Debug.LogError("not found parent");
            return;
        }

        string idString = idText.text;
        string[] parts = idString.Split(':');
        if (parts.Length < 2)
        {
            Debug.LogError("format incorrect: " + idString);
            return;
        }

        string idValueStr = parts[1].Trim();
        if (!int.TryParse(idValueStr, out int userId))
        {
            Debug.LogError("can not convert : " + idValueStr);
            return;
        }

        User user = APIUser.GetUserById(userId);
        if (user == null)
        {
            Debug.LogError("Can no get Id: " + userId);
            return;
        }

        UpdateUserDisplay(user);
    }

    private void UpdateUserDisplay(User user)
    {
        if (playerName != null)
            playerName.text = user.userName;

        if (playerId != null)
            playerId.text = "ID: " + user.id;

        if (playerLevel != null)
            playerLevel.text = "Level: " + user.level;
    }
}