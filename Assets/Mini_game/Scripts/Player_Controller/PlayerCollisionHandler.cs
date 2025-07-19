// 6/9/2025 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;
using TMPro;
using System.Collections.Generic; // Import thư viện TextMeshPro

public class PlayerCollisionHandler : MonoBehaviour
{
    private int collisionLimit = 3; // maximum number of collisions allowed before game over
    public GameObject gameOverCanvas; // reference to GameOver_Panel
    public GameObject youWonCanvas; // reference to YouWon_Panel
    public TMP_Text livesTMPText; // reference to TextMeshPro for displaying lives
    private int collisionCount = 0; // count of collisions
    public TMP_Text reward;
    public GameObject coinImg;
    public GameObject diamondImg;
    public GameObject gemImg;

    void Start()
    {
        // hide Game Over and You Won canvases at the start
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        if (youWonCanvas != null)
        {
            youWonCanvas.SetActive(false);
        }

        // update the lives text at the start
        UpdateLivesText();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // check if the collided object has the tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collisionCount++;
            Debug.Log("Số lần va chạm: " + collisionCount);

            // update the lives text
            UpdateLivesText();

            // if the collision count reaches the limit, show Game Over UI
            if (collisionCount >= collisionLimit)
            {
                GameOver();
            }
        }

        // check if the collided object has the tag "Goal"
        if (collision.gameObject.CompareTag("Goal"))
        {
            YouWon();
            if(livesTMPText.text == "x1")
            {
                User user = PlayerInfomation.LoadPlayerInfo();
                int rewardAmount = Random.Range(100, 251);
                user.coin += rewardAmount;
                reward.text = "+ " + rewardAmount.ToString();
                HideAllCurrencies();
                coinImg.SetActive(true); // show the coin image
                PlayerInfomation.SavePlayerInfo(user);
                APIUser.UpdateUser();
            }
            else if (livesTMPText.text == "x2")
            {
                User user = PlayerInfomation.LoadPlayerInfo();
                int rewardAmount = Random.Range(1, 6);
                user.diamond += rewardAmount;
                reward.text = "+ " + rewardAmount.ToString();
                HideAllCurrencies();
                diamondImg.SetActive(true); // show the diamond image
                PlayerInfomation.SavePlayerInfo(user);
                APIUser.UpdateUser();
            }
            else if (livesTMPText.text == "x3")
            {
                User user = PlayerInfomation.LoadPlayerInfo();
                int rewardAmount = Random.Range(1, 4);
                user.gem += rewardAmount;
                reward.text = "+ " + rewardAmount.ToString();
                HideAllCurrencies();
                gemImg.SetActive(true); // show the gem image
                PlayerInfomation.SavePlayerInfo(user);
                APIUser.UpdateUser();
            }
        }
    }

    void GameOver()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true); // show Game Over UI
        }

        Time.timeScale = 0; // stop the game
    }

    void YouWon()
    {
        if (youWonCanvas != null)
        {
            youWonCanvas.SetActive(true); // show You Won UI

            // set score to api
            User user = PlayerInfomation.LoadPlayerInfo();
            List<GameRecord> gameRecordList = APIGameRecord.GetGameRecordByPlayerID(user.id);
            // get gameRecordList with minigameID = 1
            GameRecord gameRecord = gameRecordList.Find(gr => gr.minigameID == 1);
            if (gameRecord != null)
            {
                gameRecord.score += 1; // increment score by 1
                APIGameRecord.SendGameRecord("PUT", user.id, 1, gameRecord.score); // update game record
            }
            else
            {
                APIGameRecord.SendGameRecord("POST", user.id, 1, 1);
            }

        }

        Time.timeScale = 0; // stop the game
    }

    void UpdateLivesText()
    {
        if (livesTMPText != null)
        {
            int livesRemaining = collisionLimit - collisionCount; // calculate remaining lives
            livesTMPText.text = "x" + livesRemaining; // update the TextMeshPro text
        }
    }

    void HideAllCurrencies()
    {
        if (coinImg != null)
        {
            coinImg.SetActive(false); // hide the coin image
        }
        if (diamondImg != null)
        {
            diamondImg.SetActive(false); // hide the diamond image
        }
        if (gemImg != null)
        {
            gemImg.SetActive(false); // hide the gem image
        }
    }
}