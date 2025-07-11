using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class SearchPlayer : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] TMP_InputField inputSearch;
    [SerializeField] GameObject notFoundPannel;
    [SerializeField] Transform playerContainer; // container to hold player clones
    [SerializeField] TMP_Text warring;
    [SerializeField] GameObject guidePannel;
    private List<GameObject> playerClones = new List<GameObject>(); // save player clones

    public void Search()
    {
        // Hide guide panel when user searches
        if (guidePannel != null)
            guidePannel.SetActive(false);

        // Load current user information
        User currentUser = PlayerInfomation.LoadPlayerInfo();

        // Take input from search field and trim whitespace
        string searchTerm = inputSearch.text.Trim();

        // Check if searchTerm is empty or null
        if (string.IsNullOrEmpty(searchTerm))
        {
            warring.text = "Let's find someone! Enter a name here.";
            return; // stop execution if search term is empty
        }
        else
        {
            warring.text = ""; // clear warning text if search term is valid
        }

        // delete all player clones current
        ClearPlayerClones();

        // Found user by search term using APIUser class
        List<User> userList = APIUser.SearchUser(searchTerm);

        // Remove the current user from the result list
        if (currentUser != null && userList != null)
        {
            // Use RemoveAll to filter out the current user by ID
            userList.RemoveAll(user => user.id == currentUser.id);
        }

        // check if userList is null or empty after removing current user
        if (userList == null || userList.Count == 0)
        {
            // show not found panel if no users found
            notFoundPannel.SetActive(true);

            // hide the Player object if it exists
            if (Player != null)
                Player.SetActive(false);
        }
        else
        {
            // hide not found panel
            notFoundPannel.SetActive(false);

            // hide the Player object if it exists
            if (Player != null)
                Player.SetActive(false);

            // Create clones of Player for each user found
            for (int i = 0; i < userList.Count; i++)
            {
                // Clone game object Player
                GameObject playerClone = Instantiate(Player, playerContainer);

                // show the cloned player object
                playerClone.SetActive(true);

                // save the clone to playerClones list
                playerClones.Add(playerClone);

                // Update player information
                UpdatePlayerInfo(playerClone, userList[i]);
            }
        }
    }



    private void UpdatePlayerInfo(GameObject playerObject, User user)
    {
        // Set name
        TMP_Text nameText = playerObject.transform.Find("Name_Player").GetComponent<TMP_Text>();
        if (nameText != null)
            nameText.text = user.userName;

        // Set role
        TMP_Text levelText = playerObject.transform.Find("Level").GetComponent<TMP_Text>();
        if (levelText != null)
            levelText.text = "Level: " + user.level.ToString();

        // Set id
        TMP_Text idText = playerObject.transform.Find("Id").GetComponent<TMP_Text>();
        if (idText != null)
            idText.text = "ID: " + user.id.ToString();
    }

    private void ClearPlayerClones()
    {
        // Destroy all player clones in the list
        foreach (GameObject clone in playerClones)
        {
            Destroy(clone);
        }

        playerClones.Clear();
    }

    private void Start()
    {
        // Hide Player object at the start
        if (Player != null)
            Player.SetActive(false);

        // Hide not found panel at the start
        if (notFoundPannel != null)
            notFoundPannel.SetActive(false);

        // Show guide panel at the start
        if (guidePannel != null)
            guidePannel.SetActive(true);
    }
}
