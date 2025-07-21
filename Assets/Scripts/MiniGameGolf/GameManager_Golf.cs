using UnityEngine;
using TMPro;
public class GameManager_Golf : MonoBehaviour
{
    public GolfBall playerBall, petBall;
    public PetStats petStats;
    public int playerStrokeCount = 0;
    public int petStrokeCount = 0;
    public TextMeshProUGUI playerStrokeText;
    public TextMeshProUGUI petStrokeText;
    public TextMeshProUGUI winText;
    private bool playerDone = false;
    private bool petDone = false;
    private bool gameEnded = false;
    void Start()
    {
        UpdateStrokeUI();
    }
    void Update()
    {
        if (playerBall.transform.position.y < -5f)
        {
            Debug.Log("Player ball out of bounds! Respawn.");
            playerBall.ResetToLastSafe();
        }
        if (petBall.transform.position.y < -5f)
        {
            Debug.Log("Pet ball out of bounds! Respawn.");
            petBall.ResetToLastSafe();
        }
    }
    public void AddPlayerStroke()
    {
        playerStrokeCount++;
        UpdateStrokeUI();
    }
    public void AddPetStroke()
    {
        petStrokeCount++;
        UpdateStrokeUI();
    }
    void UpdateStrokeUI()
    {
        if (playerStrokeText != null)
            playerStrokeText.text = "Số gậy người chơi: " + playerStrokeCount;
        if (petStrokeText != null)
            petStrokeText.text = "Số gậy pet: " + petStrokeCount;
    }
    public void RegisterFinish(GolfBall ball)
    {
        if (gameEnded) return;
        if (ball == playerBall)
        {
            playerDone = true;
            if (!petDone && winText != null)
            {
                winText.text = "Bạn đã thắng!";
                winText.gameObject.SetActive(true);
            }
        }
        else if (ball == petBall)
        {
            petDone = true;
            if (!playerDone && winText != null)
            {
                winText.text = "Pet đã thắng!";
                winText.gameObject.SetActive(true);
            }
        }
        if (playerDone && petDone)
        {
            gameEnded = true;
        }
    }
}