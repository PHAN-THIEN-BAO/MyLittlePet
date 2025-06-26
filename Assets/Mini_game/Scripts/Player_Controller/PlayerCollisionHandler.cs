// 6/9/2025 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;
using TMPro; // Import thư viện TextMeshPro

public class PlayerCollisionHandler : MonoBehaviour
{
    public int collisionLimit = 3; // Số lần va chạm tối đa
    public GameObject gameOverCanvas; // Tham chiếu đến GameOver_Panel
    public GameObject youWonCanvas; // Tham chiếu đến Won_Panel
    public TMP_Text livesTMPText; // Tham chiếu đến Text_Lives (TMP)
    private int collisionCount = 0; // Đếm số lần va chạm

    void Start()
    {
        // Ẩn các Canvas ngay từ đầu
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        if (youWonCanvas != null)
        {
            youWonCanvas.SetActive(false);
        }

        // Cập nhật số mạng ban đầu lên UI
        UpdateLivesText();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra va chạm với Enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collisionCount++;
            Debug.Log("Số lần va chạm: " + collisionCount);

            // Cập nhật UI
            UpdateLivesText();

            // Nếu số lần va chạm vượt giới hạn => Game Over
            if (collisionCount >= collisionLimit)
            {
                User user = PlayerInfomation.LoadPlayerInfo();
                user.coin += 50;
                PlayerInfomation.SavePlayerInfo(user);
                APIUser.UpdateUser();
                GameOver();
            }
        }

        // Kiểm tra va chạm với Goal để kết thúc game
        if (collision.gameObject.CompareTag("Goal"))
        {
            YouWon();
        }
    }

    void GameOver()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true); // Hiển thị Game Over UI
        }

        Time.timeScale = 0; // Dừng game
    }

    void YouWon()
    {
        if (youWonCanvas != null)
        {
            youWonCanvas.SetActive(true); // Hiển thị You Won UI
        }

        Time.timeScale = 0; // Dừng game
    }

    void UpdateLivesText()
    {
        if (livesTMPText != null)
        {
            int livesRemaining = collisionLimit - collisionCount; // Tính số mạng còn lại
            livesTMPText.text = "x" + livesRemaining; // Cập nhật nội dung Text
        }
    }
}