using TMPro;
using UnityEngine;
using System.Collections.Generic;
public class SearchPlayer : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] TMP_InputField inputSearch;
    [SerializeField] GameObject notFoundPannel;
    [SerializeField] Transform playerContainer;
    [SerializeField] TMP_Text warring;
    [SerializeField] GameObject guidePannel;
    private List<GameObject> playerClones = new List<GameObject>();
    public void Search()
    {
        if (guidePannel != null)
            guidePannel.SetActive(false);
        User currentUser = PlayerInfomation.LoadPlayerInfo();
        string searchTerm = inputSearch.text.Trim();
        if (string.IsNullOrEmpty(searchTerm))
        {
            warring.text = "Let's find someone! Enter a name here.";
            return;
        }
        else
        {
            warring.text = "";
        }
        ClearPlayerClones();
        List<User> userList = APIUser.SearchUser(searchTerm);
        if (currentUser != null && userList != null)
        {
            userList.RemoveAll(user => user.id == currentUser.id);
        }
        if (userList == null || userList.Count == 0)
        {
            notFoundPannel.SetActive(true);
            if (Player != null)
                Player.SetActive(false);
        }
        else
        {
            notFoundPannel.SetActive(false);
            if (Player != null)
                Player.SetActive(false);
            for (int i = 0; i < userList.Count; i++)
            {
                GameObject playerClone = Instantiate(Player, playerContainer);
                playerClone.SetActive(true);
                playerClones.Add(playerClone);
                UpdatePlayerInfo(playerClone, userList[i]);
            }
        }
    }
    private void UpdatePlayerInfo(GameObject playerObject, User user)
    {
        TMP_Text nameText = playerObject.transform.Find("Name_Player").GetComponent<TMP_Text>();
        if (nameText != null)
            nameText.text = user.userName;
        TMP_Text levelText = playerObject.transform.Find("Level").GetComponent<TMP_Text>();
        if (levelText != null)
            levelText.text = "Level: " + user.level.ToString();
        TMP_Text idText = playerObject.transform.Find("Id").GetComponent<TMP_Text>();
        if (idText != null)
            idText.text = "ID: " + user.id.ToString();
    }
    private void ClearPlayerClones()
    {
        foreach (GameObject clone in playerClones)
        {
            Destroy(clone);
        }
        playerClones.Clear();
    }
    private void Start()
    {
        if (Player != null)
            Player.SetActive(false);
        if (notFoundPannel != null)
            notFoundPannel.SetActive(false);
        if (guidePannel != null)
            guidePannel.SetActive(true);
    }
}