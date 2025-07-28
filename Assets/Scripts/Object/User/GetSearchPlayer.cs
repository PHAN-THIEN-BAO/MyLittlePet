using TMPro;
using UnityEngine;

public class GetSearchPlayer : MonoBehaviour
{
    [SerializeField] public TMP_Text playerName;
    [SerializeField] public TMP_Text playerId;
    [SerializeField] public TMP_Text playerLevel;

    /// <summary>
    /// Retrieves user details based on the ID found in the parent Player GameObject
    /// </summary>
    public void GetPlayerSearchId()
    {
        // take the parent transform of this GameObject
        Transform parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("Show_Detail_Button");
            return;
        }

        // find the TMP_Text component with name "Id" in the parent GameObject
        TMP_Text idText = parentTransform.Find("Id")?.GetComponent<TMP_Text>();
        if (idText == null)
        {
            Debug.LogError("not found parent");
            return;
        }

        // take the text from the TMP_Text component
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

        // call the API to get user information by ID
        User user = APIUser.GetUserById(userId);
        if (user == null)
        {
            Debug.LogError("Can no get Id: " + userId);
            return;
        }

        // update the UI with user information
        UpdateUserDisplay(user);
    }

    /// <summary>
    /// Updates the UI with user information
    /// </summary>
    /// <param name="user">The user data to display</param>
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
